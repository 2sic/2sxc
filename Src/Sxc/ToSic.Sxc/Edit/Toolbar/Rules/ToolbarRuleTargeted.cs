using ToSic.Eav.Plumbing;
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
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null
        ) : base(command, ui, parameters: parameters, operation: operation, context: context)
        {
            Target = target;
            _helper = helper;
        }

        internal object Target { get; set; }

        private readonly ToolbarButtonDecoratorHelper _helper;

        public override string GeneratedUiParams()
            => UrlParts.ConnectParameters(UiParams(), base.GeneratedUiParams());


        #region Decorators

        protected virtual string DecoratorTypeName => "";

        protected ToolbarButtonDecorator Decorator => _decorator.Get(() =>
        {
            var decoTypeName = DecoratorTypeName;
            return decoTypeName.HasValue() ? _helper?.GetDecorator(Context, decoTypeName ?? "", Command) : null;
        });
        private readonly ValueGetOnce<ToolbarButtonDecorator> _decorator = new ValueGetOnce<ToolbarButtonDecorator>();
        private string UiParams() => Decorator?.AllRules();

        #endregion
    }
}
