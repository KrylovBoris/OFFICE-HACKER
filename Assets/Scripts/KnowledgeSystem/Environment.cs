// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using System;
using System.Collections.Generic;

namespace KnowledgeSystem
{
    public class Environment
    {
        private readonly List<string> _facts;
        private readonly List<string> _actions;
        public uint EnvironmentDimension => (uint) _facts.Count;
        
        public Environment(string[] keys, string[] actions)
        {
            _facts = new List<string>(keys);
            _facts.Sort();
            _actions = new List<string>(actions);
        }

        public ulong[] GetStateHolder(params (string, bool)[] stateFacts)
        {
            var size = (uint) Math.Ceiling(EnvironmentDimension / 64.0);
            var res = new ulong[size];
            foreach (var fact in stateFacts)
            {
                if (fact.Item2)
                {
                    var i = GetFactIndex(fact.Item1) / 64;
                    res[i] |= GetFactMask(fact.Item1);
                }
            }
            return res;
        }
        public State NullState()
        {
            var size = (uint) Math.Ceiling(EnvironmentDimension / 64.0);
            var nullHolder = new ulong[size];
            return new State(this, nullHolder);
        }
        
        /// <summary>
        /// Returns mask of the fact 
        /// </summary>
        /// <param name="fact"></param>
        /// <returns></returns>
        private ulong GetFactMask(string fact)
        {
            var shift = GetFactIndex(fact) % 64;
            ulong mask = 1;
            mask <<= shift;
            return mask;
        }

        private int GetFactIndex(string fact)
        {
            var index = _facts.BinarySearch(fact);
            return index;
        }
        
        
    }

}