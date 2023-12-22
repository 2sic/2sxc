using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using static ToSic.Sxc.Blocks.BlockBuildingConstants;
using IApp = ToSic.Sxc.Apps.IApp;
using IDataSource = ToSic.Eav.DataSource.IDataSource;

namespace ToSic.Sxc.Engines;

/// <summary>
/// The foundation for engines - must be inherited by other engines
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class EngineBase : ServiceBase<EngineBase.MyServices>, IEngine
{
    #region MyServices

    public class MyServices : MyServicesBase
    {
        internal EngineAppRequirements EngineAppRequirements { get; }
        internal EnginePolymorphism EnginePolymorphism { get; }
        internal EngineCheckTemplate EngineCheckTemplate { get; }

        public MyServices(IServerPaths serverPaths,
            IBlockResourceExtractor blockResourceExtractor,
            EngineCheckTemplate engineCheckTemplate,
            EnginePolymorphism enginePolymorphism,
            EngineAppRequirements engineAppRequirements
        )
        {
            ConnectServices(
                ServerPaths = serverPaths,
                BlockResourceExtractor = blockResourceExtractor,
                EngineCheckTemplate = engineCheckTemplate,
                EngineAppRequirements = engineAppRequirements,
                EnginePolymorphism = enginePolymorphism
            );
        }

        internal IServerPaths ServerPaths { get; }
        internal IBlockResourceExtractor BlockResourceExtractor { get; }

    }

    #endregion

    #region Constructor and DI

    [PrivateApi] protected IView Template;
    [PrivateApi] protected string TemplatePath;
    [PrivateApi] protected IApp App;
    [PrivateApi] protected IDataSource DataSource;
    [PrivateApi] protected IBlock Block;

    /// <summary>
    /// Empty constructor, so it can be used in dependency injection
    /// </summary>
    protected EngineBase(MyServices services) : base(services, $"{Constants.SxcLogName}.EngBas") { }

    #endregion

    public virtual void Init(IBlock block)
    {
        var l = Log.Fn();

        // Do various pre-checks and path variations
        var view = block.View;
        var appState = block.Context.AppState;
        var appPathRootInInstallation = block.App.PathSwitch(view.IsShared, PathTypes.PhysRelative);
        var polymorphPathOrNull = Services.EnginePolymorphism.PolymorphTryToSwitchPath(appPathRootInInstallation, view, appState);
        var templatePath = polymorphPathOrNull ??
                           Path.Combine(appPathRootInInstallation, view.Path).ToAbsolutePathForwardSlash();

        // Throw Exception if Template does not exist
        if (!File.Exists(Services.ServerPaths.FullAppPath(templatePath)))
            throw new RenderingException(new CodeHelp(name: "Template File Not Found", detect: "",
                linkCode: "err-template-not-found", uiMessage: $"The template file '{templatePath}' does not exist."));

        // check common errors
        Services.EngineCheckTemplate.CheckExpectedTemplateErrors(view, appState);

        // check access permissions - before initializing or running data-code in the template
        Services.EngineCheckTemplate.CheckTemplatePermissions(view, block.Context);

        // All ok, set properties
        Block = block;
        Template = view;
        TemplatePath = templatePath;
        App = Block.App;
        DataSource = Block.Data;

        l.Done();
    }

    [PrivateApi]
    protected abstract (string Contents, List<Exception> Exception) RenderImplementation(object data);

    /// <inheritdoc />
    public virtual RenderEngineResult Render(object data)
    {
        var l = Log.Fn<RenderEngineResult>(timer: true);
            
        // check if rendering is possible, or throw exceptions...
        var preFlightResult = CheckExpectedNoRenderConditions();
        if (preFlightResult != null) return l.Return(preFlightResult, $"error: {preFlightResult.ErrorCode}");

        var renderedTemplate = RenderImplementation(data);
        var depMan = Services.BlockResourceExtractor;
        var result = depMan.Process(renderedTemplate.Contents);
        if (renderedTemplate.Exception != null)
            result = new RenderEngineResult(result, exsOrNull: renderedTemplate.Exception);
        return l.ReturnAsOk(result);
    }

    private RenderEngineResult CheckExpectedNoRenderConditions()
    {
        var l = Log.Fn<RenderEngineResult>();

        // Check App Requirements (new 16.08)
        var appReqProblems = Services.EngineAppRequirements.GetMessageForAppRequirements(Block.Context.AppState);
        if (appReqProblems != null) return l.Return(appReqProblems, "error");


        if (Template.ContentType != "" && Template.ContentItem == null && Block.Configuration.Content.All(e => e == null))
        {
            Exception ex = new ExceptionWithHelp(new CodeHelp(name: ErrorDataIsMissing, detect: "", linkCode: "err-block-data-missing"));
            var result = new RenderEngineResult(EngineMessages.ToolbarForEmptyTemplate, false, null, ErrorDataIsMissing,
                ex.ToListOfOne());
            return l.Return(result, "error");
        }

        return l.ReturnNull("all ok");
    }

}