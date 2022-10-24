using System;
using ToSic.Eav.Apps;
using ToSic.Eav.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
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
    public abstract partial class DynamicCodeRoot : HasLog, IDynamicCodeRoot, IDynamicCode
    {
        #region Constructor

        /// <summary>
        /// Helper class to ensure if dependencies change, inheriting objects don't need to change their signature
        /// </summary>
        [PrivateApi]
        public class Dependencies
        {
            public Dependencies(IServiceProvider serviceProvider, Lazy<CodeCompiler> codeCompilerLazy, AppSettingsStack settingsStack)
            {
                ServiceProvider = serviceProvider;
                CodeCompilerLazy = codeCompilerLazy;
                SettingsStack = settingsStack;
            }
            internal IServiceProvider ServiceProvider { get; }
            public Lazy<CodeCompiler> CodeCompilerLazy { get; }
            public AppSettingsStack SettingsStack { get; }

        }

        [PrivateApi]
        protected internal DynamicCodeRoot(Dependencies dependencies, string logPrefix) : base(logPrefix + ".DynCdR")
        {
            Deps = dependencies;
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
            var newService = _serviceProvider.Build<TService>();
            if(newService is INeedsDynamicCodeRoot newWithNeeds)
                newWithNeeds.ConnectToRoot(this);

            return newService;
        }

        //[PrivateApi]
        //internal PiggyBack PiggyBack => _piggyBack ?? (_piggyBack = new PiggyBack());
        //private PiggyBack _piggyBack;

        [PrivateApi]
        public virtual IDynamicCodeRoot InitDynCodeRoot(IBlock block, ILog parentLog, int compatibility)
        {
            Log.LinkTo(parentLog ?? block?.Log);
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