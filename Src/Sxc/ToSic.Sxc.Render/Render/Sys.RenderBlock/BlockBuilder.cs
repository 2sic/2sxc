using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Render.Sys.ModuleHtml;
using ToSic.Sxc.Sys.Integration.Installation;
using ToSic.Sxc.Web.Sys.PageService;
using ToSic.Sys.Capabilities.Licenses;
using ToSic.Sys.Code.InfoSystem;

namespace ToSic.Sxc.Render.Sys.RenderBlock;

/// <summary>
/// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
/// IDs of Zone and App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
/// It is needed for just about anything, because without this set of information
/// it would be hard to get anything done .
/// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
/// </summary>
[PrivateApi("not sure yet what to call this, maybe BlockHost or something")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class BlockBuilder(BlockBuilder.Dependencies services)
    : ServiceBase<BlockBuilder.Dependencies>(services, "Sxc.BlkBld"), IBlockBuilder
{
    public record Dependencies(
        IEngineFactory EngineFactory,
        Generator<IEnvironmentInstaller> EnvInstGen,
        Generator<IRenderingHelper> RenderHelpGen,
        LazySvc<PageChangeSummary> PageChangeSummary,
        LazySvc<ILicenseService> LicenseService,
        IModuleHtmlService ModuleHtmlService,
        CodeInfosInScope CodeInfos,
        BlockCachingHelper BlockCachingHelper)
        : DependenciesRecord(connect:
            [EngineFactory, EnvInstGen, RenderHelpGen, PageChangeSummary, LicenseService, ModuleHtmlService, CodeInfos, BlockCachingHelper]);

    #region Constructor

    public IBlockBuilder Setup(IBlock cb)
    {
        var l = Log.Fn<IBlockBuilder>($"get CmsInstance for a:{cb.AppId} cb:{cb.ContentBlockId}");
        // the root block is the main container. If there is none yet, use this, as it will be the root
        Block = cb;
        return l.Return(this);
    }


    /// <inheritdoc />
    public IBlock Block { get; private set; } = null!;


    #endregion

}