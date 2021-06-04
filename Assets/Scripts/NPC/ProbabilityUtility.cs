using System;
using UnityEngine;

namespace NPC
{
    public static class ProbabilityUtility
    {

        public static float NormalisedSigmoid(float val)
        {
            var f = Mathf.Exp(val/0.2f);
            return 1 / (1 + f);
        }
    }
}