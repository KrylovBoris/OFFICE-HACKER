using System;
using UnityEngine;
using Player;

public class GameManager : MonoBehaviour
{
    public static GameManager gm = null;
    public enum GameMode {FPGameplay, Pause, Computer, ItemInteraction, Smartphone}

    private GameMode currentGameMod;

    [SerializeField] private UIController uiController;
    public GameObject player;
    private string _playerEmail;
    public string PlayerEmail => _playerEmail;
    
    [Header("In-game Time Settings")] 
    
    [SerializeField] private float secondsInMinute = 60;
    [SerializeField] private int startingHours;
    [SerializeField] private int startingMinutes;
    [SerializeField] private int endingHours;
    [SerializeField] private int endingMinutes;
    public float MinuteDuration => secondsInMinute;
    private float _seconds;
    private (int hours, int minutes) _internalTime;
    public (int hours, int minutes) StartingTime => (startingHours, startingMinutes);
    public (int hours, int minutes) ClosingTime => (endingHours, endingMinutes);

    private float _secondsFromStart;
    public float SecondsFromStart => _secondsFromStart;
    private ProfileDatabase _profileDatabase;
    private InputController _playerInput;
    [SerializeField]
    private EmailCommutator commutator;
    public ProfileDatabase ProfileDatabase
    {
        get
        {
            if (_profileDatabase == null)
            {
                _profileDatabase = GetComponent<ProfileDatabase>();
            }

            return _profileDatabase;
        }
    }
    public EmailCommutator Commutator
    {
        get
        {
            if (_profileDatabase == null)
            {
                commutator = GetComponent<EmailCommutator>();
            }

            return commutator;
        }
    }
    
    private CameraScript _cameraScript;
    public bool IsPaused { get; private set; }
    public bool IsLookingAtSmartPhone { get; private set; }
    
    [SerializeField]
    
    private void Awake()
    {
        if (gm == null) gm = this;
        else 
            if (gm != this)
            {
                Debug.LogError("Multiple GameManagers in the scene. Destroying.");
                Destroy(gameObject);
            }
        commutator.Initiate();
    }

    public void SetPlayerEmail(string address)
    {
        _playerEmail = address;
    }
    
    void Start()
    {
        _secondsFromStart = startingHours * 60 * MinuteDuration + startingMinutes * MinuteDuration;
        currentGameMod = GameMode.FPGameplay;
        _cameraScript = player.GetComponent<CameraScript>();
        _profileDatabase = GetComponent<ProfileDatabase>();
        _playerInput = player.GetComponent<InputController>();
        _internalTime = StartingTime;
        _seconds = 0;
    }

    public void SendGreetingLetters()
    {
        commutator.SendLetter(commutator.greetingLetter);
        commutator.SendLetter(commutator.storeEmail);

    }

    private void Update()
    {
        _seconds += Time.deltaTime;
        _secondsFromStart += Time.deltaTime;
        if (_seconds > MinuteDuration)
        {
            _seconds = 0;
            _internalTime.minutes++;
            if (_internalTime.minutes == 60)
            {
                _internalTime.minutes = 0;
                _internalTime.hours++;
            }
        }

        if (_internalTime == ClosingTime)
        {
            _internalTime = StartingTime;
            _secondsFromStart = startingHours * 60 * MinuteDuration + startingMinutes * MinuteDuration;
        }
    }

    public void Escape()
    {
        switch (currentGameMod)
        {
            case GameMode.FPGameplay:
                Pause();
                break;
            case GameMode.Pause:
                Resume();
                break;
            case GameMode.Computer:
                throw new NotImplementedException();
                break;
            case GameMode.ItemInteraction:
                CloseNote();
                break;
            case GameMode.Smartphone:
                CloseSmartphone();
                Pause();
                break;
            default: throw new NotImplementedException();
        }
    }

    public void Pause()
    {
        Debug.Assert(!IsPaused);
        currentGameMod = GameMode.Pause;
        Time.timeScale = 0.0f;
        uiController.ShowPauseMenu();
        _cameraScript.UnlockCursor();
        IsPaused = true;
    }

    public void Resume()
    {
        currentGameMod = GameMode.FPGameplay;
        Time.timeScale = 1.0f;
        uiController.HidePauseMenu();
        _cameraScript.LockCursor();
        IsPaused = false;
    }

    public void OpenSmartphone()
    {
        if (currentGameMod == GameMode.FPGameplay)
        {
            _playerInput.LockControls();
            currentGameMod = GameMode.Smartphone;
            uiController.ShowSmartphone();
            _cameraScript.UnlockCursor();
            IsLookingAtSmartPhone = true;
        }
    }

    public void CloseSmartphone()
    {
        _playerInput.UnLockControls();
        currentGameMod = GameMode.FPGameplay;
        uiController.HideSmartphone();
        _cameraScript.LockCursor();
        IsLookingAtSmartPhone = false;
    }

    public void OpenNote(string text)
    {
        _playerInput.LockControls();
        currentGameMod = GameMode.ItemInteraction;
        _cameraScript.UnlockCursor();
        uiController.ShowNote(text);
    }

    public void CloseNote()
    {
        _playerInput.UnLockControls();
        currentGameMod = GameMode.FPGameplay;
        _cameraScript.LockCursor();
        uiController.HideNote();
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
