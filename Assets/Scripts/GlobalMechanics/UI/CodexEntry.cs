// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using TMPro;
using UnityEngine;

namespace GlobalMechanics.UI
{
    public class CodexEntry : MonoBehaviour
    {
        public TextMeshProUGUI codexEntryName;
        public TextMeshProUGUI codexEntryContentsNumber;
    
        private string _key;
        private bool _isKeySet;
        private CodexController _controller;

        public void SetKey(string k, CodexController keySource, int number = 0)
        {
            if (!_isKeySet)
            {
                _key = k;
                _isKeySet = true;
                _controller = keySource;
                codexEntryName.text = _key;
                if (number > 0)
                {
                    codexEntryContentsNumber.text = number.ToString();
                }
                else
                {
                    codexEntryContentsNumber.text = "";
                }
            }
        }

        public bool IsKeySet => _isKeySet;

        public void OpenItem()
        {
            _controller.OpenEntry(_key);
        }
    }
}
