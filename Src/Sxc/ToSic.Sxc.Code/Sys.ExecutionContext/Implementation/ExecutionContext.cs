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

namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Base class for any dynamic code root objects. <br/>
/// Root objects are the ones compiled by 2sxc - like the RazorComponent or ApiController. <br/>
/// If you create code for dynamic compilation, you'll always inherit from ToSic.Sxc.Dnn.DynamicCode.
/// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
/// </summary>
/// <remarks>
/// It can usually not be created directly, but through the <see cref="IExecutionContextFactory"/>.
/// </remarks>
[PrivateApi("Was public till v17, and previously called DynamicCodeRoot")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class ExecutionContext : ServiceBase<ExecutionContext.Dependencies>, IExecutionContext, IHasPiggyBack, IServiceWithSetup<ExecutionContextOptions>
{
    #region Constructor

    /// <summary>
    /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
    /// </summary>
    [PrivateApi]
    public record Dependencies(
        IServiceProvider ServiceProvider,
        AppDataStackService DataStackService,
        LazySvc<ExCtxDataSourceService> DataSources,
        ICodeDataFactory Cdf,
        IEditionService EditionSvc)
        : DependenciesBase(connect:
            [/* never! serviceProvider */ DataStackService, DataSources, Cdf, EditionSvc]);

    [PrivateApi]
    protected internal ExecutionContext(Dependencies services, string logPrefix) : base(services, logPrefix + ".DynCdR")
    {
        // Prepare services which need to be attached to this dynamic code root
        CmsContext = GetService<ICmsContext>();

        // Make sure we get and initialize (auto-connect) the app-level CSP if that exists or is enabled
        GetService<CspOfApp>();
    }

    [PrivateApi]
    internal ICmsContext CmsContext { get; }

    internal IModule? ModuleIfBlockUnknown { get; private set; }

    #endregion


    PiggyBack IHasPiggyBack.PiggyBack { get; } = new();

    [PrivateApi]
    void IServiceWithSetup<ExecutionContextOptions>.Setup(ExecutionContextOptions options)
    {
        Setup(options);
    }

    public virtual IExecutionContext Setup(ExecutionContextOptions options)
    {
        this.LinkLog(options.ParentLog);
        var cLog = Log.Fn<IExecutionContext>();

        Cdf.SetCompatibilityLevel(options.Compatibility);
        ModuleIfBlockUnknown = options.ModuleIfBlockUnknown;

        if (options.BlockOrNull == null)
            return cLog.Return(this, "no block");

        var block = options.BlockOrNull;
        Block = block;

        // Only attach App if it exists
        // For example when using the Execution Context in a module container, which doesn't have initialized data yet
        if (block.AppIsReady)
            AttachApp(block.App);

        return cLog.Return(this, $"AppId: {App?.AppId}, Block: {(block.ConfigurationIsReady ? block.Configuration?.BlockIdentifierOrNull?.Guid : null)}");
    }

    /// <inheritdoc />
    internal IAppForCode? App { get; private set; } = null!;

    /// <summary>
    /// WIP!
    /// </summary>
    [field: AllowNull, MaybeNull]
    internal ICodeDynamicApiHelper DynamicApi => field ??= new CodeDynamicApiHelper(this);

    [field: AllowNull, MaybeNull]
    internal ICodeTypedApiHelper TypedApi => field ??= new CodeTypedApiHelper(this);

}
