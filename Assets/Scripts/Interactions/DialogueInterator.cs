// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using GlobalMechanics.UI;
using NPC;
using UnityEngine;

namespace Interactions
{
    public class DialogueInterator : MonoBehaviour, IInteractable
    {
        private string _interactionString = "Talk";
        private InterruptionFlagsHandler _handler;

        private Personality _personality;
        // Start is called before the first frame update
        void Start()
        {
            _handler = GetComponent<InterruptionFlagsHandler>();
            _personality = GetComponent<Personality>();
        }

        public void Interact()
        {
            _handler.RaiseFlag(InterruptionFlag.Talk);
        }

        public string InteractionDescription()
        {
            return _interactionString;
        }
    }
}
