using ToSic.Eav.Apps;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;

namespace ToSic.Eav.Context.Internal;

/// <summary>
/// Base class for Context Resolver.
/// Split is because of old concept where this was also in the EAV project.
/// But as the system matured, it was only used in the setup of Blocks and Code, so not relevant for the EAV layers.
/// </summary>
/// <param name="siteCtxGenerator"></param>
/// <param name="appCtxGenerator"></param>
/// <param name="logName"></param>
/// <param name="connect"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContextResolverBase(
    Generator<IContextOfSite> siteCtxGenerator,
    Generator<IContextOfApp> appCtxGenerator,
    string logName = default,
    object[] connect = default)
    : ServiceBase(logName ?? "Eav.CtxRes", connect: [..connect ?? [], siteCtxGenerator, appCtxGenerator])
{
   
    public IContextOfSite Site() => _site.Get(siteCtxGenerator.New);
    private readonly GetOnce<IContextOfSite> _site = new();


    public IContextOfApp SetApp(IAppIdentity appIdentity)
    {
        var appCtx = appCtxGenerator.New();
        appCtx.ResetApp(appIdentity);
        AppContextFromAppOrBlock = appCtx;
        return appCtx;
    }

    public IContextOfApp AppRequired()
        => AppContextFromAppOrBlock ?? throw new($"To call {nameof(AppRequired)} first call {nameof(SetApp)}");

    public IContextOfApp AppOrNull() => AppContextFromAppOrBlock;

    /// <summary>
    /// This is set whenever an App Context or Block Context are set.
    /// </summary>
    protected IContextOfApp AppContextFromAppOrBlock { get; set; }
}