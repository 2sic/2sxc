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

    public virtual void Init(IBlock block)
    {
        var l = Log.Fn();
        EngineSpecs = Services.EngineSpecsService.GetSpecs(block);
        l.Done();
    }

    [PrivateApi]
    protected abstract (string? Contents, List<Exception>? Exception) RenderEntryRazor(EngineSpecs engineSpecs, RenderSpecs specs);

    /// <inheritdoc />
    public virtual RenderEngineResult Render(RenderSpecs specs)
    {
        var l = Log.Fn<RenderEngineResult>(timer: true);
            
        // check if rendering is possible, or throw exceptions...
        var preFlightResult = CheckExpectedNoRenderConditions(EngineSpecs);
        if (preFlightResult != null)
            return l.Return(preFlightResult, $"error: {preFlightResult.ErrorCode}");

        var renderedTemplate = RenderEntryRazor(EngineSpecs, specs);
        var resourceExtractor = Services.BlockResourceExtractor;
        var result = resourceExtractor.Process(renderedTemplate.Contents ?? "");
        if (renderedTemplate.Exception != null)
            result = result with
            {
                // Note: not sure why the existing exceptions have precedence or are not mixed, but this is how the original code before 2025-03-17 was.
                ExceptionsOrNull = result.ExceptionsOrNull ?? renderedTemplate.Exception,
            };
        return l.ReturnAsOk(result);
    }

    private RenderEngineResult? CheckExpectedNoRenderConditions(EngineSpecs engineSpecs)
    {
        var l = Log.Fn<RenderEngineResult>();

        // Check App Requirements (new 16.08)
        var block = engineSpecs.Block;
        var appReqProblems = Services.EngineAppRequirements
            .GetMessageForRequirements(block.Context.AppReaderRequired);
        if (appReqProblems != null)
            return l.Return(appReqProblems, "error");

        var view = engineSpecs.View;
        if (view.ContentType == "" || view.ContentItem != null || block.Configuration.Content.Any(e => e != null))
            return l.ReturnNull("all ok");

        var result = new RenderEngineResult
        {
            Html = EngineMessages.ToolbarForEmptyTemplate,
            ActivateJsApi = false,
            Assets = [],
            ErrorCode = null,
            ExceptionsOrNull = null, // should be null, to indicate no exceptions
        };
        return l.Return(result, "error");

    }

}