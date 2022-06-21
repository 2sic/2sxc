namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarBuilderParams
    {
        internal ToolbarBuilderParams(ToolbarBuilderParams original)
        {
            if (original == null) return;
            
            Mode = original.Mode;
            Target = original.Target;
        }

        public string Mode = null;

        public object Target = null;
    }
}
