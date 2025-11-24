using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Services;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionReaderBackend(
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
        // Also load as manifest to check for editions support
        var configuration = manifestService.LoadManifest(extensionManifestFile)
            ?? new ExtensionManifest();

        // Check for editions if manifest says they're supported
        Dictionary<string, Admin.ExtensionEditionDto>? editions = null;
        if (configuration.EditionsSupported)
        {
            editions = DetectEditions(appRootPath, folderName, configuration);
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
    private Dictionary<string, Admin.ExtensionEditionDto>? DetectEditions(string appRootPath, string extensionFolderName, ExtensionManifest primaryManifest)
    {
        var l = Log.Fn<Dictionary<string, Admin.ExtensionEditionDto>?>($"extension:'{extensionFolderName}'");
        
        var appRoot = new DirectoryInfo(appRootPath);
        var editions = new Dictionary<string, Admin.ExtensionEditionDto>(StringComparer.OrdinalIgnoreCase);

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

            editions[editionFolder.Name] = new Admin.ExtensionEditionDto
            {
                Folder = editionFolder.Name,
                Configuration = editionManifest
            };
        }

        return l.Return(editions.Count > 0 ? editions : null, $"found:{editions.Count}");
    }
}
