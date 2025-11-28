using System.Text.Json;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Builds preflight information for extension zip installations.
/// </summary>
internal class ExtensionPreflightHelper(
    ExtensionManifestService manifestService,
    LazySvc<ExtensionInspectBackend> inspectorLazy,
    ILog? parentLog)
    : HelperBase(parentLog, "Bck.ExtPre")
{
    internal ExtensionEditionDto? BuildEditionInfo(int appId, string appRoot, string extensionName, string edition, ExtensionManifest incomingManifest)
    {
        var l = Log.Fn<ExtensionEditionDto>();

        var editionRoot = ExtensionEditionHelper.GetEditionRoot(appRoot, edition);

        l.A($"prep edition:'{edition}', root:'{editionRoot}', ext:'{extensionName}'");

        if (!Directory.Exists(Path.Combine(editionRoot, FolderConstants.AppExtensionsFolder, extensionName)))
            return l.ReturnNull($"extension not found:'{extensionName}'");

        var installedManifest = LoadInstalledManifest(editionRoot, extensionName);
        if (installedManifest is null)
            return l.Return(new ExtensionEditionDto
            {
                Edition = edition
            }, $"extension without manifest:'{extensionName}'");

        var currentVersion = installedManifest.Version;
        var isInstalled = installedManifest.IsInstalled == true;

        var inspectEdition = inspectorLazy.Value.Inspect(appId, extensionName, edition.HasValue() ? edition : null);
        var hasFileChanges = inspectEdition.FoundLock && inspectEdition.Summary != null
            && (inspectEdition.Summary.Changed > 0 || inspectEdition.Summary.Added > 0 || inspectEdition.Summary.Missing > 0);
        var hasData = inspectEdition.Data?.ContentTypes.Any(ct => ct.LocalEntities > 0) == true;
        var breakingChanges = HasBreakingChanges(incomingManifest, currentVersion);

        l.A($"state installed:{isInstalled}, ver:'{currentVersion}', changes:{hasFileChanges}, data:{hasData}, breaking:{breakingChanges}");

        return l.ReturnAsOk(new ExtensionEditionDto
        {
            Edition = edition,
            IsInstalled = isInstalled,
            CurrentVersion = currentVersion,
            HasFileChanges = hasFileChanges,
            HasData = hasData,
            BreakingChanges = breakingChanges
        });
    }

    internal ExtensionFeaturesDto MapFeatures(ExtensionManifest manifest)
        => new()
            {
                FieldsInside = manifest.HasFields,
                RazorInside = manifest.HasRazor,
                AppCodeInside = manifest.HasAppCode,
                WebApiInside = manifest.HasWebApi,
                ContentTypesInside = manifest.HasContentTypes,
                DataBundlesInside = manifest.HasDataBundles,
                QueriesInside = manifest.HasQueries,
                ViewsInside = manifest.HasViews,
                DataInside = manifest.DataInside,
                InputTypeInside = manifest.InputTypeInside.HasValue()
            };

    private bool HasBreakingChanges(ExtensionManifest incomingManifest, string? currentVersion)
    {
        if (incomingManifest.Releases.ValueKind != JsonValueKind.Array)
            return false;

        var current = new Version();
        var hasCurrent = currentVersion.HasValue() && Version.TryParse(currentVersion, out current);

        foreach (var release in incomingManifest.Releases.EnumerateArray())
        {
            if (release.ValueKind != JsonValueKind.Object)
                continue;

            var breaking = release.TryGetProperty("breaking", out var breakingProp)
                           && breakingProp.ValueKind == JsonValueKind.True;
            if (!breaking)
                continue;

            if (!hasCurrent)
                return false;

            if (release.TryGetProperty("version", out var versionProp)
                && versionProp.ValueKind == JsonValueKind.String
                && Version.TryParse($"{versionProp}", out var releaseVersion))
            {
                if (releaseVersion > current)
                    return true;
            }
        }

        return false;
    }

    private ExtensionManifest? LoadInstalledManifest(string editionRoot, string extensionName)
    {
        var manifestPath = Path.Combine(editionRoot, FolderConstants.AppExtensionsFolder, extensionName, FolderConstants.DataFolderProtected, FolderConstants.AppExtensionJsonFile);
        return File.Exists(manifestPath)
            ? manifestService.LoadManifest(new FileInfo(manifestPath))
            : null;
    }
}
