using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;
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

        protected readonly object Target;
        private readonly ToolbarButtonDecoratorHelper _helper;

        public override string GeneratedUiParams()
            => UrlParts.ConnectParameters(UiParams(), base.GeneratedUiParams());

        protected IEntity TargetEntity => _entity.Get(() => Target as IEntity ?? (Target as IDynamicEntity)?.Entity);
        private readonly ValueGetOnce<IEntity> _entity = new ValueGetOnce<IEntity>();

        protected string TargetParamId() => TargetEntity?.EntityId == null ? null : $"entityId={TargetEntity?.EntityId}";

        protected string TargetParamType() => $"contentType={TargetEntity?.Type?.Name ?? "error-no-type-found"}";


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
