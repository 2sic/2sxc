using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    /// <summary>
    /// A toolbar rule for a specific target
    /// </summary>
    public abstract class ToolbarRuleTargeted: ToolbarRule
    {
        protected ToolbarRuleTargeted(
            object target, 
            string command, 
            string ui = null, 
            string parameters = null, 
            char? operation = null,
            string context = null
        ) : base(command, ui, parameters, operation)
        {
            Target = target;
            Context = context;
        }

        protected readonly object Target;
        protected readonly string Context;

        public override string GeneratedCommandParams() 
            => UrlParts.ConnectParameters(GetContext(), base.GeneratedCommandParams());


        protected string GetContext()
        {
            return Context;
        }
    }
}
