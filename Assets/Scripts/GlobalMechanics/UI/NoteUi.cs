// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using TMPro;
using UnityEngine;

namespace GlobalMechanics.UI
{
    public class NoteUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI noteText;

        public void Display(string text)
        {
            noteText.text = text;
        }
    
    }
}
