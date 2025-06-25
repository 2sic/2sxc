using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.CodeApi;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sys.Caching.PiggyBack;
using IApp = ToSic.Sxc.Apps.IApp;
// ReSharper disable InheritdocInvalidUsage

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Base class for any dynamic code root objects. <br/>
/// Root objects are the ones compiled by 2sxc - like the RazorComponent or ApiController. <br/>
/// If you create code for dynamic compilation, you'll always inherit from ToSic.Sxc.Dnn.DynamicCode.
/// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
/// </summary>
/// <remarks>
/// It can usually not be created directly, but through the <see cref="IExecutionContextFactory"/> which would return a <see cref="ExecutionContextUnknown"/>.
/// </remarks>
[PrivateApi("Was public till v17, and previously called DynamicCodeRoot")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class ExecutionContext : ServiceBase<ExecutionContext.MyServices>, IExecutionContext, IGetCodePath, IHasPiggyBack, ICanGetService
{
    #region Constructor

    /// <summary>
    /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
    /// </summary>
    [PrivateApi]
    public class MyServices(
        IServiceProvider serviceProvider,
        LazySvc<IClassCompiler> codeCompilerLazy,
        AppDataStackService dataStackService,
        LazySvc<IConvertService> convertService,
        LazySvc<CodeCreateDataSourceSvc> dataSources,
        LazySvc<ICodeDataFactory> cdf,
        Polymorphism.Internal.PolymorphConfigReader polymorphism)
        : MyServicesBase(connect:
            [/* never! serviceProvider */ codeCompilerLazy, dataStackService, convertService, dataSources, cdf, polymorphism])
    {
        public ICodeDataFactory Cdf => cdf.Value;
        public LazySvc<CodeCreateDataSourceSvc> DataSources { get; } = dataSources;
        public LazySvc<IConvertService> ConvertService { get; } = convertService;
        internal IServiceProvider ServiceProvider { get; } = serviceProvider;
        public LazySvc<IClassCompiler> CodeCompilerLazy { get; } = codeCompilerLazy;
        public AppDataStackService DataStackService { get; } = dataStackService;
        public Polymorphism.Internal.PolymorphConfigReader Polymorphism { get; } = polymorphism;
    }

    [PrivateApi]
    protected internal ExecutionContext(MyServices services, string logPrefix) : base(services, logPrefix + ".DynCdR")
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


    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class
    {
        var newService = Services.ServiceProvider.Build<TService>(Log);
        if (newService is INeedsExecutionContext newWithNeeds)
            newWithNeeds.ConnectToRoot(this);
        return newService;
    }



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

    /// <inheritdoc cref="IDynamicCode.Link" />
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