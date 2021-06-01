using System;
using System.Collections.Generic;
using Global_Mechanics;
using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(fileName = "Personality", menuName = "ScriptableObjects/Personality", order = 1)]
    public class Personality : ScriptableObject
    {
        public string personName;
        public float passwordTypingTime = 5.0f;
        public PersonalityTrait[] traitArray;
        public Relationship[] relationships;

        private string _knownPassword;
        private string _logInId;
        private int _personalityMask;
        private Dictionary<PersonalityTrait.TraitType, PersonalityTrait> _traitTypeToTrait;
        private Dictionary<Personality, Relationship> _personToRelationship;

        public string LogInId
        {
            get
            {
                if (_logInId == String.Empty)
                {
                    SetAuthenticationData();
                }
                return _logInId;
            }
        }

        public string KnownPassword {            
            get
            {
                if (_knownPassword == String.Empty)
                {
                    SetAuthenticationData();
                }
                return _knownPassword;
            } 
        }
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
    
        private float GetTraitNormalisedIntensity(PersonalityTrait.TraitType traitType)
        {
            if (!_traitTypeToTrait.ContainsKey(traitType))
            {
                return 0;
            }
            return _traitTypeToTrait[traitType].NormalisedIntensity;
        }
        
        void OnEnable()
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
            var personNegligence = this.GetTraitNormalisedIntensity(PersonalityTrait.TraitType.Negligence);
            var personTechnicalKnowledge = this.GetTraitNormalisedIntensity(PersonalityTrait.TraitType.TechnicalKnowledge);
            return ((0.7f * NormalisedSigmoid(personNegligence) + 0.3f * NormalisedSigmoid(personTechnicalKnowledge)));
        }

        public float ConversationJoiningProbability(Conversation conversation)
        {
            var personCuriosity =
                this.GetTraitNormalisedIntensity(
                    PersonalityTrait.TraitType.Curiosity);
            var personWillingness = this.GetTraitNormalisedIntensity(
                PersonalityTrait.TraitType.WillingnessToHelp);
            var personTrust = this.GetTraitNormalisedIntensity(
                PersonalityTrait.TraitType.Trust);

            var conversationRating = 0f;
            foreach (var agent in conversation.Interlocutors)
            {
                var relationship = GetRelation(agent);
                
                if (_personToRelationship[agent.Personality].IsSubordinate || this.IsManagedBy(agent))
                {
                    var responsibility = -GetTraitNormalisedIntensity(
                        PersonalityTrait.TraitType.Negligence);
                    var confidence = -GetTraitNormalisedIntensity(
                        PersonalityTrait.TraitType.Fearfulness);
                    relationship = NormalisedSigmoid((responsibility + confidence + relationship) / 3f);
                }
                conversationRating += relationship;
            }

            conversationRating /= conversation.Interlocutors.Count;

            return 0.3f * NormalisedSigmoid(0.4f * personCuriosity + 0.4f * personTrust + 0.2f * personWillingness) +
                   0.7f * NormalisedSigmoid(conversationRating);
        }

        public float GetRelation(BaseAgent other)
        {
            if (!_personToRelationship.ContainsKey(other.Personality)) return 0f;
            
            return _personToRelationship[other.Personality].NormalizedIntensity;
        }
        
        public float GetRelation(Personality other)
        {
            if (!_personToRelationship.ContainsKey(other)) return 0f;
            
            return _personToRelationship[other].NormalizedIntensity;
        }

        public bool IsManagedBy(BaseAgent other)
        {
            return other.Personality._personToRelationship[this].IsSubordinate;
        }

        private static float NormalisedSigmoid(float val)
        {
            var f = Mathf.Exp(val/0.1f);
            return 1 / (1 + f);
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

        public float ChatProbability(Personality secondPersonality)
        {
            var personCuriosity =
                this.GetTraitNormalisedIntensity(
                    PersonalityTrait.TraitType.Curiosity);
            var personWillingness = this.GetTraitNormalisedIntensity(
                PersonalityTrait.TraitType.WillingnessToHelp);
            var personTrust = this.GetTraitNormalisedIntensity(
                PersonalityTrait.TraitType.Trust);

            var conversationRating = 0f;
            var relationship = GetRelation(secondPersonality);
                
            if (_personToRelationship[secondPersonality].IsSubordinate || this.IsManagedBy(secondPersonality))
            {
                var responsibility = -GetTraitNormalisedIntensity(
                    PersonalityTrait.TraitType.Negligence);
                var confidence = -GetTraitNormalisedIntensity(
                    PersonalityTrait.TraitType.Fearfulness);
                relationship = NormalisedSigmoid((responsibility + confidence + relationship) / 3f);
            }
            conversationRating += relationship;

            return 0.3f * NormalisedSigmoid(0.4f * personCuriosity + 0.4f * personTrust + 0.2f * personWillingness) +
                   0.7f * NormalisedSigmoid(conversationRating);
        }

        private bool IsManagedBy(Personality secondPersonality)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class Relationship
    {
        [Range(-100f, 100f)] 
        [SerializeField]
        private float intensity;
        
        [SerializeField]
        private bool isSubordinate;
        [SerializeField]
        private Personality subject;

        public bool OtherPerson => subject;
        public bool IsSubordinate => isSubordinate;

        public float NormalizedIntensity => intensity / 100f;
    }
}
