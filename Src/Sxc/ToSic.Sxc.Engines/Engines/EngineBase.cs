using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Render.Sys.Output;
using ToSic.Sxc.Render.Sys.Specs;

namespace ToSic.Sxc.Engines;

/// <summary>
/// The foundation for engines - must be inherited by other engines
/// </summary>
[PrivateApi("used to be InternalApi_DoNotUse_MayChangeWithoutNotice, hidden in 17.08")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class EngineBase : ServiceBase<EngineBase.Dependencies>, IEngine
{
    #region MyServices

    public record Dependencies(
        EngineSpecsService EngineSpecsService,
        IBlockResourceExtractor BlockResourceExtractor,
        EngineAppRequirements EngineAppRequirements)
        : DependenciesRecord(connect:
            [EngineSpecsService, BlockResourceExtractor, EngineAppRequirements]);

    #endregion

    #region Constructor and DI

    /// <summary>
    /// Empty constructor, so it can be used in dependency injection
    /// </summary>
    protected EngineBase(Dependencies services, object[]? connect = default)
        : base(services, $"{SxcLogName}.EngBas", connect: connect)
    { }

    #endregion

    //public abstract void Init(IBlock block);

    /// <inheritdoc />
    public abstract RenderEngineResult Render(IBlock block, RenderSpecs specs);

}