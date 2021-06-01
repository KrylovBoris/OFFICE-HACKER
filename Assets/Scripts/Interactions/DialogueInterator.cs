using Agent;
using NPC;
using UnityEngine;

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
