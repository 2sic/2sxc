using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.DataSources;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
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
    public abstract partial class DynamicCodeRoot : ServiceBase, IDynamicCodeRoot, IDynamicCode
    {
        #region Constructor

        /// <summary>
        /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
        /// </summary>
        [PrivateApi]
        public class Dependencies: ServiceDependencies
        {
            public ILazySvc<DataSourceFactory> DataSourceFactory { get; }
            public ILazySvc<IConvertService> ConvertService { get; }
            internal IServiceProvider ServiceProvider { get; }
            public LazySvc<CodeCompiler> CodeCompilerLazy { get; }
            public AppSettingsStack SettingsStack { get; }
            public ILazySvc<DynamicEntityDependencies> DynamicEntityDependencies { get; }
            public ILazySvc<IContextOfApp> ContextOfApp { get; }
            public ILazySvc<AdamManager> AdamManager { get; }

            public Dependencies(
                IServiceProvider serviceProvider,
                LazySvc<CodeCompiler> codeCompilerLazy,
                AppSettingsStack settingsStack,
                ILazySvc<DynamicEntityDependencies> dynamicEntityDependencies,
                ILazySvc<IContextOfApp> contextOfApp,
                ILazySvc<AdamManager> adamManager,
                ILazySvc<IConvertService> convertService,
                ILazySvc<DataSourceFactory> dataSourceFactory)
            {
                AddToLogQueue(
                    ServiceProvider = serviceProvider,
                    CodeCompilerLazy = codeCompilerLazy,
                    SettingsStack = settingsStack,
                    DynamicEntityDependencies = dynamicEntityDependencies,
                    ContextOfApp = contextOfApp,
                    AdamManager = adamManager,
                    ConvertService = convertService,
                    DataSourceFactory = dataSourceFactory
                );
            }


        }

        [PrivateApi]
        protected internal DynamicCodeRoot(Dependencies dependencies, string logPrefix) : base(logPrefix + ".DynCdR")
        {
            Deps = dependencies.SetLog(Log);
            _serviceProvider = dependencies.ServiceProvider;

            // Prepare services which need to be attached to this dynamic code root
            CmsContext = GetService<ICmsContext>();

            // Make sure we get and initialize (auto-connect) the app-level CSP if that exists or is enabled
            GetService<CspOfApp>();
        }

        private readonly Dependencies Deps;
        private readonly IServiceProvider _serviceProvider;

        [PrivateApi] public ICmsContext CmsContext { get; }

        #endregion


        /// <inheritdoc />
        public TService GetService<TService>()
        {
            var newService = _serviceProvider.Build<TService>(Log);
            if (newService is INeedsDynamicCodeRoot newWithNeeds)
                newWithNeeds.ConnectToRoot(this);
            return newService;
        }

        //[PrivateApi]
        //internal PiggyBack PiggyBack => _piggyBack ?? (_piggyBack = new PiggyBack());
        //private PiggyBack _piggyBack;

        [PrivateApi]
        public virtual IDynamicCodeRoot InitDynCodeRoot(IBlock block, ILog parentLog, int compatibility)
        {
            this.LinkLog(parentLog ?? block?.Log);
            var cLog = Log.Fn<IDynamicCodeRoot>();

            CompatibilityLevel = compatibility;
            //((CmsContext)CmsContext).AttachContext(this);
            if (block == null)
                return cLog.Return(this, "no block");

            Block = block;
            Data = block.Data;
            AttachApp(block.App);


            return cLog.Return(this, $"AppId: {App?.AppId}, Block: {block?.Configuration?.BlockIdentifierOrNull?.Guid}");
        }

        /// <inheritdoc />
        public IApp App { get; private set; }

        /// <inheritdoc />
        public IBlockDataSource Data { get; private set; }

        /// <inheritdoc />
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