using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Catalog;
using ToSic.Eav.Metadata;
using ToSic.Eav.Metadata.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial record ToolbarBuilder : IToolbarBuilderInternal
{
    private const int NoAppId = -1;

    public IToolbarBuilder Context(
        object target
    )
    {
        // Get context, specify "true" to force it to be added
        var context = GenerateContext(target, true.ToString());
        var rule = new ToolbarRuleContext(null, context, Services.ToolbarButtonHelper.Value);
        return this.AddInternal([rule]);
    }

    /// <summary>
    /// See if toolbar itself has a context, or there are rules which have a context,
    /// or any of the rules have one
    /// </summary>
    /// <returns></returns>
    ToolbarContext IToolbarBuilderInternal.GetContext()
        => Rules.OfType<ToolbarRuleContext>().FirstOrDefault()?.Context
           ?? Rules.FirstOrDefault(r => r.Context != null)?.Context;


    private ToolbarContext GenerateContext(object target, string context)
    {
        var l = Log.Fn<ToolbarContext>($"{nameof(context)}:{context}");
        // Check if context had already been prepared
        if (context.ContainsInsensitive("context:"))
            return l.Return(new(context), "contains context:");

        if (target == null)
            return l.ReturnNull("no target");
        if (context.EqualsInsensitive(false.ToString()))
            return l.ReturnNull("context=false");
        var appsCatalog = Services.AppsCatalog.Value;
        if (appsCatalog == null)
            return l.ReturnNull("no AppStates");

        // Try to find the context
        var appId = FindContextAppId(target);

        // If nothing found
        if (appId is 0 or NoAppId or KnownAppsConstants.TransientAppId or < 1)
            return l.ReturnNull("no app identified");

        var identity = appsCatalog.AppIdentity(appId);
        if (identity == null) return l.ReturnNull("app not found");

        // If we're not forcing the context "true" then check cases where it's not needed
        if (!context.EqualsInsensitive(true.ToString()))
            // If we're still on the same app, and we didn't force the context, return null
            if (CurrentAppIdentity != null && CurrentAppIdentity.AppId == identity.AppId)
            {
                // ensure we're not in a global context where the current-context is already special
                var globalAppId = appsCatalog.GetPrimaryAppOfAppId(appId, Log);
                if (globalAppId != identity.AppId)
                    return l.ReturnNull($"same app and not Global, context not forced: {identity.Show()}");
            }

        var result = new ToolbarContext(identity);
        return l.Return(result, "ok");
    }

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