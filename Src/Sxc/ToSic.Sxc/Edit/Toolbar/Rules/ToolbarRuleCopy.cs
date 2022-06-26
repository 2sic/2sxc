using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleCopy: ToolbarRuleTargeted
    {
        internal const string CommandName = "copy";

        internal ToolbarRuleCopy(
            object target, 
            string ui = null, 
            string parameters = null,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null
        ) : base(target, CommandName, operation: '+', ui: ui, parameters: parameters, context: context, helper: helper)
        {
        }

        protected override string DecoratorTypeName => TargetEntity?.Type?.Name;

        public override string GeneratedCommandParams() 
            => UrlParts.ConnectParameters(CopyCommandParams(), base.GeneratedCommandParams());

        //protected IEntity TargetEntity => _entity.Get(() => Target as IEntity ?? (Target as IDynamicEntity)?.Entity);
        //private readonly ValueGetOnce<IEntity> _entity = new ValueGetOnce<IEntity>();

        //protected string TargetParamId() => TargetEntity?.EntityId == null ? null : $"entityId={TargetEntity?.EntityId}";

        //protected string TargetParamType() => $"contentType={TargetEntity?.Type?.Name ?? "error-no-type-found"}";

        private string CopyCommandParams()
        {
            return TargetEntity == null 
                ? null 
                : string.Join("&", new[] { TargetParamId(), TargetParamType() }.Where(v => v.HasValue()));
        }
    }
}
