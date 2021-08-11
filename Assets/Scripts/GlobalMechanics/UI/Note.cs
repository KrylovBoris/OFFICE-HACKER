// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace GlobalMechanics.UI
{
    public class Note : MonoBehaviour, IInteractable
    {
        private string _text;
        private bool isTextSet = false;
        private string caption = "Read";

        public void SetText(string newText)
        {
            if (!isTextSet)
            {
                _text = newText;
                isTextSet = true;
            }
        }

        public void Interact()
        {
            GameManager.gm.OpenNote(_text);
        }

        public string InteractionDescription()
        {
            return caption;
        }
    }
}
