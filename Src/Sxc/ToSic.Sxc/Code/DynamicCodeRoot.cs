using System;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;
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
            public IServiceProvider ServiceProvider { get; }
            public ICmsContext CmsContext { get; }
            public Lazy<CodeCompiler> CodeCompilerLazy { get; }

            public Dependencies(IServiceProvider serviceProvider, ICmsContext cmsContext, Lazy<CodeCompiler> codeCompilerLazy)
            {
                ServiceProvider = serviceProvider;
                CmsContext = cmsContext;
                CodeCompilerLazy = codeCompilerLazy;
            }
        }

        [PrivateApi]
        protected DynamicCodeRoot(Dependencies dependencies, string logPrefix) : base(logPrefix + ".DynCdR")
        {
            Deps = dependencies;
            _serviceProvider = dependencies.ServiceProvider;
            CmsContext = dependencies.CmsContext;
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

        [PrivateApi]
        internal PiggyBack PiggyBack => _piggyBack ?? (_piggyBack = new PiggyBack());
        private PiggyBack _piggyBack;

        [PrivateApi]
        public virtual IDynamicCodeRoot Init(IBlock block, ILog parentLog, int compatibility)
        {
            Log.LinkTo(parentLog ?? block?.Log);
            CompatibilityLevel = compatibility;
            if (block == null)
                return this;

            ((CmsContext) CmsContext).Update(block);
            Block = block;
            Data = block.Data;

            AttachApp(block.App);

            return this;
        }

        /// <inheritdoc />
        public IApp App { get; private set; }

        /// <inheritdoc />
        public IBlockDataSource Data { get; private set; }

        /// <inheritdoc />
        // Note that ILinkHelper uses INeedsCodeRoot, so if initialized in GetService this will be auto-provided
        public ILinkHelper Link => _link ?? (_link = GetService<ILinkHelper>());
        private ILinkHelper _link;


        #region Edit

        /// <inheritdoc />
        public IInPageEditingSystem Edit => _edit ?? (_edit = GetService<IInPageEditingSystem>());// { get; private set; }
        private IInPageEditingSystem _edit;

        #endregion

        #region Accessor to Root

        [PrivateApi] public IDynamicCodeRoot _DynCodeRoot => this;

        #endregion

        public IDevTools DevTools => throw new NotImplementedException("This is a future feature, we're just reserving the object name");
    }
}