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


        private static ProbabilityData _ourProbabilityData;
        public static ProbabilityData OurProbabilityData
        {
            get
            {
                if (_ourProbabilityData == null)
                {
                    _ourProbabilityData = Resources.Load<ProbabilityData>("ProbabilityMatrix");
                }

                return _ourProbabilityData;
            }
        }

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

        public float GetProbability(string key) => OurProbabilityData.CalculateProbability(this, key);
        
        // public float LineProbability()
        // {
        //     var confidence = -this.GetTraitNormalisedIntensity(PersonalityTrait.TraitType.Fearfulness);
        //     var negCuriosity = -this.GetTraitNormalisedIntensity(PersonalityTrait.TraitType.Curiosity);
        //     var trust = -this.GetTraitNormalisedIntensity(PersonalityTrait.TraitType.Trust);
        //     return 0.5f * (0.33f * NormalisedSigmoid(confidence) + 0.33f * NormalisedSigmoid(negCuriosity) +
        //                    0.33f * NormalisedSigmoid(trust));
        // }

        public float GetTraitIntensity(PersonalityTrait.TraitType traitType)
        {
            if (!_traitTypeToTrait.ContainsKey(traitType))
            {
                return 0;
            }
            return _traitTypeToTrait[traitType].Intensity;
        }

        public float GetNormalisedTraitIntensity(PersonalityTrait.TraitType traitType)
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
            _personToRelationship = new Dictionary<Personality, Relationship>();
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

            foreach (var relationship in relationships)
            {
                if (_personToRelationship.ContainsKey(relationship.OtherPerson))
                {
                    throw new Exception("Multiple same-type personality relations detected");
                }
                else
                {
                    _personToRelationship.Add(relationship.OtherPerson, relationship);
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

        public float ConversationJoiningProbability(Conversation conversation)
        {
            var conversationRating = 0f;
            foreach (var agent in conversation.Interlocutors)
            {
                var relationship = GetRelation(agent);
                
                if (IsSubordinate(agent.Personality) || this.IsManagedBy(agent))
                {
                    var responsibility = -GetNormalisedTraitIntensity(
                        PersonalityTrait.TraitType.Negligence);
                    var confidence = -GetNormalisedTraitIntensity(
                        PersonalityTrait.TraitType.Fearfulness);
                    relationship = ProbabilityUtility.NormalisedSigmoid((responsibility + confidence + relationship) / 3f);
                }
                conversationRating += relationship;
            }

            conversationRating /= conversation.Interlocutors.Count;

            return OurProbabilityData.CalculateProbability(this, "ConversationJoin") + 0.7f * ProbabilityUtility.NormalisedSigmoid(conversationRating);
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

        public bool IsSubordinate(Personality other)
        {
            if (!_personToRelationship.ContainsKey(other)) return false;

            return _personToRelationship[other].IsSubordinate;
        }

        public bool IsManagedBy(BaseAgent other) => IsManagedBy(other.Personality);

        public string NoteText()
        {
            return "Password: " + KnownPassword;
        }

        public float ChatProbability(Personality secondPersonality)
        {
            var conversationRating = 0f;
            var relationship = GetRelation(secondPersonality);
                
            if (IsSubordinate(secondPersonality) || this.IsManagedBy(secondPersonality))
            {
                var responsibility = -GetNormalisedTraitIntensity(
                    PersonalityTrait.TraitType.Negligence);
                var confidence = -GetNormalisedTraitIntensity(
                    PersonalityTrait.TraitType.Fearfulness);
                relationship = ProbabilityUtility.NormalisedSigmoid((responsibility + confidence + relationship) / 3f);
            }
            conversationRating += relationship;

            return 0.3f * OurProbabilityData.CalculateProbability(this, "Chat") + 0.7f * ProbabilityUtility.NormalisedSigmoid(conversationRating);
        }

        private bool IsManagedBy(Personality secondPersonality)
        {
            if (!secondPersonality._personToRelationship.ContainsKey(this)) return false;
            return secondPersonality._personToRelationship[this].IsSubordinate;
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

        public Personality OtherPerson => subject;
        public bool IsSubordinate => isSubordinate;

        public float NormalizedIntensity => intensity / 100f;
    }
}
