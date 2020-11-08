using System.Collections.Generic;
using Global_Mechanics;
using UnityEngine;

[CreateAssetMenu(fileName = "Commutator", menuName = "ScriptableObjects/EmailCommutator", order = 1)]
public class EmailCommutator : ScriptableObject
{
    
    public string[] friendlyInformalTexts;
    public string[] neutralInformalTexts;
    public string[] unfriendlyInformalTexts;
    
    public string[] friendlyFormalTexts;
    public string[] neutralFormalTexts;
    public string[] unfriendlyFormalTexts;
    
    public string[] friendlyInformalOpenings;
    public string[] neutralInformalOpenings;
    public string[] unfriendlyInformalOpenings;
    
    public string[] formalOpenings;
    
    public string[] friendlyInformalClosings;
    public string[] neutralInformalClosings;
    public string[] unfriendlyInformalClosings;

    public string[] formalClosings;

    public Email greetingLetter;
    public Email storeEmail;
    
    private Dictionary<string, EmailBox> _mapClients;
    
    
    // Start is called before the first frame update
    public void Initiate()
    {
        _mapClients = new Dictionary<string, EmailBox>();    
    }

    public void EstablishConnection(EmailBox newClient)
    {
        _mapClients.Add(newClient.Address, newClient);
    }

    public void SendLetter(Email letter)
    {
        if (_mapClients.ContainsKey(letter.Receiver))
        {
            _mapClients[letter.Receiver].ReceiveEmail(letter);
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid receiver address");
        }
    }

    public void BroadcastLetter(Email letter)
    {
        foreach (var user in _mapClients.Keys)
        {
            if (user != GameManager.gm.PlayerEmail)
            {
                _mapClients[user].ReceiveEmail(letter);
            }
        }
    }
    
    
}
