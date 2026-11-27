using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Render.Polymorphism.Sys;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;
using ToSic.Sys.Caching.PiggyBack;

// ReSharper disable InheritdocInvalidUsage

namespace ToSic.Sxc.Code.Context.Sys;

/// <summary>
/// Base class for any dynamic code root objects. <br/>
/// Root objects are the ones compiled by 2sxc - like the RazorComponent or ApiController. <br/>
/// If you create code for dynamic compilation, you'll always inherit from ToSic.Sxc.Dnn.DynamicCode.
/// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
/// </summary>
/// <remarks>
/// It can usually not be created directly, but through the <see cref="ICodeContextFactory"/>.
/// </remarks>
[PrivateApi("Was public till v17, and previously called DynamicCodeRoot")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class CodeContext : ServiceBase<CodeContext.Dependencies>,
    ICodeContext,
    IHasPiggyBack,
    //IWrapper<ICodeDataFactory>, probably not needed any more
    IServiceWithSetup<CodeContextOptions>
{
    #region Constructor

    /// <summary>
    /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
    /// </summary>
    [PrivateApi]
    public record Dependencies(
        ISysServiceProvider ServiceProvider,
        AppDataStackService DataStackService,
        LazySvc<CodeContextDataSourceService> DataSources,
        // 2026-11-23 2dm - WIP - removed, as we want to create it from the CreateServices and ensure it's connected
        //ICodeDataFactory Cdf,
        IEditionService EditionSvc)
        : DependenciesBase(connect:
            [/* never! serviceProvider */ DataStackService, DataSources, /*Cdf,*/ EditionSvc]);

    [PrivateApi]
    protected internal CodeContext(Dependencies services, string logPrefix) : base(services, logPrefix + ".DynCdR")
    {
        // Prepare services which need to be attached to this dynamic code root
        CmsContext = GetService<ICmsContext>();

        // Make sure we get and initialize (auto-connect) the app-level CSP if that exists or is enabled
        GetService<CspOfApp>().RequiredPostSetup();
    }

    [PrivateApi]
    protected void AttachApp(IAppForCode app) => App = app;

    // Todo: this is used 1x - probably where it's used it would already be available in other form
    // It's used in CodeDynamicApiHelper and I'm trying to replace it with the View.Edition
    // Keep this for a bit, then remove once verified.
    //internal string? EditionForHotBuild => _editionForHotBuild.Get(() => Block == null
    //    ? null
    //    : App == null
    //        ? null
    //        : Services.EditionSvc.Edition(Block.View, App.AppReader));
    //private readonly LazyGet<string?> _editionForHotBuild = new();

    [PrivateApi]
    public ICmsContext CmsContext { get; }

    internal IModule? ModuleIfBlockUnknown { get; private set; }

    #endregion


    PiggyBack IHasPiggyBack.PiggyBack { get; } = new();

    [PrivateApi]
    void IServiceWithSetup<CodeContextOptions>.Setup(CodeContextOptions options)
    {
        Setup(options);
    }

    public virtual ICodeContext Setup(CodeContextOptions options)
    {
        this.LinkLog(options.ParentLog);
        var l = Log.Fn<ICodeContext>();

        Cdf.SetCompatibilityLevel(options.Compatibility);
        ModuleIfBlockUnknown = options.ModuleIfBlockUnknown;

        // Case 1: If no block is provided, we can still attach an app if it was provided
        if (options.BlockOrNull == null)
        {
            if (options.AppToAttach == null)
                return l.Return(this, "no block");
            AttachApp(options.AppToAttach);
            return l.Return(this, "no block, but with app");
        }

        // Case 2: If a block is provided, we can attach the app from the block, or override it with the provided app
        var block = options.BlockOrNull;
        Block = block;

        // Only attach App if it exists
        // For example when using the Execution Context in a module container, which doesn't have initialized data yet
        if (options.AppToAttach != null || block.AppIsReady)
            AttachApp(options.AppToAttach ?? block.App);

        return l.Return(this, $"AppId: {App?.AppId}, Block: {(block.ConfigurationIsReady ? block.Configuration?.BlockIdentifierOrNull?.Guid : null)}");
    }


    [PrivateApi] public ICodeDataFactory Cdf => field ??= GetService<ICodeDataFactory>();

    // 2026-11-27 probably not needed any more
    //ICodeDataFactory IWrapper<ICodeDataFactory>.GetContents()
    //    => Cdf;

    /// <inheritdoc />
    public IAppForCode? App { get; private set; }

    /// <summary>
    /// WIP!
    /// </summary>
    [field: AllowNull, MaybeNull]
    internal ICodeDynamicApiHelper DynamicApi => field ??= new CodeDynamicApiHelper(this);

    [field: AllowNull, MaybeNull]
    internal ICodeTypedApiHelper TypedApi => field ??= new CodeTypedApiHelper(this);

}
