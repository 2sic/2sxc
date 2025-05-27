using ToSic.Eav.Internal.Licenses;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Integration.Installation;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.PageService;
using ToSic.Sys.Code.InfoSystem;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
/// IDs of Zone and App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
/// It is needed for just about anything, because without this set of information
/// it would be hard to get anything done .
/// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
/// </summary>
[PrivateApi("not sure yet what to call this, maybe BlockHost or something")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class BlockBuilder(BlockBuilder.MyServices services)
    : ServiceBase<BlockBuilder.MyServices>(services, "Sxc.BlkBld"), IBlockBuilder
{
    public class MyServices(
        IEngineFactory engineFactory,
        Generator<IEnvironmentInstaller> envInstGen,
        Generator<IRenderingHelper> renderHelpGen,
        LazySvc<PageChangeSummary> pageChangeSummary,
        LazySvc<ILicenseService> licenseService,
        IModuleService moduleService,
        CodeInfosInScope codeInfos)
        : MyServicesBase(connect:
            [engineFactory, envInstGen, renderHelpGen, pageChangeSummary, licenseService, moduleService, codeInfos])
    {
        public CodeInfosInScope CodeInfos { get; } = codeInfos;
        public IEngineFactory EngineFactory { get; } = engineFactory;
        public Generator<IEnvironmentInstaller> EnvInstGen { get; } = envInstGen;
        public Generator<IRenderingHelper> RenderHelpGen { get; } = renderHelpGen;
        public LazySvc<PageChangeSummary> PageChangeSummary { get; } = pageChangeSummary;
        public LazySvc<ILicenseService> LicenseService { get; } = licenseService;
        public IModuleService ModuleService { get; } = moduleService;
    }

    #region Constructor

    public IBlockBuilder Setup(IBlock cb)
    {
        var l = Log.Fn<BlockBuilder>($"get CmsInstance for a:{cb.AppId} cb:{cb.ContentBlockId}");
        // the root block is the main container. If there is none yet, use this, as it will be the root
        Block = cb;
        return l.Return(this);
    }


    /// <inheritdoc />
    public IBlock Block { get; private set; }


    #endregion

}