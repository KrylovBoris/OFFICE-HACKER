using System.Collections;
using System.Threading.Tasks;

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

        private bool CheckEndTask()
        {
            return BasicCheck(_endingConditions);
        }

        internal override async Task<TaskStatus> Execution()
        {
            if (!CheckPreConditions())
            {
                return TaskStatus.Failure;
            }

            _taskAction.Invoke();

            do
            {
                if (!CheckTaskIntegrity())
                {
                    return TaskStatus.Failure;
                }
                await Task.Yield();
            } while (!CheckEndTask());

            return TaskStatus.Complete;
        }
    }
}