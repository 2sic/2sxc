using ToSic.Eav.Apps.Sys.Extensions;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Services;
using ToSic.Eav.Sys;
using ToSic.Sxc.Services;
using AppEditionsDataSource = ToSic.Sxc.DataSources.AppEditions;

namespace ToSic.Sxc.Apps.Extensions.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionReaderBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    LazySvc<IJsonService> jsonLazy,
    ExtensionManifestService manifestService,
    IDataSourceGenerator<AppEditionsDataSource> appEditions)
    : ServiceBase("Bck.ExtRead", connect: [appReadersLazy, site, appPathSvc, jsonLazy, manifestService, appEditions])
{
    private const string IconFileName = "icon.png";

    public List<ExtensionRaw> GetExtensions(int appId)
    {
        var l = Log.Fn<List<ExtensionRaw>>($"a#{appId}");
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        HashSet<string> editionNames =
        [
            with(StringComparer.OrdinalIgnoreCase),
            string.Empty,
            .. AvailableEditionNames(appId)
        ];

        var availableEditions = editionNames
            .OrderBy(name => name.IsEmpty() ? 0 : 1)
            .ThenBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Log.A($"available editions: {string.Join(", ", availableEditions.Select(EditionLabel))}");

        var list = new List<ExtensionRaw>();
        // 2026-08-20 2dm - seems unused, commented out; #ToRemoveV24
        //var primaryManifests = new Dictionary<string, ExtensionManifest>(StringComparer.OrdinalIgnoreCase);
        //var primaryInputTypes = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

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
                var manifestFile = ExtensionManifestService.GetManifestFileInfo(dir);
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

                // 2026-08-20 2dm - seems unused, commented out; #ToRemoveV24
                //var inputTypeInside = ReadInputType(manifestFile);

                if (editionName.IsEmpty())
                {
                    list.Add(new ExtensionRaw
                    {
                        Folder = folderName,
                        Edition = editionName,
                        Configuration = configuration,
                        Icon = GetIconUrl(appPaths, editionName, dir)
                    });

                    // 2026-08-20 2dm - seems unused, commented out; #ToRemoveV24
                    //primaryManifests[folderName] = configuration;
                    //primaryInputTypes[folderName] = inputTypeInside;
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

                list.Add(new ExtensionRaw
                {
                    Folder = folderName,
                    Edition = editionName,
                    Configuration = configuration,
                    Icon = GetIconUrl(appPaths, editionName, dir)
                });
                Log.A($"registered edition '{EditionLabel(editionName)}' for extension '{folderName}'");
            }
        }
        Log.A($"extensions discovered: {list.Count}");
        return l.ReturnAsOk(list);

        string EditionLabel(string editionName) => editionName.IsEmpty() ? "(primary)" : editionName;
    }

    private ICollection<string> AvailableEditionNames(int appId)
        => appEditions
            .New(new DataSourceOptions { AppIdentityOrReader = appReadersLazy.Value.AppIdentity(appId) })
            .List
            .Select(edition => edition.Get<string>("Name"))
            .Where(name => name != null)
            .Select(name => name!)
            .ToListOpt();

    // 2026-08-20 2dm - seems unused, commented out; #ToRemoveV24
    //private static string? ReadInputType(FileInfo manifestFile)
    //{
    //    if (!manifestFile.Exists)
    //        return null;

    //    using var json = JsonDocument.Parse(File.ReadAllText(manifestFile.FullName));
    //    return json.RootElement.TryGetProperty("inputTypeInside", out var inputType)
    //        && inputType.ValueKind == JsonValueKind.String
    //        ? inputType.GetString()
    //        : null;
    //}

    private static string GetIconUrl(IAppPaths appPaths, string editionName, string extensionDir)
    {
        var iconPath = Path.Combine(extensionDir, IconFileName);
        if (!File.Exists(iconPath))
            return "";

        var folderName = Path.GetFileName(extensionDir);
        var relativePath = BuildRelativeIconPath(appPaths, editionName, folderName);
        return relativePath;
    }

    private static string BuildRelativeIconPath(IAppPaths appPaths, string editionName, string folderName)
    {
        var baseRelative = appPaths.RelativePath.ForwardSlash().Trim('/');

        var parts = new List<string>();
        if (!baseRelative.IsEmpty())
            parts.Add(baseRelative);
        if (!editionName.IsEmpty())
            parts.Add(editionName);

        parts.Add(FolderConstants.AppExtensionsFolder);
        parts.Add(folderName);
        parts.Add(IconFileName);

        var relativePath = string.Join("/", parts)
            .TrimPrefixSlash()
            .PrefixSlash();

        return relativePath;
    }
}
