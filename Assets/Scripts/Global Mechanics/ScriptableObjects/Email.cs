using FileSystem;
using UnityEngine;

namespace Global_Mechanics
{
    [CreateAssetMenu(fileName = "Email", menuName = "ScriptableObjects/E-Mail", order = 2)]
    public class Email:ScriptableObject
    {
        [SerializeField]
        private string _sender;
        [SerializeField]
        private string _receiver;
        [SerializeField]
        private string _topic;
        [SerializeField]
        [Multiline]
        private string _text;
        private File _attachment;

        public string Sender => _sender;
        public string Receiver => _receiver;

        public string Topic => _topic;

        public string Text => _text;
        public File Attachment => _attachment;

        public Email(string author, string receiver, string topic, string mainText, File attach)
        {
            _sender = author;
            _receiver = receiver;
            _topic = topic;
            _text = mainText;
            _attachment = attach;
        }

        public void MarkTopicAsDuplicate(int index)
        {
            if (index > 0)
            {
                _topic += " (" + index + ")";
            }
        }
        
    }
}