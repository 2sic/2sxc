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

    [PrivateApi] protected EngineSpecs EngineSpecs = null!;

    /// <summary>
    /// Empty constructor, so it can be used in dependency injection
    /// </summary>
    protected EngineBase(Dependencies services, object[]? connect = default)
        : base(services, $"{SxcLogName}.EngBas", connect: connect)
    { }

    #endregion

    public abstract void Init(IBlock block);

    [PrivateApi]
    protected abstract (string? Contents, List<Exception>? Exception) RenderEntryRazor(EngineSpecs engineSpecs, RenderSpecs specs);

    /// <inheritdoc />
    public virtual RenderEngineResult Render(IBlock block, RenderSpecs specs)
    {
        var l = Log.Fn<RenderEngineResult>(timer: true);
            
        // check if rendering is possible, or throw exceptions...
        var preFlightResult = Services.EngineAppRequirements.CheckExpectedNoRenderConditions(EngineSpecs);
        if (preFlightResult != null)
            return l.Return(preFlightResult, "error");

        var renderedTemplate = RenderEntryRazor(EngineSpecs, specs);
        var result = Services.BlockResourceExtractor.Process(renderedTemplate.Contents ?? "");
        if (renderedTemplate.Exception != null)
            result = result with
            {
                // Note: not sure why the existing exceptions have precedence or are not mixed, but this is how the original code before 2025-03-17 was.
                ExceptionsOrNull = result.ExceptionsOrNull ?? renderedTemplate.Exception,
            };
        return l.ReturnAsOk(result);
    }

}