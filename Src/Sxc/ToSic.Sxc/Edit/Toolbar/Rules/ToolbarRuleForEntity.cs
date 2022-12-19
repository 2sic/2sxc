using System;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helper;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleForEntity: ToolbarRuleTargeted
    {
        public const string PrefixPrefill = "prefill:";
        public const string PrefixFilters = "filter:";

        internal ToolbarRuleForEntity(
            string commandName,
            object target = null,   // IEntity, DynEntity or int
            char? operation = null,
            string contentType = null,
            string ui = null, 
            string parameters = null,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper decoHelper = null,
            string[] propsSkip = null,
            string[] propsKeep = null
        ) : base(target, commandName, operation: operation, ui: ui, parameters: parameters, context: context, decoHelper: decoHelper)
        {
            if (target is int intTarget) EditInfo.entityId = intTarget;
            if (contentType != null) EditInfo.contentType = contentType;

            if (propsSkip != null) _urlValueFilterNames = new UrlValueFilterNames(true, propsSkip);
            else if (propsKeep != null) _urlValueFilterNames = new UrlValueFilterNames(false, propsKeep);
        }

        /// <summary>
        /// The filter for what entity properties to keep in the params. By default, keep all.
        /// </summary>
        private readonly UrlValueFilterNames _urlValueFilterNames = new UrlValueFilterNames(true, Array.Empty<string>());


        protected IEntity TargetEntity => _entity.Get(() => Target as IEntity ?? (Target as IEntityWrapper)?.Entity);
        private readonly GetOnce<IEntity> _entity = new GetOnce<IEntity>();

        internal EntityEditInfo EditInfo => _editInfo.Get(() => new EntityEditInfo(TargetEntity));
        private readonly GetOnce<EntityEditInfo> _editInfo = new GetOnce<EntityEditInfo>();

        protected override string DecoratorTypeName => TargetEntity?.Type?.Name;

        public override string GeneratedCommandParams()
            => UrlParts.ConnectParameters(EntityParamsList(), base.GeneratedCommandParams());

        protected string EntityParamsList()
        {
            var obj2Url = new ObjectToUrl(null, new[] { _urlValueFilterNames });
            //var obj2Url = new ObjectToUrl(null, (n, v) => _urlValueFilter.FilterValues(n, v));
            return obj2Url.Serialize(EditInfo);
        }
    }
}
