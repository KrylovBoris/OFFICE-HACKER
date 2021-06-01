using NPC;
using UnityEngine;

public class DialogueController : MonoBehaviour
{

    private Personality _interlocutor;

    public void SetInterlocutor(Personality npc)
    {
        _interlocutor = npc;
    }
    
    
}
