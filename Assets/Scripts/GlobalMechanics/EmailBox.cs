// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using System.Collections.Generic;
using GlobalMechanics.ScriptableObjects;
using NPC;
using UnityEngine;

namespace GlobalMechanics
{
    public class EmailBox
    {
        readonly string _receiverAddress;
        private Dictionary<string, EmailShell> _inBox;
        private Dictionary<string, EmailShell> _outBox;

        public string Address => _receiverAddress;
    
        public EmailBox(string address)
        {
            _receiverAddress = address;
            _inBox = new Dictionary<string, EmailShell>();
            _outBox = new Dictionary<string, EmailShell>();
        }
    
        public void ReceiveEmail(Email mail)
        {
            var duplicateNumber = 0;
            while (_inBox.ContainsKey(mail.Sender + "|" + mail.Topic) || _inBox.ContainsKey(mail.Sender + "|" + mail.Topic + " (" + duplicateNumber + ")"))
            {
                duplicateNumber++;
            }
            mail.MarkTopicAsDuplicate(duplicateNumber);
            _inBox.Add(mail.Sender + "|" + mail.Topic, new EmailShell(mail));
        }
    
        private class EmailShell
        {
            private Email _letter;
            private bool _read;
            private bool _seen;
            public EmailShell(Email l)
            {
                _letter = l;
                _read = false;
                _seen = false;
            }

            public Email Letter => _letter;
            public bool IsRead => _read;

            public bool IsSeen => _seen;

            public void MarkAsRead()
            {
                _read = true;
            }
        
            public void MarkAsSeen()
            {
                _seen = true;
            }
        }

        public void MarkAsRead(string readLetter)
        {
            _inBox[readLetter].MarkAsRead();
        }

        public void MarkAllUnSeen()
        {
            foreach (var key in _inBox.Keys)
            {
                if (!_inBox[key].IsSeen)
                {
                    _inBox[key].MarkAsSeen();
                }
            }
        }

        public string[] InBoxKeys()
        {
            var keys = _inBox.Keys;
            var result = new string[keys.Count];
            keys.CopyTo(result, 0);
            return result;
        }

        public string[] OutBoxKeys()
        {
            var keys = _outBox.Keys;
            var result = new string[keys.Count];
            keys.CopyTo(result, 0);
            return result;
        }
    
        public bool IsEmailRead(string key)
        {
            return _inBox[key].IsRead;
        }
    
        public bool IsEmailSeen(string key)
        {
            return _inBox[key].IsSeen;
        }

        public Email GetEmailByKey(string key)
        {
            if (_inBox.ContainsKey(key))
            {
                return _inBox[key].Letter;
            }

            return _outBox[key].Letter;

        }

        public bool HasAnyUnreadEmail()
        {
            foreach (var k in _inBox.Keys)
            {
                if (!IsEmailSeen(k)) return false;
            }

            return true;
        }

        public void NpcCheckMail(Personality profile)
        {
            //TODO Make attack
        
            foreach (var key in _inBox.Keys)
            {
                if (!_inBox[key].IsSeen)
                {
                    _inBox[key].MarkAsRead();
                }
            }
            Debug.Log("NPC checked e-mail");
        }
    }
}
