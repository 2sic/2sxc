using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Internal.Licenses;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Integration.Installation;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
/// IDs of Zone and App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
/// It is needed for just about anything, because without this set of information
/// it would be hard to get anything done .
/// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
/// </summary>
[PrivateApi("not sure yet what to call this, maybe BlockHost or something")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class BlockBuilder(BlockBuilder.MyServices services)
    : ServiceBase<BlockBuilder.MyServices>(services, "Sxc.BlkBld"), IBlockBuilder
{
    public class MyServices: MyServicesBase
    {

        public MyServices(
            EngineFactory engineFactory,
            Generator<IEnvironmentInstaller> envInstGen,
            Generator<IRenderingHelper> renderHelpGen,
            LazySvc<PageChangeSummary> pageChangeSummary,
            LazySvc<ILicenseService> licenseService,
            IModuleService moduleService,
            CodeInfosInScope codeInfos
        )
        {
            ConnectLogs([
                EngineFactory = engineFactory,
                EnvInstGen = envInstGen,
                RenderHelpGen = renderHelpGen,
                PageChangeSummary = pageChangeSummary,
                LicenseService = licenseService,
                ModuleService = moduleService,
                CodeInfos = codeInfos
            ]);
        }

        public CodeInfosInScope CodeInfos { get; }
        public EngineFactory EngineFactory { get; }
        public Generator<IEnvironmentInstaller> EnvInstGen { get; }
        public Generator<IRenderingHelper> RenderHelpGen { get; }
        public LazySvc<PageChangeSummary> PageChangeSummary { get; }
        public LazySvc<ILicenseService> LicenseService { get; }
        public IModuleService ModuleService { get; }
    }

    #region Constructor

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