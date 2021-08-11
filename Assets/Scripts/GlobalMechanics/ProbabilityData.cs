// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using NPC;
using UnityEngine;

namespace GlobalMechanics
{
    [CreateAssetMenu(fileName = "ProbabilityMatrix", menuName = "ScriptableObjects/Probability Matrix", order = 100)]
    public class ProbabilityData : ScriptableObject
    {

        public List<ProbabilityRow> matrix = new List<ProbabilityRow>();

        private readonly Dictionary<string, ProbabilityRow> _dataDictionary = new Dictionary<string, ProbabilityRow>();

        public void Add()
        {
            matrix.Add(new ProbabilityRow("Key" + matrix.Count));
        }

        public void OnEnable()
        {
            if (!IsValid) return;
            foreach (var row in matrix)
            {
                _dataDictionary.Add(row.key, row);
            }
        }

        public bool IsValid
        {
            get
            {
                return matrix.Select(row => row.key).Distinct().Count() == matrix.Count;
            }
        }

        [Serializable]
        public class ProbabilityRow
        {
            [SerializeField]
            public string key;
            
            [SerializeField]
            public float Curiosity;
            
            [SerializeField]
            public float Negligence; 
            
            [SerializeField]
            public float TechnicalKnowledge;
            
            [SerializeField]
            public float WillingnessToHelp;
            
            [SerializeField]
            public float Fearfulness; 
            
            [SerializeField]
            public float Greed;

            [SerializeField]
            public float Trust;

            public ProbabilityRow(string keyName)
            {
                key = keyName;
            }

            public float CalculateProbability(Personality personality)
            {
                return Curiosity * ProbabilityUtility.NormalisedSigmoid(personality.GetNormalisedTraitIntensity(PersonalityTrait.TraitType.Curiosity)) +
                       Negligence * ProbabilityUtility.NormalisedSigmoid(personality.GetNormalisedTraitIntensity(PersonalityTrait.TraitType.Negligence)) +
                       TechnicalKnowledge * ProbabilityUtility.NormalisedSigmoid(personality.GetNormalisedTraitIntensity(PersonalityTrait.TraitType.TechnicalKnowledge)) +
                       WillingnessToHelp * ProbabilityUtility.NormalisedSigmoid(personality.GetNormalisedTraitIntensity(PersonalityTrait.TraitType.WillingnessToHelp)) +
                       Fearfulness * ProbabilityUtility.NormalisedSigmoid(personality.GetNormalisedTraitIntensity(PersonalityTrait.TraitType.Fearfulness)) +
                       Greed * ProbabilityUtility.NormalisedSigmoid(personality.GetNormalisedTraitIntensity(PersonalityTrait.TraitType.Greed)) +
                       Trust * ProbabilityUtility.NormalisedSigmoid(personality.GetNormalisedTraitIntensity(PersonalityTrait.TraitType.Trust));
            }
        }

        public float CalculateProbability(Personality personality, string line)
        {
            return _dataDictionary[line].CalculateProbability(personality);
        }
    }
    
    
}