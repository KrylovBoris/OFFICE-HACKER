using System;
using System.Collections;
using UnityEngine;

namespace GlobalMechanics
{
    public static class CoroutineUtils
    {
        public static IEnumerator DelayedAction(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke();
        }

        public static IEnumerator ConditionedAction(Func<bool> condition, Action action)
        {
            while (!condition.Invoke())
            {
                yield return null;
            }
            action.Invoke();
        }
    }
}