

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        internal override async Task<TaskStatus> Execution()
        {
            Status = TaskStatus.InProgress;
            if (!CheckPreConditions())
            {
                return TaskStatus.Failure;
            }

            _taskExecutionPlan = new Queue<HtnTask>(DecomposeTask());

            var planStatus = await ExecutePlan();
            
            return planStatus;
        }

        private async Task<TaskStatus> ExecutePlan()
        {
            while (_taskExecutionPlan.Any())
            {
                if (!CheckTaskIntegrity())
                {
                    return TaskStatus.Failure;
                }
                
                _currentHtnTask = _taskExecutionPlan.Dequeue();
                
                var status = await _currentHtnTask.Execution();
                
                if (status == TaskStatus.Failure)
                {
                    return TaskStatus.Failure;
                }
            }
            return TaskStatus.Complete;
        }
        
    }
}
