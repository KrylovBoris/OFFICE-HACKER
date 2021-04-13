using System;
using System.Collections.Generic;

namespace KnowledgeContainer
{
    public class State : IEquatable<State>
    {
        private readonly ulong[] _stateHolder;
        private readonly Environment _hostingEnvironment;

        public static State operator&(State state1, State state2)
        {
            if (state1._hostingEnvironment != state2._hostingEnvironment)
            {
                //TODO: throw DifferentEnvironments;
            }
            var length = state1._stateHolder.Length;
            var resHolder = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                resHolder[i] = state1._stateHolder[i] & state2._stateHolder[i];
            }
            return new State(state1._hostingEnvironment, resHolder);
        }
        
        public static State operator|(State state1, State state2)
        {
            if (state1._hostingEnvironment != state2._hostingEnvironment)
            {
                //TODO: throw DifferentEnvironments;
            }

            var length = state1._stateHolder.Length;
            var resHolder = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                resHolder[i] = state1._stateHolder[i] | state2._stateHolder[i];
            }
            return new State(state1._hostingEnvironment, resHolder);
        }
        
        public static bool operator!= (State state1, State state2)
        {
            if ((object)state1 == null && (object)state2 == null)
            {
                return false;
            }
            
            if ((object)state1 == null || (object)state2 == null)
            {
                return true;
            }

            if (state1._hostingEnvironment != state2._hostingEnvironment)
            {
                return true;
            }
            
            for (int i = 0; i < state1._stateHolder.Length; i++)
            {
                if (state1._stateHolder[i] != state2._stateHolder[i])
                {
                    return true;
                }
            }

            return false;
        }

        public static bool operator==(State state1, State state2)
        {
            return !(state1 != state2);
        }

        public bool Equals(State state)
        {
            return this == state;
        }

        public static State Copy(State original)
        {
            ulong[] newHolder = new ulong[original._stateHolder.Length];
            original._stateHolder.CopyTo(newHolder, 0);
            return new State(original._hostingEnvironment, newHolder);
        }
        
        //public State(Environment environment)
        //{
        //    _hostingEnvironment = environment;
        //    var holderDim = (uint) Math.Ceiling(_hostingEnvironment.EnvironmentDimension / 64.0);
        //    _stateHolder = new ulong[holderDim];
        //}

        public bool IsNull()
        {
            foreach (var statePart in _stateHolder)
            {
                if (statePart != 0) return false;
            }

            return true;
        }

        public bool StateApplicable(State appliedState)
        {
            return (this & appliedState) == appliedState;
        }

        public List<State> WeakerStatesList()
        {
            Queue<State> statesToWeaken = new Queue<State>();
            List<State> result = new List<State>();
            State processingState;
            statesToWeaken.Enqueue(this);
            while (statesToWeaken.Count > 0)
            {
                processingState = statesToWeaken.Dequeue();
                for (int i = 0; i < processingState._stateHolder.Length; i++)
                {
                    ulong holder = processingState._stateHolder[i];
                    ulong mask = 1;
                    ulong deltaHolder;
                    while (mask != 0 && holder != 0)
                    {
                        deltaHolder = holder & ~mask;
                        if (deltaHolder != holder)
                        {
                            ulong[] newHolder = new ulong[processingState._stateHolder.Length];
                            processingState._stateHolder.CopyTo(newHolder, 0);
                            newHolder[i] = deltaHolder;
                            var state = new State(processingState._hostingEnvironment, newHolder);
                            if (!state.IsNull() && !result.Contains(state))
                            {
                                statesToWeaken.Enqueue(state);
                                result.Add(state);
                            }
                        } 
                        mask <<= 1;
                    }
                }
            }

            return result;
        }
        
        public State(Environment environment, ulong[] holder)
        {
            _hostingEnvironment = environment;
            _stateHolder = holder;
        }
               
    }
    
    
}