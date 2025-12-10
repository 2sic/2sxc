using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Services;
using ToSic.Sys.Utils;
using System.Text.Json;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionReaderBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    LazySvc<IJsonService> jsonLazy,
    ExtensionManifestService manifestService,
    LazySvc<CodeControllerReal> codeLazy)
    : ServiceBase("Bck.ExtRead", connect: [appReadersLazy, site, appPathSvc, jsonLazy, manifestService, codeLazy])
{
    public ExtensionsResultDto GetExtensions(int appId)
    {
        var l = Log.Fn<ExtensionsResultDto>($"a#{appId}");
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var editionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            string.Empty
        };

        foreach (var edition in codeLazy.Value.GetEditions(appId).Editions.Select(e => e.Name))
            editionNames.Add(edition);

        var availableEditions = editionNames
            .OrderBy(name => name.IsEmpty() ? 0 : 1)
            .ThenBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Log.A($"available editions: {string.Join(", ", availableEditions.Select(EditionLabel))}");

        var list = new List<ExtensionDto>();
        var primaryManifests = new Dictionary<string, ExtensionManifest>(StringComparer.OrdinalIgnoreCase);
        var primaryInputTypes = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var editionName in availableEditions)
        {
            var editionExtensionsDir = Path.Combine(appPaths.PhysicalPath, editionName.IsEmpty()
                ? FolderConstants.AppExtensionsFolder
                : Path.Combine(editionName, FolderConstants.AppExtensionsFolder));
            if (!Directory.Exists(editionExtensionsDir))
            {
                Log.A($"edition {EditionLabel(editionName)} has no extensions folder at '{editionExtensionsDir}'");
                continue;
            }

            foreach (var dir in Directory.GetDirectories(editionExtensionsDir))
            {
                var folderName = Path.GetFileName(dir);
                var manifestFile = manifestService.GetManifestFile(new(dir));
                //if (!manifestFile.Exists)
                //{
                //    Log.A($"skip extension '{folderName}' in {EditionLabel(editionName)} because manifest file missing");
                //    continue;
                //}

                var configuration = manifestService.LoadManifest(manifestFile) ?? new();
                //if (configuration == null)
                //{
                //    Log.A($"skip extension '{folderName}' in {EditionLabel(editionName)} because manifest couldn't be parsed");
                //    continue;
                //}

                var inputTypeInside = ReadInputType(manifestFile);

                if (editionName.IsEmpty())
                {
                    list.Add(new ExtensionDto
                    {
                        Folder = folderName,
                        Edition = editionName,
                        Configuration = configuration
                    });

                    primaryManifests[folderName] = configuration;
                    primaryInputTypes[folderName] = inputTypeInside;
                    Log.A($"registered primary extension '{folderName}', supports editions: {configuration?.EditionsSupported}");
                    continue;
                }

                //if (!primaryManifests.TryGetValue(folderName, out var primaryManifest) || !primaryManifest.EditionsSupported)
                //{
                //    Log.A($"skip edition '{EditionLabel(editionName)}' for '{folderName}' because primary manifest missing or not edition-enabled");
                //    continue;
                //}

                //if (!primaryInputTypes.TryGetValue(folderName, out var primaryInputType))
                //{
                //    Log.A($"skip edition '{EditionLabel(editionName)}' for '{folderName}' because base input type was not recorded");
                //    continue;
                //}

                //if (inputTypeInside.IsEmpty())
                //{
                //    Log.A($"skip edition '{EditionLabel(editionName)}' for '{folderName}' because edition manifest lacks inputTypeInside");
                //    continue;
                //}

                //if (!string.Equals(primaryInputType, inputTypeInside, StringComparison.OrdinalIgnoreCase))
                //{
                //    Log.A($"skip edition '{EditionLabel(editionName)}' for '{folderName}' due to inputType mismatch '{inputTypeInside}' != '{primaryInputType}'");
                //    continue;
                //}

                list.Add(new ExtensionDto
                {
                    Folder = folderName,
                    Edition = editionName,
                    Configuration = configuration
                });
                Log.A($"registered edition '{EditionLabel(editionName)}' for extension '{folderName}'");
            }
        }
        Log.A($"extensions discovered: {list.Count}");
        return l.ReturnAsOk(new ExtensionsResultDto { Extensions = list });

        string EditionLabel(string editionName) => editionName.IsEmpty() ? "(primary)" : editionName;
    }

    // TODO: @STV - WARNING - THIS CODE LOOKS EXTREMELY SIMILAR TO AppFileSystemInputTypesLoader.BuildUiAssets
    // PLS CHECK AGAIN TO AVOID DUPLICATE CODE

    /// <summary>
    /// Detect and build edition information for an extension.
    /// </summary>
    private List<ExtensionDto> DetectEditions(string appRootPath, string extensionFolderName, ExtensionManifest primaryManifest)
    {
        var l = Log.Fn<List<ExtensionDto>>($"extension:'{extensionFolderName}'");
        
        var appRoot = new DirectoryInfo(appRootPath);
        var editions = new List<ExtensionDto>();

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
            //if (editionManifest?.InputTypeInside.IsEmpty() ?? true)
            if (editionManifest?.InputFieldInside ?? true)
                continue;

            // Ensure the edition manifest references the same input type
            //if (!editionManifest.InputTypeInside.Equals(primaryManifest.InputTypeInside, StringComparison.OrdinalIgnoreCase))
            if (editionManifest.InputFieldInside != primaryManifest.InputFieldInside)
            {
                //l.A($"Edition {editionFolder.Name} has mismatched inputTypeInside: {editionManifest.InputTypeInside} != {primaryManifest.InputTypeInside}");
                l.A($"Edition {editionFolder.Name} has mismatched inputTypeInside: {editionManifest.InputFieldInside} != {primaryManifest.InputFieldInside}");
                continue;
            }

            editions.Add(new()
            {
                Folder = extensionFolderName,
                Edition = editionFolder.Name,
                Configuration = editionManifest
            });
        }

        return l.Return(editions, $"found:{editions.Count}");
    }

    private static string? ReadInputType(FileInfo manifestFile)
    {
        if (!manifestFile.Exists)
            return null;

        using var json = JsonDocument.Parse(File.ReadAllText(manifestFile.FullName));
        return json.RootElement.TryGetProperty("inputTypeInside", out var inputType)
            && inputType.ValueKind == JsonValueKind.String
            ? inputType.GetString()
            : null;
    }
}
