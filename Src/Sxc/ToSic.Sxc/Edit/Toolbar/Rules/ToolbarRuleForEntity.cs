using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleForEntity: ToolbarRuleTargeted
    {
        public const string PrefixPrefill = "prefill:";

        internal ToolbarRuleForEntity(
            string commandName,
            object target = null,   // IEntity, DynEntity or int
            char? operation = null,
            int? entityId = null,
            string contentType = null,
            string ui = null, 
            string parameters = null,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null
        ) : base(target, commandName, operation: operation, ui: ui, parameters: parameters, context: context, helper: helper)
        {
            if (target is int intTarget) EditInfo.entityId = intTarget;
            if (entityId != null) EditInfo.entityId = entityId;
            if (contentType != null) EditInfo.contentType = contentType;
        }

        #region Configuration of params to generate

        internal bool PropSerializeDefault = true;
        internal Dictionary<string, bool> PropSerializeMap = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);

        internal void PropSerializeSetAll(bool setAll)
        {
            PropSerializeDefault = setAll;
            PropSerializeMap = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion
        

        protected IEntity TargetEntity => _entity.Get(() => Target as IEntity ?? (Target as IDynamicEntity)?.Entity);
        private readonly ValueGetOnce<IEntity> _entity = new ValueGetOnce<IEntity>();

        internal EntityEditInfo EditInfo => _editInfo.Get(() => new EntityEditInfo(TargetEntity));
        private readonly ValueGetOnce<EntityEditInfo> _editInfo = new ValueGetOnce<EntityEditInfo>();

        protected override string DecoratorTypeName => TargetEntity?.Type?.Name;

        public override string GeneratedCommandParams()
            => UrlParts.ConnectParameters(EntityParamsList(), base.GeneratedCommandParams());

        protected string EntityParamsList()
        {
            var obj2Url = new ObjectToUrl(null, (name, value)
                => PropSerializeMap.TryGetValue(name, out var reallyUse2)
                    ? (reallyUse2, value)
                    : (PropSerializeDefault, value));

            return obj2Url.Serialize(EditInfo);
        }
    }
}
