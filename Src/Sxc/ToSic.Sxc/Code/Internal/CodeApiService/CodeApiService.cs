using ToSic.Eav.Apps.Services;
using ToSic.Eav.Services;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;
using IApp = ToSic.Sxc.Apps.IApp;
// ReSharper disable InheritdocInvalidUsage

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Base class for any dynamic code root objects. <br/>
/// Root objects are the ones compiled by 2sxc - like the RazorComponent or ApiController. <br/>
/// If you create code for dynamic compilation, you'll always inherit from ToSic.Sxc.Dnn.DynamicCode.
/// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
/// </summary>
[PrivateApi("Was public till v17, and previously called DynamicCodeRoot")]
public abstract partial class CodeApiService : ServiceBase<CodeApiService.MyServices>, ICodeApiService
{
    #region Constructor

    /// <summary>
    /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
    /// </summary>
    [PrivateApi]
    public class MyServices: MyServicesBase
    {
        public CodeDataFactory Cdf => _cdf.Value;
        private readonly LazySvc<CodeDataFactory> _cdf;
        public LazySvc<CodeCreateDataSourceSvc> DataSources { get; }
        public LazySvc<IDataSourcesService> DataSourceFactory { get; }
        public LazySvc<IConvertService> ConvertService { get; }
        internal IServiceProvider ServiceProvider { get; }
        public LazySvc<CodeCompiler> CodeCompilerLazy { get; }
        public AppDataStackService DataStackService { get; }
        public Polymorphism.Internal.PolymorphConfigReader Polymorphism { get; }

        public MyServices(
            IServiceProvider serviceProvider,
            LazySvc<CodeCompiler> codeCompilerLazy,
            AppDataStackService dataStackService,
            LazySvc<IConvertService> convertService,
            LazySvc<IDataSourcesService> dataSourceFactory,
            LazySvc<CodeCreateDataSourceSvc> dataSources,
            LazySvc<CodeDataFactory> cdf,
            Polymorphism.Internal.PolymorphConfigReader polymorphism)
        {
            ConnectServices(
                ServiceProvider = serviceProvider,
                CodeCompilerLazy = codeCompilerLazy,
                DataStackService = dataStackService,
                ConvertService = convertService,
                DataSourceFactory = dataSourceFactory,
                DataSources = dataSources,
                _cdf = cdf,
                Polymorphism = polymorphism
            );
        }


    }

    [PrivateApi]
    protected internal CodeApiService(MyServices services, string logPrefix) : base(services, logPrefix + ".DynCdR")
    {
        _serviceProvider = services.ServiceProvider;

        // Prepare services which need to be attached to this dynamic code root
        CmsContext = GetService<ICmsContext>();

        // Make sure we get and initialize (auto-connect) the app-level CSP if that exists or is enabled
        GetService<CspOfApp>();
    }

    private readonly IServiceProvider _serviceProvider;

    [PrivateApi] public ICmsContext CmsContext { get; }

    #endregion


    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class
    {
        var newService = _serviceProvider.Build<TService>(Log);
        if (newService is INeedsCodeApiService newWithNeeds)
            newWithNeeds.ConnectToRoot(this);
        return newService;
    }

    TService ICodeApiServiceInternal.GetKitService<TService>()
    {
        if (_kitServices.TryGetValue(typeof(TService), out var service))
            return (TService)service;
        var generated = _CodeApiSvc.GetService<TService>();
        _kitServices[typeof(TService)] = generated;
        return generated;
    }
    private readonly Dictionary<Type, object> _kitServices = new();


    [PrivateApi]
    public virtual ICodeApiService InitDynCodeRoot(IBlock block, ILog parentLog)
    {
        this.LinkLog(parentLog ?? block?.Log);
        var cLog = Log.Fn<ICodeApiService>();

        if (block == null)
            return cLog.Return(this, "no block");

        _Block = block;
        Data = block.Data;
        AttachApp(block.App);


        return cLog.Return(this, $"AppId: {App?.AppId}, Block: {block.Configuration?.BlockIdentifierOrNull?.Guid}");
    }

    /// <inheritdoc />
    public IApp App { get; private set; }

    /// <inheritdoc />
    public IBlockInstance Data { get; private set; }

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _link ??= (this as ICodeApiServiceInternal).GetKitService<ILinkService>();
    private ILinkService _link;


    #region Edit

    /// <inheritdoc />
    public IEditService Edit => _edit ??= (this as ICodeApiServiceInternal).GetKitService<IEditService>();
    private IEditService _edit;

    #endregion

    #region Accessor to Root

    // ReSharper disable once InconsistentNaming
    [PrivateApi] public ICodeApiService _CodeApiSvc => this;

    #endregion

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => throw new NotImplementedException("This is a future feature, we're just reserving the object name");
}