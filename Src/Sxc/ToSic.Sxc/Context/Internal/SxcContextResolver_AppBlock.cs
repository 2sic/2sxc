using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Context.Internal;

partial class SxcContextResolver
{

    public IContextOfApp GetBlockOrSetApp(int appId)
    {
        // get the current block context
        var ctx = BlockContextOrNull();

        // if there is a block context, make sure it's of the requested app (or no app was specified)
        // then return that
        // note: an edge case is that a block context exists, but no app was selected - then AppState is null
        if (ctx != null && (appId == 0 || appId == ctx.AppState?.AppId)) return ctx;

        // if block was found but we're working on another app (like through app-admin)
        // then ignore block permissions / context and only return app
        return SetApp(new AppIdentity(Site().Site.ZoneId, appId));
    }


    public IContextOfApp SetAppOrGetBlock(string nameOrPath) => SetAppOrNull(nameOrPath) ?? BlockContextRequired();


    public IContextOfApp AppNameRouteBlock(string nameOrPath)
    {
        var ctx = SetAppOrNull(nameOrPath);
        if (ctx != null) return ctx;

        var identity = AppIdResolver.Value.GetAppIdFromRoute();
        if (identity != null)
            return SetApp(identity);

        ctx = BlockContextOrNull();
        return ctx ?? throw new($"Tried to auto detect app by name '{nameOrPath}', url params or block context, all failed.");
    }

}