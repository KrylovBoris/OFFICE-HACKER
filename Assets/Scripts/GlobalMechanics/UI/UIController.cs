// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace GlobalMechanics.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject gamePlayUI;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private GameObject smartphoneUI;
        [SerializeField] private GameObject computerUI;
        [SerializeField] private GameObject dialogueUI;
        [SerializeField] private GameObject testUI;
        [SerializeField] private GameObject noteUI;

        private NoteUi note;

        private void Awake()
        {
            note = noteUI.GetComponent<NoteUi>();
        }

        public void ShowPauseMenu()
        {
            pauseUI.SetActive(true);
            gamePlayUI.SetActive(false);
        }

        public void HidePauseMenu()
        {
        
            pauseUI.SetActive(false);
            gamePlayUI.SetActive(true);
        }

        public void ShowSmartphone()
        {
            smartphoneUI.SetActive(true);
            gamePlayUI.SetActive(false);

        }

        public void HideSmartphone()
        {
            smartphoneUI.SetActive(false);
            gamePlayUI.SetActive(true);

        }

        public void ShowNote(string text)
        {
            noteUI.SetActive(true);
            note.Display(text);
            gamePlayUI.SetActive(false);
        }

        public void HideNote()
        {
            noteUI.SetActive(false);
            gamePlayUI.SetActive(true);
        }

        public void ShowComputer()
        {
            computerUI.SetActive(true);
            gamePlayUI.SetActive(false);

        }

        public void HideComputer()
        {
            computerUI.SetActive(false);
            gamePlayUI.SetActive(true);
        }

        public void ShowDialogue()
        {
            dialogueUI.SetActive(true);
            gamePlayUI.SetActive(false);

        }

        public void HideDialogue()
        {
            dialogueUI.SetActive(false);
            gamePlayUI.SetActive(true);
        }

    }
}
