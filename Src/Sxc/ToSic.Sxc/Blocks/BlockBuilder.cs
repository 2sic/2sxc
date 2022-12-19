using System;
using ToSic.Eav.Configuration.Licenses;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Run;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.PageService;

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
        public class Dependencies: ServiceDependencies
        {
            public Dependencies(
                EngineFactory engineFactory,
                Generator<IEnvironmentInstaller> envInstGen,
                Generator<IRenderingHelper> renderHelpGen,
                LazyInitLog<PageChangeSummary> pageChangeSummary,
                Lazy<ILicenseService> licenseService,
                IModuleService moduleService
            ) => AddToLogQueue(
                EngineFactory = engineFactory,
                EnvInstGen = envInstGen,
                RenderHelpGen = renderHelpGen,
                PageChangeSummary = pageChangeSummary,
                LicenseService = licenseService,
                ModuleService = moduleService
            );

            public EngineFactory EngineFactory { get; }
            public Generator<IEnvironmentInstaller> EnvInstGen { get; }
            public Generator<IRenderingHelper> RenderHelpGen { get; }
            public LazyInitLog<PageChangeSummary> PageChangeSummary { get; }
            public Lazy<ILicenseService> LicenseService { get; }
            public IModuleService ModuleService { get; }
        }

        #region Constructor
        public BlockBuilder(Dependencies dependencies) : base("Sxc.BlkBld") 
            => _deps = dependencies.SetLog(Log);
        private readonly Dependencies _deps;

        public BlockBuilder Init(IBlockBuilder rootBlockBuilder, IBlock cb)
        {
            Log.A($"get CmsInstance for a:{cb?.AppId} cb:{cb?.ContentBlockId}");
            // the root block is the main container. If there is none yet, use this, as it will be the root
            RootBuilder = rootBlockBuilder ?? this;
            Block = cb;
            return this;
        }
        #region Info for current runtime instance

        /// <inheritdoc />
        public IBlock Block { get; private set; }

        public IBlockBuilder RootBuilder { get; private set; }
        #endregion



        #endregion

    }
}