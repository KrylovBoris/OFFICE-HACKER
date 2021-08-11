// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/Hierarchical-Task-Network-planner-for-Unity/blob/master/LICENSE

using System.Collections;
using System.Threading.Tasks;
using JetBrains.Lifetimes;
using UnityEngine;

namespace HierarchicalTaskNetwork
{
    public class SimpleTask : HtnTask
    {
        public delegate void TaskAction();

        private readonly TaskAction _taskAction;
        private readonly Condition[] _endingConditions;

        public override TaskType Type => TaskType.Simple;

        public SimpleTask(string name,
            TaskAction action = null,
            Condition[] conditions = null,
            Condition[] rules = null,
            Condition[] finish = null) : base(name, conditions, rules)
        {
            _taskAction = action;
            if (finish == null)
            {
                _endingConditions = EmptyCondition;
            }
            else
            {
                _endingConditions = new Condition[finish.Length];
                finish.CopyTo(_endingConditions, 0);
            }

        }

        private bool CheckEndTask(Lifetime lifetime)
        {
            return BasicCheck(lifetime, _endingConditions);
        }

        internal override async Task<TaskStatus> Execution(Lifetime lifetime)
        {
            Debug.Log($"{Name} started");
            Status = TaskStatus.InProgress;
            
            if (!CheckPreConditions(lifetime))
            {
                return TaskStatus.Failure;
            }

            if (lifetime.IsNotAlive)
            {
                return TaskStatus.Failure;
            }
            _taskAction.Invoke();

            do
            {
                if (!CheckTaskIntegrity(lifetime))
                {
                    return TaskStatus.Failure;
                }
                await Task.Yield();
            } while (!CheckEndTask(lifetime));

            return TaskStatus.Complete;
        }
    }
}