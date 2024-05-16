using ToSic.Eav.Apps.Services;
using ToSic.Eav.Data.PiggyBack;
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
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract partial class CodeApiService : ServiceBase<CodeApiService.MyServices>, ICodeApiService, IHasPiggyBack
{
    #region Constructor

    /// <summary>
    /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
    /// </summary>
    [PrivateApi]
    public class MyServices(
        IServiceProvider serviceProvider,
        LazySvc<CodeCompiler> codeCompilerLazy,
        AppDataStackService dataStackService,
        LazySvc<IConvertService> convertService,
        LazySvc<CodeCreateDataSourceSvc> dataSources,
        LazySvc<CodeDataFactory> cdf,
        Polymorphism.Internal.PolymorphConfigReader polymorphism)
        : MyServicesBase(connect:
            [/* never! serviceProvider */ codeCompilerLazy, dataStackService, convertService, dataSources, cdf, polymorphism])
    {
        public CodeDataFactory Cdf => cdf.Value;
        public LazySvc<CodeCreateDataSourceSvc> DataSources { get; } = dataSources;
        public LazySvc<IConvertService> ConvertService { get; } = convertService;
        internal IServiceProvider ServiceProvider { get; } = serviceProvider;
        public LazySvc<CodeCompiler> CodeCompilerLazy { get; } = codeCompilerLazy;
        public AppDataStackService DataStackService { get; } = dataStackService;
        public Polymorphism.Internal.PolymorphConfigReader Polymorphism { get; } = polymorphism;
    }

    [PrivateApi]
    protected internal CodeApiService(MyServices services, string logPrefix) : base(services, logPrefix + ".DynCdR")
    {
        // Prepare services which need to be attached to this dynamic code root
        CmsContext = GetService<ICmsContext>();

        // Make sure we get and initialize (auto-connect) the app-level CSP if that exists or is enabled
        GetService<CspOfApp>();
    }

    [PrivateApi] public ICmsContext CmsContext { get; }

    #endregion


    PiggyBack IHasPiggyBack.PiggyBack { get; } = new();


    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class
    {
        var newService = Services.ServiceProvider.Build<TService>(Log);
        if (newService is INeedsCodeApiService newWithNeeds)
            newWithNeeds.ConnectToRoot(this);
        return newService;
    }



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
    public ILinkService Link => _link ??= GetService<ILinkService>(reuse: true);
    private ILinkService _link;


    #region Edit

    /// <inheritdoc />
    public IEditService Edit => _edit ??= GetService<IEditService>(reuse: true);
    private IEditService _edit;

    #endregion

    #region Accessor to Root

    // ReSharper disable once InconsistentNaming
    [PrivateApi] public ICodeApiService _CodeApiSvc => this;

    #endregion

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => throw new NotImplementedException("This is a future feature, we're just reserving the object name");

}