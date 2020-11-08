using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Agent
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

        private Dictionary<InterruptionFlag, bool> _flagToValue;

        public InterruptionFlagsHandler()
        {
            _flagToValue = new Dictionary<InterruptionFlag, bool>();
            var flags = Enum.GetValues(typeof(InterruptionFlag)).Cast<InterruptionFlag>();
            foreach (var f in flags)
            {
                if (f != InterruptionFlag.None) _flagToValue.Add(f, false);
            }
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

