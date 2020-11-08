using System.Collections;
using UnityEngine;

public class MailSmartphoneUI : MonoBehaviour, ISmartphoneService, IMail, INotification
{

    public string emailAddress;

    private EmailBox _mailbox;
    public float timeToMarkAllSeen = 1.0f;
    public GameObject emailDisplayer;
    //public bool hasMenu = true;
    public GameObject menu;
    public GameObject emailButtonPrefab;
    public RectTransform buttonHolder;

    private bool _isDisplayingIns = true;
    
    // Start is called before the first frame update

    public void OpenLetter(string key)
    {
        var letter = _mailbox.GetEmailByKey(key);
        emailDisplayer.SetActive(true);
        emailDisplayer.GetComponent<EmailDisplay>().DisplayMail(letter);
        _mailbox.MarkAsRead(key);
    }

    private void ShowMenu()
    {
        menu.SetActive(true);
    }

    public void HideMenu()
    {
        menu.SetActive(false);
    }

    public void ToggleMenu()
    {
        if (!menu.activeInHierarchy)
        {
            ShowMenu();
            DisableButtons();
        }
        else
        {
            HideMenu();
            EnableButtons();
        }
    }

    public void DisplayMail(string key)
    {
        emailDisplayer.SetActive(true);
        emailDisplayer.GetComponent<EmailDisplay>().DisplayMail(_mailbox.GetEmailByKey(key));
        _mailbox.MarkAsRead(key);
        RefreshButtonIcons();
        DisableButtons();
    }

    public void HideMail()
    {
        emailDisplayer.SetActive(false);
        EnableButtons();
    }

    private void DisableButtons()
    {
        foreach (UnityEngine.UI.Button button in buttonHolder.GetComponentsInChildren<UnityEngine.UI.Button>())
        {
            button.interactable = false;
        }
    }

    private void EnableButtons()
    {
        foreach (UnityEngine.UI.Button button in buttonHolder.GetComponentsInChildren<UnityEngine.UI.Button>())
        {
            button.interactable = true;
        }
    }

    public void DisplayMails(bool displayIns)
    {
        _isDisplayingIns = displayIns;
        for (int i = 0; i < buttonHolder.childCount; i++)
        {
            Destroy(buttonHolder.GetChild(i).gameObject);
        }
        
        if (_isDisplayingIns)
        {
            var ins = _mailbox.InBoxKeys();
            for(int i = 0; i < ins.Length; i++)
            {
                InstantiateEmailButton(ins[i]);
            }
    
    
                StartCoroutine(MarkAllLettersAsSeen());
        }
        else
        {
            var outs = _mailbox.OutBoxKeys();
            for(int i = 0; i < outs.Length; i++)
            {
                InstantiateEmailButton(outs[i]);
            }
        }
    }

    private void InstantiateEmailButton(string letterKey)
    {
        var button = Instantiate(emailButtonPrefab, buttonHolder);
        var displayComponent = button.GetComponent<LetterButtonDisplay>();
        if (_isDisplayingIns)
        {
            displayComponent.DisplayInLetter(_mailbox.GetEmailByKey(letterKey), _mailbox.IsEmailRead(letterKey), _mailbox.IsEmailSeen(letterKey));
        }
        else
        {
            displayComponent.DisplayOutLetter(_mailbox.GetEmailByKey(letterKey));
        }
        displayComponent.SetMailController(this);
    }

    public void Back()
    {
        if (menu.activeInHierarchy)
        {
            HideMenu();
        }
        else
        {
            if (emailDisplayer.activeInHierarchy)
            {
                HideMail();
            }
            else
            {
                Home();
            }
        }
    }

    public void Home()
    {
        HideMenu();
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        if (_mailbox == null)
        {
            _mailbox = new EmailBox(emailAddress);
            GameManager.gm.SetPlayerEmail(emailAddress);
            GameManager.gm.Commutator.EstablishConnection(_mailbox);
            GameManager.gm.SendGreetingLetters();
        }
        this.gameObject.SetActive(true);
        _isDisplayingIns = true;
        DisplayMails(_isDisplayingIns);
    }

    public bool IsOpened()
    {
        return this.gameObject.activeInHierarchy;
    }

    public void RefreshButtonIcons()
    {
        foreach (var button in buttonHolder.GetComponentsInChildren<LetterButtonDisplay>())
        {
            string letterShell = button.Key;
            button.RefreshIcon(_mailbox.IsEmailRead(letterShell), _mailbox.IsEmailSeen(letterShell));
        }
    }

    private IEnumerator MarkAllLettersAsSeen()
    {
        yield return new WaitForSeconds(timeToMarkAllSeen);
        _mailbox.MarkAllUnSeen();
        RefreshButtonIcons();
    }

    public string NotificationAnimationBool()
    {
        return "UnreadLettersInMailbox";
    }

    public bool HasToShowNotification()
    {
        return _mailbox.HasAnyUnreadEmail();
    }
}
