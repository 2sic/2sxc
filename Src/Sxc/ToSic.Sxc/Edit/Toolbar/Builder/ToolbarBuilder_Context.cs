using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Edit.Toolbar;

partial class ToolbarBuilder: IToolbarBuilderInternal
{
    private const int NoAppId = -1;

    public IToolbarBuilder Context(
        object target
    )
    {
        // Get context, specify "true" to force it to be added
        var context = GenerateContext(target, true.ToString());
        var rule = new ToolbarRuleContext(null, context, Services.ToolbarButtonHelper.Value);
        return this.AddInternal(rule);
    }

    /// <summary>
    /// See if toolbar itself has a context, or there are rules which have a context,
    /// or any of the rules have one
    /// </summary>
    /// <returns></returns>
    public ToolbarContext GetContext()
        => Rules.OfType<ToolbarRuleContext>().FirstOrDefault()?.Context
           ?? Rules.FirstOrDefault(r => r.Context != null)?.Context;


    private ToolbarContext GenerateContext(object target, string context) => Log.Func($"{nameof(context)}:{context}", () =>
    {
        // Check if context had already been prepared
        if (context.ContainsInsensitive("context:")) return (new(context), "contains context:");

        if (target == null) return (null, "no target");
        if (context.EqualsInsensitive(false.ToString())) return (null, "context=false");
        var appStates = Services.AppStatesLazy.Value;
        if (appStates == null) return (null, "no AppStates");

        // Try to find the context
        var appId = FindContextAppId(target);

        // If nothing found
        if (appId == 0
            || appId == NoAppId
            || appId == Eav.Constants.TransientAppId
            || appId < 1
           ) return (null, "no app identified");

        var identity = appStates.IdentityOfApp(appId);
        if (identity == null) return (null, "app not found");

        // If we're not forcing the context "true" then check cases where it's not needed
        if (!context.EqualsInsensitive(true.ToString()))
            // If we're still on the same app, and we didn't force the context, return null
            if (_currentAppIdentity != null && _currentAppIdentity.AppId == identity.AppId)
            {
                // ensure we're not in a global context where the current-context is already special
                var globalAppId = appStates.GetPrimaryAppOfAppId(appId, Log);
                if (globalAppId != identity.AppId)
                    return (null, $"same app and not Global, context not forced: {identity.Show()}");
            }

        var result = new ToolbarContext(identity);
        return (result, "ok");
    });

    private int FindContextAppId(object target)
    {
        var l = Log.Fn<int>();
        if (target is IEntity entity)
            return l.Return(entity.AppId, "entity-appid");

        //if (target is IDynamicEntity dynEntity)
        //    return (dynEntity.Entity?.AppId ?? NoAppId, "dyn entity");
        if (target is ICanBeEntity canBeEntity)
            return l.Return(canBeEntity.Entity?.AppId ?? NoAppId, "dyn/typed entity");

        if (target is IHasMetadata md)
        {
            if (md.Metadata.Any())
                return l.Return(md.Metadata.FirstOrDefault()?.AppId ?? NoAppId, "metadata");
            if (md.Metadata is IMetadataInternals mdInternal)
                return l.Return(mdInternal.Context("todo")?.AppId ?? NoAppId, "metadata internal");
        }

        return l.Return(NoAppId, "no app id");
    }
}