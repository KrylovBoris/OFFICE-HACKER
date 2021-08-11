// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/Hierarchical-Task-Network-planner-for-Unity/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Lifetimes;
using UnityEngine;

namespace HierarchicalTaskNetwork
{
    public class ComplexTask: HtnTask
    {
        private HtnTask _currentHtnTask;
        private Queue<HtnTask> _taskExecutionPlan;
        public delegate HtnTask[] DecompositionMethod();
        
        
        public override TaskType Type => TaskType.Complex;

        private readonly DecompositionMethod _decompose;
        
        public ComplexTask(string name,
            DecompositionMethod decompose,
            Condition[] conditions = null, 
            Condition[] rules = null) : base(name, conditions, rules)
        {
            _decompose = decompose;
        }

        public override HtnTask[] DecomposeTask()
        {
            return _decompose.Invoke();
        }

        internal override async Task<TaskStatus> Execution(Lifetime lifetime)
        {
            Status = TaskStatus.InProgress;

            if (!CheckPreConditions(lifetime))
            {
                return TaskStatus.Failure;
            }

            _taskExecutionPlan = new Queue<HtnTask>(DecomposeTask());

            var planStatus = await ExecutePlan(lifetime.CreateNested().Lifetime);
            
            return planStatus;
        }

        private async Task<TaskStatus> ExecutePlan(Lifetime lifetime)
        {
            Debug.Log($"{Name} started");
            while (_taskExecutionPlan.Any())
            {
                if (!CheckTaskIntegrity(lifetime))
                {
                    return TaskStatus.Failure;
                }
                
                _currentHtnTask = _taskExecutionPlan.Dequeue();
                try
                {
                    var status = await _currentHtnTask.Execution(lifetime);
                    if (status == TaskStatus.Failure)
                    {
                        return TaskStatus.Failure;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{Name} has failed!");
                    throw;
                }
            }
            return TaskStatus.Complete;
        }
        
    }
}
