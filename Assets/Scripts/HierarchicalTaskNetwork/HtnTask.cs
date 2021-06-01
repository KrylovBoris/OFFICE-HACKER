using System;
using System.Collections;
using System.Threading.Tasks;
using JetBrains.Lifetimes;
using UnityEngine;

namespace HierarchicalTaskNetwork
{
    public abstract class HtnTask
    {
        private string taskName;

        private TaskStatus taskStatus;

        public static readonly Condition[] EmptyCondition =
        {
            () => true
        };


        public delegate bool Condition();

        private readonly Condition[] _preConditionsList;
        private readonly Condition[] _integrityRules;

        protected HtnTask(string name,
            Condition[] conditions = null,
            Condition[] rules = null)
        {
            taskName = name;
            if (conditions == null)
            {
                _preConditionsList = EmptyCondition;
            }
            else
            {
                _preConditionsList = new Condition[conditions.Length];
                conditions.CopyTo(_preConditionsList, 0);
            }

            if (rules == null)
            {
                _integrityRules = EmptyCondition;
            }
            else
            {
                _integrityRules = new Condition[rules.Length];
                rules.CopyTo(_integrityRules, 0);
            }

            taskStatus = TaskStatus.Planned;
        }

        public enum TaskType
        {
            Empty,
            Simple,
            Complex
        }

        public enum TaskStatus
        {
            None,
            Planned,
            InProgress,
            Complete,
            Failure
        }

        public string Name => taskName;

        public virtual TaskType Type => TaskType.Empty;

        public TaskStatus Status
        {
            get => taskStatus;
            set => taskStatus = value;
        }


        #region Checks

        protected bool BasicCheck(Lifetime lifetime, Condition[] collection)
        {
            if (lifetime.IsNotAlive)
            {
                return false;
            }
            
            var result = true;
            try
            {
                foreach (var condition in collection)
                {
                    UnityEngine.Debug.Assert(condition != null);
                    result &= condition.Invoke();
                    if (!result) return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }

            return true;
        }


        protected bool CheckPreConditions(Lifetime lifetime)
        {
            return BasicCheck(lifetime, _preConditionsList);
        }

        protected bool CheckTaskIntegrity(Lifetime lifetime)
        {
            return BasicCheck(lifetime, _integrityRules);
        }

        #endregion


        internal virtual async Task<TaskStatus> Execution(Lifetime lifetime)
        {
            return TaskStatus.Complete;
        }

        public async void StartExecution(Lifetime lf)
        {
            UnityEngine.Debug.Log($"Starting task {Name}");
            Status = await Execution(lf);
            UnityEngine.Debug.Log($"Task {Name} finished");
        }

        public virtual HtnTask[] DecomposeTask()
        {
            return null;
        }

    }
}