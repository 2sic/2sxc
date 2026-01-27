using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Engines.Sys;

public class EngineSpecsService(
    IServerPaths serverPaths,
    EnginePolymorphism enginePolymorphism,
    EngineCheckTemplate engineCheckTemplate
) : ServiceBase("Sxc.EgSpec", connect: [serverPaths, enginePolymorphism, engineCheckTemplate])
{
    public EngineSpecs GetSpecs(IBlock block)
    {
        var l = Log.Fn<EngineSpecs>();

        // Do various pre-checks and path variations
        var view = block.View!;
        var appReader = block.Context.AppReaderRequired;
        var appPathRootInInstallation = block.App.PathSwitch(view.IsShared, PathTypes.PhysRelative);
        var (polymorphPathOrNull, edition) = enginePolymorphism
            .PolymorphTryToSwitchPath(appPathRootInInstallation, view, appReader);

        var templatePath = polymorphPathOrNull
                           ?? Path.Combine(appPathRootInInstallation, view.Path).ToAbsolutePathForwardSlash();

        // Throw Exception if Template does not exist
        if (!File.Exists(serverPaths.FullAppPath(templatePath)))
            throw new RenderingException(new()
            {
                Name = "Template File Not Found",
                Detect = "",
                LinkCode = "err-template-not-found",
                UiMessage = $"The template file '{templatePath}' does not exist.",
            });

        // check common errors
        engineCheckTemplate.CheckExpectedTemplateErrors(view, appReader);

        // check access permissions - before initializing or running data-code in the template
        engineCheckTemplate.ThrowIfViewPermissionsDenyAccess(view, block.Context);

        // All ok, set properties
        var engineSpecs = new EngineSpecs()
        {
            App = block.App,
            Block = block,
            DataSource = block.Data,
            Edition = edition,
            TemplatePath = templatePath,
            View = view,
        };

        return l.Return(engineSpecs);
    }
}
