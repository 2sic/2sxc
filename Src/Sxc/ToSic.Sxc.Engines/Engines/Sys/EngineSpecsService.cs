using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Engines.Sys;

public class EngineSpecsService(
    IServerPaths serverPaths,
    EnginePolymorphism enginePolymorphism,
    EngineCheckTemplate engineCheckTemplate,
    LazySvc<IAppJsonConfigurationService> appJsonService
) : ServiceBase("Sxc.EgSpec", connect: [serverPaths, enginePolymorphism, engineCheckTemplate, appJsonService])
{
    /// <summary>
    /// Do various preflight checks and create the Engine Specs according to the information in the BlockSpecs
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    /// <exception cref="RenderingException"></exception>
    public EngineSpecs GetSpecs(IBlock block)
    {
        var l = Log.Fn<EngineSpecs>();

        // Do various pre-checks and path variations
        var view = block.View!;
        var appReader = block.Context.AppReaderRequired;
        var appPathRootInInstallation = block.App.PathSwitch(view.IsShared, PathTypes.PhysRelative);
        var (polymorphPathOrNull, edition) = enginePolymorphism
            .PolymorphTryToSwitchPath(appPathRootInInstallation, view, appReader);

        // if edition is empty get edition from AppJsonConfiguration
        if (edition.IsEmpty())
        {
            var appJson = appJsonService.Value.GetAppJson(block.App.AppId);
            if (appJson?.Editions?.Count > 0)
            {
                var defaultOrFirstEdition = appJson.Editions.OrderByDescending(e => e.Value.IsDefault).FirstOrDefault();
                if (defaultOrFirstEdition.Key != null)
                {
                    edition = defaultOrFirstEdition.Key;
                    l.A($"Using edition from app.json: {edition}, isDefault:{defaultOrFirstEdition.Value.IsDefault}");

                    // Recalculate the template path with the edition
                    var editionPath = Path.Combine(appPathRootInInstallation, edition, view.Path).ToAbsolutePathForwardSlash();
                    if (File.Exists(serverPaths.FullAppPath(editionPath)))
                    {
                        polymorphPathOrNull = editionPath;
                        l.A($"Found template in edition path: {editionPath}");
                    }
                }
            }
        }

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
