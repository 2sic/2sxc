using ToSic.Eav;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
    /// IDs of Zone and App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
    /// It is needed for just about anything, because without this set of information
    /// it would be hard to get anything done .
    /// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
    /// </summary>
    [PrivateApi("not sure yet what to call this, maybe BlockHost or something")]
    public partial class BlockBuilder : HasLog, IBlockBuilder
    {
        #region App-level information
        public IApp App => Block.App;

        #endregion


        #region Info for current runtime instance

        /// <summary>
        /// Environment - should be the place to refactor everything into, which is the host around 2sxc
        /// </summary>
        public IAppEnvironment Environment { get; }

        /// <inheritdoc />
        public IInstanceContext Context { get; }

        /// <inheritdoc />
        public IBlock Block { get; }

        public IBlockBuilder RootBuilder { get; }
        #endregion

        #region Constructor
        internal BlockBuilder(IBlockBuilder rootBlockBuilder, IBlock cb, 
            IInstanceContext ctx,
            ILog parentLog)
            : base("Sxc.BlkBld", parentLog, $"get CmsInstance for a:{cb?.AppId} cb:{cb?.ContentBlockId}")
        {
            Environment = Factory.Resolve<IAppEnvironment>().Init(parentLog);
            // the root block is the main container. If there is none yet, use this, as it will be the root
            RootBuilder = rootBlockBuilder ?? this;
            Block = cb;
            Context = ctx;
        }

        #endregion

    }
}