using UnityEngine;

namespace NPC
{
    [System.Serializable]
    public class PersonalityTrait
    {
        public enum TraitType: int
        {
            Curiosity = 1,
            Negligence, //Responsibility
            TechnicalKnowledge, 
            WillingnessToHelp,
            Fearfulness, //Confidence
            Greed, //Generosity
            Trust //Distrust
        }

        public TraitType type;
        [SerializeField]
        [Range(-100f, 100f)]
        private float traitIntensity;

        public float Intensity => traitIntensity;

        public float NormalisedIntensity => Intensity / 100;
        
        public int GetTraitMask()
        {
            return 1 << (int)type;
        }

        public static int operator &(PersonalityTrait trait1, PersonalityTrait trait2)
        {
            return trait1.GetTraitMask() & trait2.GetTraitMask();
        }
        
        public static int operator |(PersonalityTrait trait1, PersonalityTrait trait2)
        {
            return trait1.GetTraitMask() | trait2.GetTraitMask();
        }

        public static int GetTraitBitMask(params TraitType[] traitList)
        {
            var result = 0;
            foreach (var trait in traitList)
            {
                result = result | (1 << (int)trait);
            }
            return result;
        }

        public static int GetTraitBitMask(params PersonalityTrait[] traitList)
        {
            var result = 0;
            foreach (var trait in traitList)
            {
                result = result | (1 << (int)trait.type);
            }
            return result;
        }
        
        
    }
}