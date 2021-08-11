﻿// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using GlobalMechanics.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlobalMechanics.UI
{
    public class EmailDisplay : MonoBehaviour
    {
        public TextMeshProUGUI senderUi;
        public TextMeshProUGUI receiverUi;
        public TextMeshProUGUI topicTmp;
        public TextMeshProUGUI emailTextUi;
        public Image attachmentIcon;
        public TextMeshProUGUI attachmentText;

        private Email _email;
    
    
        public void DisplayMail(Email mail)
        {
            _email = mail;
            ShowMailContents();
        }

        private void ShowMailContents()
        {
            senderUi.text = _email.Sender;
            receiverUi.text = _email.Receiver;
            topicTmp.text = _email.Topic;
            emailTextUi.text = _email.Text;
            if (_email.Attachment != null)
            {
                attachmentText.text = _email.Attachment.Name;
            }

        }
    }
}
