using System;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Apps;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppFolder: ServiceBase
{
    public AppFolder(IContextResolver ctxResolver) : base("AppFolder") 
        => _ctxResolver = ctxResolver;
    private readonly IContextResolver _ctxResolver;

    /// <summary>
    /// This is necessary for special calls where the _ctxResolve may not yet be complete...
    /// Important: not sure if this is actually needed, I believe the ctxResolver is always initialized on all web-api requests...?
    /// </summary>
    /// <param name="getBlock"></param>
    /// <returns></returns>
    public AppFolder Init(BlockWithContextProvider getBlock)
    {
        _ctxResolver.AttachBlock(getBlock);
        return this;
    }

    public string GetAppFolder()
    {
        var ctx = _ctxResolver.AppNameRouteBlock("");
        return ctx.AppState.Folder;
    }
}