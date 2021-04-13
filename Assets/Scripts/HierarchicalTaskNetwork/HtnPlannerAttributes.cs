using System;

namespace HierarchicalTaskNetwork
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class HtnPlannerConditionAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HtnPlannerActionAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HtnPlannerSimpleTaskAttribute : Attribute
    {
        public string TaskName { get; set; }
        public SimpleTask.Condition[] PreConditions { get; set; }
        public SimpleTask.Condition[] IntegrityRules { get; set; }
        public SimpleTask.Condition[] PostConditions { get; set; }

        public HtnPlannerSimpleTaskAttribute(string name,
            SimpleTask.Condition[] preConditions,
            SimpleTask.Condition[] integrityRules,
            SimpleTask.Condition[] postConditions)
        {
            TaskName = name;
            PreConditions = preConditions;
            IntegrityRules = integrityRules;
            PostConditions = postConditions;
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HtnRootTaskAttribute : Attribute
    {
        public HtnRootTaskAttribute(string name)
        {
            TaskName = name;
        }

        public string TaskName { get; }
    }
}
