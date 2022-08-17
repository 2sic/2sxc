using System;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarBuilderConfiguration
    {
        internal ToolbarBuilderConfiguration(
            ToolbarBuilderConfiguration original,
            string mode = null,
            bool? condition = null,
            Func<bool> conditionFunc = null,
            bool? force = null,
            string group = null
        )
        {
            Mode = mode ?? original?.Mode;
            Condition = condition ?? original?.Condition;
            ConditionFunc = conditionFunc ?? original?.ConditionFunc;
            Force = force ?? original?.Force;
            Group = group ?? original?.Group;
        }

        public readonly string Mode = null;

        public readonly bool? Condition = null;

        public readonly Func<bool> ConditionFunc = null;

        public readonly bool? Force = null;

        public readonly string Group = null;

        // 2022-08-17 2dm - was an idea, but won't work in current infrastructure,
        // because the object doesn't always exist when this code is needed
        //public ToolbarBuilderConfiguration GetUpdated(
        //    string mode = null,
        //    bool? condition = null,
        //    Func<bool> conditionFunc = null,
        //    bool? force = null,
        //    string group = null
        //)
        //{
        //    return null == (mode ?? condition ?? conditionFunc ?? force as object ?? group) 
        //        ? this 
        //        : new ToolbarBuilderConfiguration(this, mode, condition, conditionFunc, force, group);
        //}
    }
}
