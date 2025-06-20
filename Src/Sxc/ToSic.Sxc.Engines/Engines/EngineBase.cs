using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Blocks.Internal;
using IApp = ToSic.Sxc.Apps.IApp;
using IDataSource = ToSic.Eav.DataSource.IDataSource;

namespace ToSic.Sxc.Engines;

/// <summary>
/// The foundation for engines - must be inherited by other engines
/// </summary>
[PrivateApi("used to be InternalApi_DoNotUse_MayChangeWithoutNotice, hidden in 17.08")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class EngineBase : ServiceBase<EngineBase.MyServices>, IEngine
{
    #region MyServices

    public class MyServices(
        IServerPaths serverPaths,
        IBlockResourceExtractor blockResourceExtractor,
        EngineCheckTemplate engineCheckTemplate,
        EnginePolymorphism enginePolymorphism,
        EngineAppRequirements engineAppRequirements)
        : MyServicesBase(connect:
            [serverPaths, blockResourceExtractor, engineCheckTemplate, engineAppRequirements, enginePolymorphism])
    {
        internal EngineAppRequirements EngineAppRequirements { get; } = engineAppRequirements;
        internal EnginePolymorphism EnginePolymorphism { get; } = enginePolymorphism;
        internal EngineCheckTemplate EngineCheckTemplate { get; } = engineCheckTemplate;
        public IServerPaths ServerPaths { get; } = serverPaths;
        internal IBlockResourceExtractor BlockResourceExtractor { get; } = blockResourceExtractor;
    }

    #endregion

    #region Constructor and DI

    [PrivateApi] protected IView Template = null!;
    [PrivateApi] protected string TemplatePath = null!;
    [PrivateApi] protected string? Edition;
    [PrivateApi] protected IApp App = null!;
    [PrivateApi] protected IDataSource DataSource = null!;
    [PrivateApi] protected IBlock Block = null!;

    /// <summary>
    /// Empty constructor, so it can be used in dependency injection
    /// </summary>
    protected EngineBase(MyServices services, object[]? connect = default)
        : base(services, $"{SxcLogName}.EngBas", connect: connect)
    { }

    #endregion

    public virtual void Init(IBlock block)
    {
        var l = Log.Fn();

        // Do various pre-checks and path variations
        var view = block.View!;
        var appReader = block.Context.AppReaderRequired;
        var appPathRootInInstallation = block.App.PathSwitch(view.IsShared, PathTypes.PhysRelative);
        var (polymorphPathOrNull, edition) = Services.EnginePolymorphism
            .PolymorphTryToSwitchPath(appPathRootInInstallation, view, appReader);

        var templatePath = polymorphPathOrNull
                           ?? Path.Combine(appPathRootInInstallation, view.Path).ToAbsolutePathForwardSlash();

        // Throw Exception if Template does not exist
        if (!File.Exists(Services.ServerPaths.FullAppPath(templatePath)))
            throw new RenderingException(new()
            {
                Name = "Template File Not Found",
                Detect = "",
                LinkCode = "err-template-not-found",
                UiMessage = $"The template file '{templatePath}' does not exist.",
            });

        // check common errors
        Services.EngineCheckTemplate.CheckExpectedTemplateErrors(view, appReader);

        // check access permissions - before initializing or running data-code in the template
        Services.EngineCheckTemplate.ThrowIfViewPermissionsDenyAccess(view, block.Context);

        // All ok, set properties
        Block = block;
        Template = view;
        Edition = edition;
        TemplatePath = templatePath;
        App = Block.App;
        DataSource = Block.Data;

        l.Done();
    }

    [PrivateApi]
    protected abstract (string Contents, List<Exception>? Exception) RenderEntryRazor(RenderSpecs specs);

    /// <inheritdoc />
    public virtual RenderEngineResult Render(RenderSpecs specs)
    {
        var l = Log.Fn<RenderEngineResult>(timer: true);
            
        // check if rendering is possible, or throw exceptions...
        var preFlightResult = CheckExpectedNoRenderConditions();
        if (preFlightResult != null)
            return l.Return(preFlightResult, $"error: {preFlightResult.ErrorCode}");

        var renderedTemplate = RenderEntryRazor(specs);
        var resourceExtractor = Services.BlockResourceExtractor;
        var result = resourceExtractor.Process(renderedTemplate.Contents);
        if (renderedTemplate.Exception != null)
            result = result with
            {
                // Note: not sure why the existing exceptions have precedence or are not mixed, but this is how the original code before 2025-03-17 was.
                ExceptionsOrNull = result.ExceptionsOrNull ?? renderedTemplate.Exception,
            };
        return l.ReturnAsOk(result);
    }

    private RenderEngineResult? CheckExpectedNoRenderConditions()
    {
        var l = Log.Fn<RenderEngineResult>();

        // Check App Requirements (new 16.08)
        var appReqProblems = Services.EngineAppRequirements
            .GetMessageForRequirements(Block.Context.AppReaderRequired);
        if (appReqProblems != null)
            return l.Return(appReqProblems, "error");

        if (Template.ContentType != "" && Template.ContentItem == null && Block.Configuration.Content.All(e => e == null))
        {
            var result = new RenderEngineResult
            {
                Html = EngineMessages.ToolbarForEmptyTemplate,
                ActivateJsApi = false,
                Assets = [],
                ErrorCode = null,
                ExceptionsOrNull = null, // changed from [], // TODO: NOT SURE If this is correct, I think it should be null?
            };
            return l.Return(result, "error");
        }

        return l.ReturnNull("all ok");
    }

}