using GlobalMechanics.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlobalMechanics.UI
{
    public class LetterButtonDisplay : MonoBehaviour
    {

        public enum LetterStatus
        {
            New,
            SeenUnread,
            Read,
            Sent
        }

        public TextMeshProUGUI topicTmp;

        public TextMeshProUGUI personTmp;

        public Image letterIcon;

        public Sprite newIcon;
        public Sprite unreadIcon;
        public Sprite readIcon;
        public Sprite sentIcon;

        private IMail _mailController;
    
        private string _fromString = "From: ";
        private string _toString = "To: ";
        private string _key;

        public string Key => _key;

        public void SetMailController(IMail mailController)
        {
            _mailController = mailController;
        }
    
        public void DisplayOutLetter(Email letter)
        {
            topicTmp.text = letter.Topic;
            personTmp.text = _toString + letter.Receiver;
            _key = letter.Sender + "|" + letter.Topic;
            letterIcon.sprite = sentIcon;
        }

        public void DisplayInLetter(Email letter, bool isRead, bool isSeen)
        {
            topicTmp.text = letter.Topic;
            personTmp.text = _fromString + letter.Sender;
            if (isSeen)
            {
                letterIcon.sprite = isRead ? readIcon : unreadIcon; 
            }
            else
            {
                letterIcon.sprite = newIcon;
            }
            _key = letter.Sender + "|" + letter.Topic;
        }

        public void RefreshIcon(bool isRead, bool isSeen)
        {
            if (isSeen)
            {
                letterIcon.sprite = isRead ? readIcon : unreadIcon; 
            }
            else
            {
                letterIcon.sprite = newIcon;
            }
        }
    
        public void OpenLetter()
        {
            _mailController.OpenLetter(_key);
        }
    }
}
