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
