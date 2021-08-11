// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using System.Collections.Generic;

namespace KnowledgeSystem
{
    public class ActionFactory
    {
        private Environment _hostingEnvironment;

        public ActionFactory(Environment env)
        {
            _hostingEnvironment = env;
        }

        
        public Transaction ProduceTransaction(string action, List<(string, bool)> essentialConditionFacts, 
                                                            List<(string, bool)> negligibleConditionFacts, 
                                                            List<(string, bool)> effectFacts)
        {
            var essential = new State(_hostingEnvironment, _hostingEnvironment.GetStateHolder(essentialConditionFacts.ToArray()));
            var negligible = new State(_hostingEnvironment, _hostingEnvironment.GetStateHolder(negligibleConditionFacts.ToArray()));
            var effect = essential | new State(_hostingEnvironment, _hostingEnvironment.GetStateHolder(effectFacts.ToArray()));
            return new Transaction(essential, negligible, effect, action);
        }

    }

    public class Transaction
    {
        readonly public State[] Preconditions;
        readonly public State Effect;
        readonly public string ActionToken;

        public Transaction(State essentialPreconditions, State negligiblePrecondition, State actionEffect, string action)
        {
            List<State> preconditionList = new List<State>{essentialPreconditions | negligiblePrecondition};
            var weakenedPrecond = negligiblePrecondition.WeakerStatesList();
            for (int i = 0; i < weakenedPrecond.Count; i++)
            {
                preconditionList.Add(weakenedPrecond[i]);
            }

            if (!negligiblePrecondition.IsNull())
            {
                preconditionList.Add(essentialPreconditions);
            }

            Effect = actionEffect;
            ActionToken = action;
            Preconditions = preconditionList.ToArray();
        }
    }
}