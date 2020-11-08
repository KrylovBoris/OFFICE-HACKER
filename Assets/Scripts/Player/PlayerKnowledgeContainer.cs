using System.Collections.Generic;
using UnityEngine;

public class PlayerKnowledgeContainer : MonoBehaviour
{
    private Dictionary<string, PlayerKnowledge> _availableKnowledge;

    private List<PlayerKnowledge> _knowledgeTransactionList;
    // Start is called before the first frame update
    void Start()
    {
        _availableKnowledge = new Dictionary<string, PlayerKnowledge>();
        _knowledgeTransactionList = new List<PlayerKnowledge>();
    }

    public void LearnNewPerson(string personName)
    {
        UnityEngine.Debug.Log("Learnt" + personName);
        _availableKnowledge.Add(personName, new PlayerKnowledge(personName));        
    }

    public void LearnNewLogin(string personName, string login)
    {
        UnityEngine.Debug.Log("Learnt login of" + personName + ": " + login);
        if (!_availableKnowledge.ContainsKey(personName)) 
            LearnNewPerson(personName);
        _availableKnowledge[personName].LearnLogin(login);
    }
    
    public void LearnNewPassword(string personName, string password)
    {
        UnityEngine.Debug.Log("Learnt password of" + personName + ": " + password);
        if (!_availableKnowledge.ContainsKey(personName)) 
            LearnNewPerson(personName);
        _availableKnowledge[personName].LearnNewPassword(password);
    }

    public string KnownPassword(string personName)
    {
        return _availableKnowledge[personName].Password;
    }
    
    public bool PlayerKnowPerson(string personName)
    {
        
        return _availableKnowledge.ContainsKey(personName);
    }

    public void MakeKnowledgeTransaction(PlayerKnowledge newKnowledge)
    {
        if (!_availableKnowledge.ContainsKey(newKnowledge.Name))
        {
            LearnNewPerson(newKnowledge.Name);
        }

        if (_availableKnowledge[newKnowledge.Name] != newKnowledge)
        {
            if (!_knowledgeTransactionList.Contains(newKnowledge))
                _knowledgeTransactionList.Add(newKnowledge);
            _availableKnowledge[newKnowledge.Name] += newKnowledge;
        }

    }
}
