using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleForEntity: ToolbarRuleTargeted
    {
        public const string PrefixPrefill = "prefill:";

        internal ToolbarRuleForEntity(
            object target,
            string commandName,
            char? operation = null,
            int? entityId = null,
            string contentType = null,
            string ui = null, 
            string parameters = null,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null
        ) : base(target, commandName, operation: operation, ui: ui, parameters: parameters, context: context, helper: helper)
        {
            EntityId = entityId ?? target as int?;
            ContentType = contentType;
        }

        protected int? EntityId { get; }
        protected string ContentType { get; }

        #region Configuration of params to generate

        internal bool ParamEntityIdUsed = false;
        internal bool ParamContentTypeUsed = false;

        #endregion
        

        protected IEntity TargetEntity => _entity.Get(() => Target as IEntity ?? (Target as IDynamicEntity)?.Entity);
        private readonly ValueGetOnce<IEntity> _entity = new ValueGetOnce<IEntity>();

        
        protected override string DecoratorTypeName => TargetEntity?.Type?.Name;

        public override string GeneratedCommandParams()
            => UrlParts.ConnectParameters(EntityCommandParams(), base.GeneratedCommandParams());

        private string EntityCommandParams()
            => UrlParts.ConnectParameters(EntityParamsList());

        protected string[] EntityParamsList()
        {
            var pars = new List<string>();
            if (ParamEntityIdUsed)
            {
                var eId = EntityId ?? TargetEntity?.EntityId;
                if (eId != null) pars.Add($"{KeyEntityId}={eId}");
            }
            if (ParamContentTypeUsed)
            {
                var type = ContentType ?? TargetEntity?.Type?.Name;
                if (type != null) pars.Add($"{KeyContentType}={type}");
            }
            return pars.Where(p => p.HasValue()).ToArray();
        }
    }
}
