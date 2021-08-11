// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using GlobalMechanics.UI;
using Player;
using UnityEngine;

namespace GlobalMechanics
{
    public class MailBox : MonoBehaviour, IInteractable
    {
        public ParticleSystem fillIndicator;
        private bool _isFilled = false;
        private PlayerInventory.Package _package;
        private string _mailboxCaption = "Take your purchases";
    
        public void Interact()
        {
            if (_isFilled)
            {
                var playerInventory = GameManager.gm.player.GetComponent<PlayerInventory>();
                _package.Unpack();
                fillIndicator.Stop();
                _isFilled = false;
                _package = null;
            }
        }

        public string InteractionDescription()
        {
            return _mailboxCaption;
        }

        public void Fill(PlayerInventory.Package fillings)
        {
            if (_package == null)
            {
                _package = fillings;
                _isFilled = true;
                fillIndicator.Play();
            }
            else
            {
                _package.UniteWith(fillings);
            }
        }

        private void Start()
        {
            fillIndicator.Stop();
        }
    }
}
