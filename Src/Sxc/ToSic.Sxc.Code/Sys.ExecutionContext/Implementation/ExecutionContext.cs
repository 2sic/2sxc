using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeApiService;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Polymorphism.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;
using ToSic.Sys.Caching.PiggyBack;
using IApp = ToSic.Sxc.Apps.IApp;
// ReSharper disable InheritdocInvalidUsage

namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Base class for any dynamic code root objects. <br/>
/// Root objects are the ones compiled by 2sxc - like the RazorComponent or ApiController. <br/>
/// If you create code for dynamic compilation, you'll always inherit from ToSic.Sxc.Dnn.DynamicCode.
/// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
/// </summary>
/// <remarks>
/// It can usually not be created directly, but through the <see cref="ExecutionContextUnknown"/> which would return a <see cref="IExecutionContextFactory"/>.
/// </remarks>
[PrivateApi("Was public till v17, and previously called DynamicCodeRoot")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class ExecutionContext : ServiceBase<ExecutionContext.Dependencies>, IExecutionContext, IGetCodePath, IHasPiggyBack
{
    #region Constructor

    /// <summary>
    /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
    /// </summary>
    [PrivateApi]
    public record Dependencies(
        IServiceProvider ServiceProvider,
        LazySvc<IClassCompiler> CodeCompilerLazy,
        AppDataStackService DataStackService,
        LazySvc<IConvertService> ConvertService,
        LazySvc<CodeCreateDataSourceSvc> DataSources,
        ICodeDataFactory Cdf,
        PolymorphConfigReader Polymorphism)
        : DependenciesRecord(connect:
            [/* never! serviceProvider */ CodeCompilerLazy, DataStackService, ConvertService, DataSources, Cdf, Polymorphism]);

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

    #endregion


    PiggyBack IHasPiggyBack.PiggyBack { get; } = new();



    [PrivateApi]
    public virtual IExecutionContext InitDynCodeRoot(IBlock? block, ILog? parentLog)
    {
        this.LinkLog(parentLog ?? (block as IHasLog)?.Log);
        var cLog = Log.Fn<IExecutionContext>();

        if (block == null)
            return cLog.Return(this, "no block");

        Block = block;
        AttachApp(block.App);

        return cLog.Return(this, $"AppId: {App?.AppId}, Block: {(block.ConfigurationIsReady ? block.Configuration?.BlockIdentifierOrNull?.Guid : null)}");
    }

    /// <inheritdoc />
    internal IApp App { get; private set; } = null!;

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    [field: AllowNull, MaybeNull]
    internal ILinkService Link => field ??= GetService<ILinkService>(reuse: true);


    #region Edit

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    internal IEditService Edit => field ??= GetService<IEditService>(reuse: true);

    #endregion

    [PrivateApi("Not yet ready")]
    internal IDevTools DevTools => throw new NotImplementedException("This is a future feature, we're just reserving the object name");

    /// <summary>
    /// WIP!
    /// </summary>
    [field: AllowNull, MaybeNull]
    internal ICodeDynamicApiHelper DynamicApi => field ??= new CodeDynamicApiHelper(this);

    [field: AllowNull, MaybeNull]
    internal ICodeTypedApiHelper TypedApi => field ??= new CodeTypedApiHelper(this);
}