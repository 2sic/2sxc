using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Run;

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
        #region Info for current runtime instance

        /// <inheritdoc />
        public IBlock Block { get; }

        public IBlockBuilder RootBuilder { get; }
        #endregion


        #region Constructor
        internal BlockBuilder(IBlockBuilder rootBlockBuilder, 
            IBlock cb, 
            Generator<IEnvironmentInstaller> envInstGen, 
            Generator<IRenderingHelper> renderHelpGen,
            Generator<IRazorEngine> razorEngineGen, 
            Generator<TokenEngine> tokenEngineGen,
            ILog parentLog)
            : base("Sxc.BlkBld", parentLog, $"get CmsInstance for a:{cb?.AppId} cb:{cb?.ContentBlockId}")
        {
            _envInstGen = envInstGen;
            _renderHelpGen = renderHelpGen;
            _razorEngineGen = razorEngineGen;
            _tokenEngineGen = tokenEngineGen;
            // the root block is the main container. If there is none yet, use this, as it will be the root
            RootBuilder = rootBlockBuilder ?? this;
            Block = cb;
        }

        private readonly Generator<IEnvironmentInstaller> _envInstGen;
        private readonly Generator<IRenderingHelper> _renderHelpGen;
        private readonly Generator<IRazorEngine> _razorEngineGen;
        private readonly Generator<TokenEngine> _tokenEngineGen;

        #endregion

    }
}