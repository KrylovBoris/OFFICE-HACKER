using System;
using NPC;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class PasswordPeek : MonoBehaviour
{
    public float letterGuessProbability = 0.7f;
    public float letterChangeThres = 0.1f;
    public GameObject UI;
    public Image progressBar;
    public Image alertBar;
    public TextMeshProUGUI[] letterHolders;
    public GameObject peekHint;
    private bool _canPeek;
    private bool _isPeeking;
    private int _currentLetterIndex;
    private float _lettersChanged;
    private string _passwordOwner;
    private string _learntPassword;
    private string _password;
    private string _knownToPlayerPassword;
    private Personality _availableVictim = null;
    private PlayerKnowledgeContainer memory;
    private float _startTime;
    private float[] _timeStamps;
    // Start is called before the first frame update
    private const string PasswordAlphabet = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    void Start()
    {
        memory = GetComponent<PlayerKnowledgeContainer>();
        UI.SetActive(false);
        foreach (var holder in letterHolders)
        {
            holder.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPeeking)
        {
            progressBar.fillAmount = (Time.time - _startTime) / _availableVictim.passwordTypingTime;
            if (Time.time - _lettersChanged > letterChangeThres)
            {
                for (int i = 0; i < letterHolders.Length; i++)
                {
                    if (i >= _currentLetterIndex)
                    {
                        letterHolders[i].text = RandomLetter();
                    }
                    else
                    {
                        letterHolders[i].text = _learntPassword[i].ToString();
                    }
                }
                _lettersChanged = Time.time;
            }
            UpdateIndex();
            if (_knownToPlayerPassword == string.Empty)
            {
                if (_learntPassword.Length <= _currentLetterIndex)
                {
                    _learntPassword += GuessLetter(_currentLetterIndex);
                }
            }
            else
            {
                if (_knownToPlayerPassword[_currentLetterIndex] == '?')
                {
                    var arr = _learntPassword.ToCharArray();
                    arr[_currentLetterIndex] = GuessLetter(_currentLetterIndex)[0];
                    _learntPassword = new string(arr);
                }
                    
            }
            
        }
    }

    public bool CanPeek => _canPeek;

    public bool IsPeeking => _isPeeking;

    public void MakePeekingPossible(Personality victim, float startTime)
    {
        _canPeek = true;
        _availableVictim = victim;
        _startTime = startTime;
        peekHint.SetActive(true);
    }

    public void ActivatePeeking()
    {
        Assert.IsTrue(_canPeek, "Player shouldn't be able to peek");
        var knowledge = new PlayerKnowledge(_availableVictim.personName);
        knowledge.LearnLogin(_availableVictim.LogInId);
        memory.MakeKnowledgeTransaction(knowledge);
        _isPeeking = true;
        _learntPassword = "";
        _password = _availableVictim.KnownPassword;
        _knownToPlayerPassword = memory.KnownPassword(_availableVictim.personName);
        UI.SetActive(true);
        var t = _availableVictim.passwordTypingTime / _password.Length;
        _timeStamps = new float[_password.Length]; 
        
        for (int i = 0; i < _password.Length; i++)
        {
            _timeStamps[i] = i * t;
            letterHolders[i].gameObject.SetActive(true);
            if (memory.KnownPassword(_availableVictim.personName) == String.Empty)
            {
                if ((Time.time - _startTime) > _timeStamps[i])
                {
                    letterHolders[i].text = "?";
                    _learntPassword += "?";
                }
            }
            else
            {
                letterHolders[i].text = _knownToPlayerPassword[i].ToString();
                _learntPassword += _knownToPlayerPassword[i].ToString();
            }
        }
        _lettersChanged = Time.time;
        progressBar.fillAmount = 0;
        alertBar.fillAmount = 0;
    }
    
    public void DeactivatePeeking()
    {
        if (IsPeeking)
        {
        _isPeeking = false;
        UI.SetActive(false);
        foreach (var holder in letterHolders)
        {
            holder.gameObject.SetActive(false);
        }
        var knowledge = new PlayerKnowledge(_availableVictim.personName);
        knowledge.LearnNewPassword(_learntPassword);
        memory.MakeKnowledgeTransaction(knowledge);
        }
    }

    public void MakePeekingImpossible()
    {
        _canPeek = false;
        peekHint.SetActive(false);
    }

    private void UpdateIndex()
    {
        for (int i = 0; i < _timeStamps.Length; i++)
        {
            if ((Time.time - _startTime) > _timeStamps[i])
            {
                _currentLetterIndex = i;
            }
        }
    }
    
    private string GuessLetter(int index)
    {
        var coinToss = UnityEngine.Random.Range(0.0f, 1.0f);
        if (coinToss > letterGuessProbability)
        {
            return "?";
        }
        else
        {
            return _password[index].ToString();
        }
    }
    
    private string RandomLetter()
    {
        return PasswordAlphabet[UnityEngine.Random.Range(0, PasswordAlphabet.Length)].ToString();
        
    }
}
