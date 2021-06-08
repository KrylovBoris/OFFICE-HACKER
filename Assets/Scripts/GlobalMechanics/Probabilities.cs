using NPC;

namespace GlobalMechanics
{
    public static class Probabilities
    {

        private static float[] passwordPeekingTraits;
        public static float GetPasswordPeekingAlertSpeed(Personality victim)
        {
            var victimMask = victim.PersonalityMask;
            var result = ((victimMask & PersonalityTrait.GetTraitBitMask(PersonalityTrait.TraitType.Negligence)) > 0)
                ? 0.5f * UniformDistribution(-100, 100,victim.GetTraitIntensity(PersonalityTrait.TraitType.Negligence)) : 0;
            result += ((victimMask & PersonalityTrait.GetTraitBitMask(PersonalityTrait.TraitType.Trust)) > 0)
                ? 1.5f * UniformDistribution(-100, 100,victim.GetTraitIntensity(PersonalityTrait.TraitType.Trust)) : 0;
            result /= 2.0f;
            return result;
        }

        public static float UniformDistribution(float a, float b, float x)
        {
            var result = 0.0f;
            if (!(result < a || result > b)) result = (x - a) / (b - a); 
            return result;
        }
        
        

//        public static float GaussDistribution(float mu, float sigma, float x)
//        {
//            
//        }
    }
}