using UnityEngine;
using UnityEngine.Assertions;
public class Computer : MonoBehaviour
{

    public SpriteRenderer worldspaceGUI;
    public Sprite TurnOnIcon;
    public Sprite LockIcon;
    public Sprite UnlockedIcon;
    public GameFileSystem fs;
    
    public enum LogInStatus
    {
        LoggedOut,
        ProfileChosen,
        LoggedIn
    }

    private LogInStatus status;


    private ProfileDatabase _profileDatabase;
    private string _chosenProfile;
    private string _rightPassword;
    private bool _isLocked;
    private bool _isOn;

    public bool IsOn => _isOn;

    public bool IsLoggedIn(out string loginId)
    {
        loginId = "";
        if (status == LogInStatus.LoggedIn)
        {
            loginId = _chosenProfile;
            return true;
        }

        return false;
    }

    public string Password
    {
        get
        {
            Assert.IsTrue(status != LogInStatus.LoggedOut, "Profile isn't chosen");
            return _rightPassword;
        }
    }
    
    public void SetNewPassword(string ID, string pass)
    {
        GameManager.gm.ProfileDatabase.GetProfile(ID).ChangePassword(pass);
    }

    private void Start()
    {
        _isOn = false;
        _profileDatabase = GameManager.gm.ProfileDatabase;
        UpdateWorldGui();
    }

    public void ChooseProfile(string ID)
    {
        Assert.IsTrue(_isOn, "Computer is not On");
        Assert.IsFalse(status == LogInStatus.LoggedIn, "Person is already logged in");
        _chosenProfile = ID;
        _rightPassword = _profileDatabase.GetProfile(ID).password;
    }

    private void UpdateWorldGui()
    {
        if (!_isOn) worldspaceGUI.sprite = TurnOnIcon;
        else
        {
            if (status != LogInStatus.LoggedIn)
            {
                worldspaceGUI.sprite = LockIcon;
            }
            else
            {
                worldspaceGUI.sprite = UnlockedIcon;
            }
        }
    }

    public void TurnOn()
    {
        _isOn = true;
        UpdateWorldGui();
    }

    public void TurnOff()
    {
        LogOut();
        _isOn = false;
        UpdateWorldGui();
    }

    public void LogOut()
    {
        status = LogInStatus.LoggedOut;
        UpdateWorldGui();
    }

    public void CheckPassword(string password, out bool result)
    {
        Assert.IsTrue(_isOn, "Computer isn't on");
        Assert.IsFalse(status == LogInStatus.ProfileChosen, "Profile isn't chosen");
        result = password == _rightPassword;
        if (result)
        {
            status = LogInStatus.LoggedIn;
            //Do something;
            UpdateWorldGui();
        }    
    }
    
    
    
}
