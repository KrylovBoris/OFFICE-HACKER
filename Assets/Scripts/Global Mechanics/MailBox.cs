using System.Collections.Generic;
using Player;
using UnityEngine;

public class MailBox : MonoBehaviour, IInteractable
{
    public ParticleSystem fillIndicator;
    private bool _isFilled = false;
    private Dictionary<Item, uint> _package;
    private string _mailboxCaption = "Take your purchases";
    public void Interact()
    {
        if (_isFilled)
        {
            var playerInventory = GameManager.gm.player.GetComponent<PlayerInventory>();
            foreach (var key in _package.Keys)
            {
                playerInventory.IncreaseItem(key, _package[key]);
            }
            fillIndicator.Stop();
            _isFilled = false;
            _package = null;
        }
    }

    public string InteractionDescription()
    {
        return _mailboxCaption;
    }

    public void Fill(Dictionary<Item, uint> fillings)
    {
        if (_package == null)
        {
            _package = fillings;
            _isFilled = true;
            fillIndicator.Play();
        }
        else
        {
            foreach (var k in fillings.Keys)
            {
                if (_package.ContainsKey(k))
                {
                    _package[k] += fillings[k];
                }
                else
                {
                    _package.Add(k, fillings[k]);
                }
            }   
        }
    }

    private void Start()
    {
        fillIndicator.Stop();
    }
}
