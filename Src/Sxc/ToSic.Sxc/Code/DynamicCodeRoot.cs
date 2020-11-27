using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Edit.InPageEditingSystem;
using ToSic.Sxc.Run.Context;
using ToSic.Sxc.Web;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Base class for any dynamic code root objects. <br/>
    /// Root objects are the ones compiled by 2sxc - like the RazorComponent or ApiController. <br/>
    /// If you create code for dynamic compilation, you'll always inherit from ToSic.Sxc.Dnn.DynamicCode. 
    /// Note that other DynamicCode objects like RazorComponent or ApiController reference this object for all the interface methods of <see cref="IDynamicCode"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class DynamicCodeRoot : HasLog, IDynamicCode
    {
        [PrivateApi] public IServiceProvider ServiceProvider { get; }

        //public DynamicCodeRoot(IServiceProvider serviceProvider, IContextOfSite context) : this(serviceProvider, context, "Sxc") { }

        protected DynamicCodeRoot(IServiceProvider serviceProvider, IContextOfSite context, string logPrefix) : base(logPrefix + ".DynCdR")
        {
            ServiceProvider = serviceProvider;
            //Context = context;
        }

        [PrivateApi]
        public DynamicCodeRoot Init(IBlock block, ILog parentLog, int compatibility = 10)
        {
            Log.LinkTo(parentLog ?? block?.Log);
            if (block == null)
                return this;

            CompatibilityLevel = compatibility;
            Block = block;
            App = Block.App;
            Data = Block.Data;
            Edit = new InPageEditingHelper(Block, Log);

            //Context = block.Context;

            return this;
        }

        /// <inheritdoc />
        public IApp App { get; private set; }

        /// <inheritdoc />
        public IBlockDataSource Data { get; private set; }

        /// <inheritdoc />
        public ILinkHelper Link { get; protected set; }


        
        #region Edit

        /// <inheritdoc />
        public IInPageEditingSystem Edit { get; private set; }
        #endregion



        #region Context WIP

        [PrivateApi] public RunContext RunContext => _runContext ?? (_runContext = Block.Context.ServiceProvider.Build<RunContext>().Init(this));
        private RunContext _runContext;

        #endregion
    }
}