using System.Text.Json.Nodes;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Services;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsReaderBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    LazySvc<IJsonService> jsonLazy,
    ExtensionManifestService manifestService)
    : ServiceBase("Bck.ExtRead", connect: [appReadersLazy, site, appPathSvc, jsonLazy, manifestService])
{
    public ExtensionsResultDto GetExtensions(int appId)
    {
        var l = Log.Fn<ExtensionsResultDto>($"a#{appId}");
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var root = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder);

        var list = new List<ExtensionDto>();
        if (Directory.Exists(root))
        {
            foreach (var dir in Directory.GetDirectories(root))
            {
                var folderName = Path.GetFileName(dir);
                var extensionDto = BuildExtensionDto(dir, folderName, appPaths.PhysicalPath);
                list.Add(extensionDto);
            }
        }
        return l.ReturnAsOk(new ExtensionsResultDto { Extensions = list });
    }

    /// <summary>
    /// Build an ExtensionDto for a single extension folder.
    /// </summary>
    private ExtensionDto BuildExtensionDto(string extensionPath, string folderName, string appRootPath)
    {
        var l = Log.Fn<ExtensionDto>($"folder:'{folderName}'");
        
        var extensionManifestFile = manifestService.GetManifestFile(new(extensionPath));
        
        object? configuration = null;
        ExtensionManifest? manifest = null;
        
        try
        {
            if (extensionManifestFile.Exists)
            {
                var json = File.ReadAllText(extensionManifestFile.FullName);
                configuration = jsonLazy.Value.ToObject(json);
                
                // Also load as manifest to check for editions support
                manifest = manifestService.LoadManifest(extensionManifestFile);
            }
        }
        catch (Exception ex)
        {
            l.Ex(ex);
        }

        configuration ??= new JsonObject();

        // Check for editions if manifest says they're supported
        Dictionary<string, ExtensionEditionDto>? editions = null;
        if (manifest?.EditionsSupported == true)
        {
            editions = DetectEditions(appRootPath, folderName, manifest);
            if (editions?.Count > 0)
                l.A($"Found {editions.Count} editions for extension '{folderName}'");
        }

        var result = new ExtensionDto
        {
            Folder = folderName,
            Configuration = configuration,
            Editions = editions
        };
        
        return l.Return(result, $"folder:'{folderName}', hasEditions:{editions != null}");
    }

    /// <summary>
    /// Detect and build edition information for an extension.
    /// </summary>
    private Dictionary<string, ExtensionEditionDto>? DetectEditions(string appRootPath, string extensionFolderName, ExtensionManifest primaryManifest)
    {
        var l = Log.Fn<Dictionary<string, ExtensionEditionDto>?>($"extension:'{extensionFolderName}'");
        
        var appRoot = new DirectoryInfo(appRootPath);
        var editions = new Dictionary<string, ExtensionEditionDto>(StringComparer.OrdinalIgnoreCase);

        // Look for edition folders at the app root level (e.g., /staging, /live, /dev)
        foreach (var editionFolder in appRoot.GetDirectories())
        {
            // Skip the primary extensions folder
            if (editionFolder.Name.Equals(FolderConstants.AppExtensionsFolder, StringComparison.OrdinalIgnoreCase))
                continue;

            // Check if this edition folder has a matching extensions subfolder
            var editionExtensionsPath = Path.Combine(editionFolder.FullName, FolderConstants.AppExtensionsFolder);
            if (!Directory.Exists(editionExtensionsPath))
                continue;

            // Check if the specific extension exists in this edition
            var editionExtensionPath = Path.Combine(editionExtensionsPath, extensionFolderName);
            if (!Directory.Exists(editionExtensionPath))
                continue;

            // Load the edition manifest
            var editionManifestFile = manifestService.GetManifestFile(new DirectoryInfo(editionExtensionPath));
            if (!editionManifestFile.Exists)
                continue;

            var editionManifest = manifestService.LoadManifest(editionManifestFile);
            if (editionManifest?.InputTypeInside.IsEmpty() ?? true)
                continue;

            // Ensure the edition manifest references the same input type
            if (!editionManifest.InputTypeInside.Equals(primaryManifest.InputTypeInside, StringComparison.OrdinalIgnoreCase))
            {
                l.A($"Edition {editionFolder.Name} has mismatched inputTypeInside: {editionManifest.InputTypeInside} != {primaryManifest.InputTypeInside}");
                continue;
            }

            // Load the edition configuration
            object? editionConfiguration = null;
            try
            {
                var json = File.ReadAllText(editionManifestFile.FullName);
                editionConfiguration = jsonLazy.Value.ToObject(json);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
            }

            editionConfiguration ??= new JsonObject();

            editions[editionFolder.Name] = new ExtensionEditionDto
            {
                Folder = editionFolder.Name,
                Configuration = editionConfiguration
            };
        }

        return l.Return(editions.Count > 0 ? editions : null, $"found:{editions.Count}");
    }
}
