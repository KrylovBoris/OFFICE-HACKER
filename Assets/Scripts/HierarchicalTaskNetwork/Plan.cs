using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HierarchicalTaskNetwork
{

    #region Exceptions

    public class NoneTaskStatusException : Exception
    {
        public override string Message
        {
            get { return "Plan has encountered a task without status"; }
        }
    }

    #endregion

    public class Plan
    {
        private string planName;
        private HtnTask _rootTask;
        private HtnTask _currentHtnTask;
        private Queue<HtnTask> nextTask;


        public enum PlanStatus
        {
            InProgress,
            Complete,
            Failure
        }

        private PlanStatus _planStatus;

        public Plan(HtnTask root, string planName)
        {
            this.planName = planName;
            var tasks = root.DecomposeTask();
            _rootTask = root;
            nextTask = new Queue<HtnTask>(tasks);
            _currentHtnTask = nextTask.Dequeue();
            _planStatus = PlanStatus.InProgress;
        }

        public PlanStatus Status => _planStatus;



        public void PlanIterate()
        {
            switch (_currentHtnTask.Status)
            {
                case HtnTask.TaskStatus.Planned:
                    _currentHtnTask.StartExecution();
                    break;
                case HtnTask.TaskStatus.InProgress:
                    break;
                case HtnTask.TaskStatus.Complete:
                    if (nextTask.Count > 0)
                    {
                        _currentHtnTask = nextTask.Dequeue();
                    }
                    else
                    {
                        _planStatus = PlanStatus.Complete;
                    }

                    break;
                case HtnTask.TaskStatus.Failure:
                    _planStatus = PlanStatus.Failure;
                    break;
                case HtnTask.TaskStatus.None:
                    throw new NoneTaskStatusException();
            }
        }

    }
}