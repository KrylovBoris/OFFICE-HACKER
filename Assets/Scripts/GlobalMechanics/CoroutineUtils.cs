// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

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