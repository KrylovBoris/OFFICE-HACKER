// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using System;
using System.Linq;
using JetBrains.Collections.Viewable;
using UnityEngine.Assertions;

namespace NPC
{
    public enum InterruptionFlag
    {
        None,
        Talk,
        PlayerPeeking,
        PlayerTookWorkplace,
        WrongPassword,
        PlayerTurnedOffTheComputer, 
    }
    public class InterruptionFlagsHandler
    {

        private ViewableMap<InterruptionFlag, bool> _flagToValue;

        public InterruptionFlagsHandler()
        {
            
            var flags = Enum.GetValues(typeof(InterruptionFlag)).Cast<InterruptionFlag>().
                ToDictionary(flagType => flagType, flagType => false);
            _flagToValue = new ViewableMap<InterruptionFlag, bool>(flags);
            Assert.IsTrue(NoFlagsRaised());
        }

        public void RaiseFlag(InterruptionFlag flag)
        {
            _flagToValue[flag] = true;
        }

        public void ResetFlags()
        {
            foreach (var key in _flagToValue.Keys)
            {
                _flagToValue[key] = false;
            }
        }

        
        public bool NoFlagsRaised()
        {
            var result = true;
            foreach (var value in _flagToValue.Values)
            {
                result &= !value;
            }

            return result;
        }
        
        public bool NoFlagsRaised(out InterruptionFlag f)
        {
            f = InterruptionFlag.None;
            var result = true;
            foreach (var key in _flagToValue.Keys)
            {
                result &= !_flagToValue[key];
                if (_flagToValue[key]) f = key;
            }
            return result;
        }
        
    }
}

