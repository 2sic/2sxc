using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Configuration.Internal;

namespace ToSic.Sxc.Context.Internal;

partial class SxcContextResolver
{
    public IContextOfApp GetBlockOrSetApp(int appId)
    {
        // get the current block context
        var moduleCtx = BlockContextOrNull();

        // If there is no block context, then just use a "blank" app context
        // This way security checks can only verify the App but not any module permissions
        if (moduleCtx == null)
            return SetApp(SiteAppIdentity());

        // if there is a block context, make sure it's of the requested app (or no app was specified)
        // then return that
        // note: an edge case is that a block context exists, but no app was selected - then AppState is null


        // If the app in the request matches the app in the context, everything is fine
        if (appId == moduleCtx.AppReader?.AppId) return moduleCtx;
        
        // If the app in the request doesn't match the app in the context
        // Set the app in the context to the real one to be sure about security check
        // We still want to preserve the ModuleContext, so security checks can
        // still verify module permissions.
        // Only do this if the feature is enabled, as we are opening security when we do this
        if (!featuresService.Value.IsEnabled(SxcFeatures.PermissionPrioritizeModuleContext.NameId))
            return SetApp(SiteAppIdentity());

        moduleCtx.ResetApp(SiteAppIdentity());
        return moduleCtx;

        AppIdentity SiteAppIdentity() => new(Site().Site.ZoneId, appId);
    }


    public IContextOfApp SetAppOrGetBlock(string nameOrPath) => SetAppOrNull(nameOrPath) ?? BlockContextRequired();


    public IContextOfApp AppNameRouteBlock(string nameOrPath)
    {
        var ctx = SetAppOrNull(nameOrPath);
        if (ctx != null) return ctx;

        var identity = appIdResolverLazy.Value.GetAppIdFromRoute();
        if (identity != null)
            return SetApp(identity);

        ctx = BlockContextOrNull();
        return ctx ?? throw new($"Tried to auto detect app by name '{nameOrPath}', url params or block context, all failed.");
    }

}