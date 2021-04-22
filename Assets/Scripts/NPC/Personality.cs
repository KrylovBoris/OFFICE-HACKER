using System;
using System.Collections.Generic;
using Global_Mechanics;
using NPC;
using UnityEngine;

public class Personality : MonoBehaviour
{
    public string personName;
    public float passwordTypingTime = 5.0f;
    public PersonalityTrait[] traitArray;
    private string _knownPassword;
    private string _logInId;
    private int _personalityMask;
    private Dictionary<PersonalityTrait.TraitType, PersonalityTrait> _traitTypeToTrait;
    public string LogInId => _logInId;
    public string KnownPassword => _knownPassword;
    public int PersonalityMask => _personalityMask;

    private float _suspicion = 0.0f;

    public float Suspicion => _suspicion;

    public float GetTraitIntensity(PersonalityTrait.TraitType traitType)
    {
        if (!_traitTypeToTrait.ContainsKey(traitType))
        {
            return 0;
        }

        return _traitTypeToTrait[traitType].Intensity;
    }

    // Start is called before the first frame update
    void Start()
    {
        _traitTypeToTrait = new Dictionary<PersonalityTrait.TraitType, PersonalityTrait>();
        _personalityMask = PersonalityTrait.GetTraitBitMask(traitArray);
        foreach (var trait in traitArray)
        {
            if (_traitTypeToTrait.ContainsKey(trait.type))
            {
                throw new Exception("Multiple same-type personality traits detected");
            }
            else
            {
                _traitTypeToTrait.Add(trait.type, trait);
            }
        }

        SetAuthenticationData();
    }

    public void SetAuthenticationData()
    {
        var db = GameManager.gm.ProfileDatabase;
        _logInId = db.GetLogIn(personName);
        _knownPassword = db.GetProfile(_logInId).password;
    }

    public void RaiseSuspicion(float amount)
    {
        UnityEngine.Debug.Log("Suspicion raised by " + amount);
        _suspicion += amount * -Probabilities.UniformDistribution(-100, 100,
                          _traitTypeToTrait[PersonalityTrait.TraitType.Trust].Intensity);
    }

    public float NoteLostProbability()
    {
        var personNegligence = this.GetTraitIntensity(PersonalityTrait.TraitType.Negligence);
        var personTechnicalKnowledge = this.GetTraitIntensity(PersonalityTrait.TraitType.TechnicalKnowledge);
        return 0.005f * ((0.7f * Sigmoid(personNegligence) + 0.3f * Sigmoid(personTechnicalKnowledge)) + 100);
    }

    private static float Sigmoid(float val)
    {
        var f = Mathf.Exp(val);
        return f / (1 + f);
    }

public string NoteText()
    {
        return "Password: " + KnownPassword;
    }

    public float UsbAttackProbability()
    {
        //TODO USB attack
        return 0f;
    }
}
