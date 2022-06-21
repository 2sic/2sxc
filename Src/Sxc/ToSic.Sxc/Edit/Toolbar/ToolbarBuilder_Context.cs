using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private const int NoAppId = -1;

        public ToolbarContext Context()
        {
            // See if any rules have a context
            var rulesWithContext = Rules.Where(r => r.Context != null).ToArray();
            if (!rulesWithContext.Any()) return null;
            return rulesWithContext.FirstOrDefault()?.Context;
        }


        private ToolbarContext GetContext(object target, string context)
        {
            var callLog = Log.Fn<ToolbarContext>($"{nameof(context)}:{context}");
            // Check if context had already been prepared
            if (context.ContainsInsensitive("context:")) return callLog.ReturnAndLog(new ToolbarContext(context));

            if (target == null) return callLog.ReturnNull("no target");
            if (context.EqualsInsensitive(false.ToString())) return callLog.ReturnNull("context=false");
            var appStates = _deps.AppStatesLazy.Value;
            if (appStates == null) return callLog.ReturnNull("no AppStates");

            // Try to find the context
            var appId = FindContextAppId(target);

            // If nothing found
            if (appId == 0 || appId == NoAppId) return callLog.ReturnNull("no app identified");

            var identity = appStates.IdentityOfApp(appId);
            if (identity == null) return callLog.ReturnNull("app not found");

            // If we're not forcing the context "true" then check cases where it's not needed
            if (!context.EqualsInsensitive(true.ToString()))
                // If we're still on the same app, and we didn't force the context, return null
                if (_currentAppIdentity != null && _currentAppIdentity.AppId == identity.AppId)
                {
                    // ensure we're not in a global context where the current-context is already special
                    var globalId = appStates.GetPrimaryAppOfAppId(appId, Log);
                    if (globalId.AppId != identity.AppId)
                        return callLog.ReturnNull($"same app and not Global, context not forced: {identity.Show()}");
                }

            var result = new ToolbarContext(identity);
            return callLog.ReturnAndLog(result);
        }

        private int FindContextAppId(object target)
        {
            var callLog = Log.Fn<int>();
            if (target is IEntity entity) 
                return callLog.Return(entity.AppId, "entity-appid");
            if (target is IDynamicEntity dynEntity)
                return callLog.Return(dynEntity.Entity?.AppId ?? NoAppId, "dyn entity");
            if (target is IHasMetadata md)
            {
                if (md.Metadata.Any()) 
                    return callLog.Return(md.Metadata.FirstOrDefault()?.AppId ?? NoAppId, "metadata");
                if (md.Metadata is IMetadataInternals mdInternal)
                    return mdInternal.Context("todo")?.AppId ?? NoAppId;
            }
            return NoAppId;
        }
    }
}
