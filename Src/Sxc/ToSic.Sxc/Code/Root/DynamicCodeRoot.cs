using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Services;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Code.Helpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.AsConverter;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using IApp = ToSic.Sxc.Apps.IApp;
// ReSharper disable InheritdocInvalidUsage
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Base class for any dynamic code root objects. <br/>
    /// Root objects are the ones compiled by 2sxc - like the RazorComponent or ApiController. <br/>
    /// If you create code for dynamic compilation, you'll always inherit from ToSic.Sxc.Dnn.DynamicCode.
    /// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class DynamicCodeRoot : ServiceBase<DynamicCodeRoot.MyServices>, IDynamicCodeRoot
    {
        #region Constructor

        /// <summary>
        /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
        /// </summary>
        [PrivateApi]
        public class MyServices: MyServicesBase
        {
            public AsConverterService AsConverter => _asConverter.Value;
            private readonly LazySvc<AsConverterService> _asConverter;
            public LazySvc<DynamicCodeDataSources> DataSources { get; }
            public LazySvc<IDataSourcesService> DataSourceFactory { get; }
            public LazySvc<IConvertService> ConvertService { get; }
            internal IServiceProvider ServiceProvider { get; }
            public LazySvc<CodeCompiler> CodeCompilerLazy { get; }
            public AppSettingsStack SettingsStack { get; }

            public MyServices(
                IServiceProvider serviceProvider,
                LazySvc<CodeCompiler> codeCompilerLazy,
                AppSettingsStack settingsStack,
                LazySvc<IConvertService> convertService,
                LazySvc<IDataSourcesService> dataSourceFactory,
                LazySvc<DynamicCodeDataSources> dataSources,
                LazySvc<AsConverterService> asConverter)
            {
                ConnectServices(
                    ServiceProvider = serviceProvider,
                    CodeCompilerLazy = codeCompilerLazy,
                    SettingsStack = settingsStack,
                    ConvertService = convertService,
                    DataSourceFactory = dataSourceFactory,
                    DataSources = dataSources,
                    _asConverter = asConverter
                );
            }


        }

        [PrivateApi]
        protected internal DynamicCodeRoot(MyServices services, string logPrefix) : base(services, logPrefix + ".DynCdR")
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


        /// <inheritdoc cref="IDynamicCode.GetService{TService}" />
        public TService GetService<TService>()
        {
            var newService = _serviceProvider.Build<TService>(Log);
            if (newService is INeedsDynamicCodeRoot newWithNeeds)
                newWithNeeds.ConnectToRoot(this);
            return newService;
        }

        [PrivateApi]
        public virtual IDynamicCodeRoot InitDynCodeRoot(IBlock block, ILog parentLog, int compatibility)
        {
            this.LinkLog(parentLog ?? block?.Log);
            var cLog = Log.Fn<IDynamicCodeRoot>($"{nameof(compatibility)}: {compatibility}");

            AsC.SetCompatibilityLevel(compatibility);
            if (block == null)
                return cLog.Return(this, "no block");

            Block = block;
            Data = block.Data;
            // Data.ConnectToRoot(this);
            AttachApp(block.App);


            return cLog.Return(this, $"AppId: {App?.AppId}, Block: {block.Configuration?.BlockIdentifierOrNull?.Guid}");
        }

        /// <inheritdoc />
        public IApp App { get; private set; }

        /// <inheritdoc />
        public IContextData Data { get; private set; }

        /// <inheritdoc cref="IDynamicCode.Link" />
        // Note that ILinkHelper uses INeedsCodeRoot, so if initialized in GetService this will be auto-provided
        public ILinkService Link => _link ?? (_link = GetService<ILinkService>());
        private ILinkService _link;


        #region Edit

        /// <inheritdoc />
        public IEditService Edit => _edit ?? (_edit = GetService<IEditService>());
        private IEditService _edit;

        #endregion

        #region Accessor to Root

        // ReSharper disable once InconsistentNaming
        [PrivateApi] public IDynamicCodeRoot _DynCodeRoot => this;

        #endregion

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => throw new NotImplementedException("This is a future feature, we're just reserving the object name");
    }
}