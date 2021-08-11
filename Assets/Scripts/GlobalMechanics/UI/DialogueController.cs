// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using NPC;
using UnityEngine;

namespace GlobalMechanics.UI
{
    public class DialogueController : MonoBehaviour
    {

        private Personality _interlocutor;

        public void SetInterlocutor(Personality npc)
        {
            _interlocutor = npc;
        }
    
    
    }
}
