using System;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarBuilderConfiguration
    {
        internal ToolbarBuilderConfiguration(ToolbarBuilderConfiguration original)
        {
            if (original == null) return;
            
            Mode = original.Mode;
            //Target = original.Target;
            Condition = original.Condition;
            ConditionFunc = original.ConditionFunc;
            Force = original.Force;
        }

        public string Mode = null;

        //public object Target = null;

        public bool? Condition = null;

        public Func<bool> ConditionFunc = null;

        public bool? Force = null;
    }
}
