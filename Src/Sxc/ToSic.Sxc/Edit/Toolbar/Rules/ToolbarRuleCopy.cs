using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;
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

        protected override string DecoratorTypeName => Entity?.Type?.Name;

        public override string GeneratedCommandParams() 
            => UrlParts.ConnectParameters(CopyCommandParams(), base.GeneratedCommandParams());

        private IEntity Entity => _entity.Get(() => Target as IEntity ?? (Target as IDynamicEntity)?.Entity);
        private readonly ValueGetOnce<IEntity> _entity = new ValueGetOnce<IEntity>();

        private string CopyCommandParams()
        {
            var entity = Entity;
            if (entity == null) return null;
            var typeName = entity.Type?.Name ?? "error-no-type-found";
            var newRule = $"entityId={entity.EntityId}&contentType={typeName}";
            return newRule;
        }
    }
}
