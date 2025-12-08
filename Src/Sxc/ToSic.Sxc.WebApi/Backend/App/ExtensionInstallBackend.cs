using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sys.Configuration;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInstallBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    IGlobalConfiguration globalConfiguration,
    ExtensionManifestService manifestService,
    LazySvc<ExtensionInspectBackend> inspectorLazy,
    LazySvc<CodeControllerReal> codeLazy)
    : ServiceBase("Bck.ExtZip", connect: [appReadersLazy, site, appPathSvc, globalConfiguration, manifestService, inspectorLazy, codeLazy])
{
    private ReadOnlyFileHelper ReadOnlyHelper => field ??= new(Log);
    private ExtensionValidationHelper Validation => field ??= new(manifestService, Log);
    private ExtensionExtractionHelper Extraction => field ??= new(appReadersLazy, site, appPathSvc, globalConfiguration, Validation, Log);
    private ExtensionInstallHelper Copier => field ??= new(ReadOnlyHelper, Log);
    private ExtensionPreflightHelper Preflight => field ??= new(manifestService, inspectorLazy, Log);

    public bool InstallExtensionZip(int appId, Stream zipStream, bool overwrite = false, string? originalZipFileName = null, string editions = null!)
    {
        var l = Log.Fn<bool>($"a:{appId}, overwrite:{overwrite}, ofn:'{originalZipFileName}'");

        string? tempDir = null;
        try
        {
            var prep = Extraction.PrepareExtraction(appId, zipStream, editions);
            tempDir = prep.TempDir;
            if (!prep.Success)
                throw new InvalidOperationException(prep.Error ?? "error");

            var appRoot = prep.AppRoot;
            var editionList = prep.Editions;
            var extensionsRoot = Path.Combine(appRoot, FolderConstants.AppExtensionsFolder);
            Directory.CreateDirectory(extensionsRoot);
            var manifestResults = prep.ManifestResults;

            var installed = new List<string>();

            foreach (var lockResult in prep.LockResults)
            {
                var folderName = lockResult.Key;
                var lockValidation = lockResult.Value;

                l.A($"prepare install:'{folderName}'");

                if (!manifestResults.TryGetValue(folderName, out var manifestValidation))
                    throw new InvalidOperationException($"missing manifest info for '{folderName}'");

                var editionSupportError = EnsureEditionsSupported(manifestValidation.EditionsSupported, editionList);
                if (editionSupportError != null)
                    throw new InvalidOperationException(editionSupportError);

                foreach (var edition in editionList)
                {
                    var editionRoot = ExtensionEditionHelper.GetEditionRoot(appRoot, edition);

                    var editionExtensionsRoot = Path.Combine(editionRoot, FolderConstants.AppExtensionsFolder);
                    Directory.CreateDirectory(editionExtensionsRoot);

                    var installResult = Copier.InstallSingleExtension(
                        folderName: folderName,
                        lockValidation: lockValidation,
                        tempDir: tempDir,
                        extensionsRoot: editionExtensionsRoot,
                        appRoot: editionRoot,
                        overwrite: overwrite);

                    if (!installResult.Success)
                        throw new InvalidOperationException(installResult.Error ?? $"install failed:'{folderName}'");
                }

                installed.Add(folderName);
            }
            return l.ReturnTrue($"installed '{string.Join("','", installed)}' from '{originalZipFileName}'");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            throw;
        }
        finally
        {
            if (tempDir != null)
                Zipping.TryToDeleteDirectory(tempDir, l);
        }
    }

    public PreflightResultDto InstallPreflight(int appId, Stream zipStream, string? originalZipFileName = null, string editions = null!)
    {
        var l = Log.Fn<PreflightResultDto>($"a:{appId}, ofn:'{originalZipFileName}'");

        string? tempDir = null;
        try
        {
            var prep = Extraction.PrepareExtraction(appId, zipStream, editions);
            tempDir = prep.TempDir;
            if (!prep.Success)
                throw new InvalidOperationException(prep.Error ?? "error");

            var appRoot = prep.AppRoot;
            var requestedEditions = prep.Editions;
            var availableEditions = codeLazy.Value.GetEditions(appId).Editions.Select(e => e.Name).ToList();
            var lockResults = prep.LockResults;
            var manifestResults = prep.ManifestResults;

            var result = new PreflightResultDto();

            foreach (var kvp in lockResults)
            {
                var folderName = kvp.Key;
                var lockValidation = kvp.Value;

                if (!manifestResults.TryGetValue(folderName, out var manifestValidation) || manifestValidation.Manifest == null)
                    throw new InvalidOperationException($"missing manifest info for '{folderName}'");

                var manifest = manifestValidation.Manifest;
                var editionSupportError = EnsureEditionsSupported(manifestValidation.EditionsSupported, requestedEditions);
                if (editionSupportError != null)
                    throw new InvalidOperationException(editionSupportError);

                var editionTargets = ExtensionEditionHelper.MergeEditions(requestedEditions.ToList(), ExtensionEditionHelper.DetectInstalledEditions(appRoot, availableEditions, folderName));
                var allEditionTargets = ExtensionEditionHelper.MergeEditions(editionTargets, availableEditions);
                var extDto = new PreflightExtensionDto
                {
                    Name = folderName,
                    Version = manifest.Version,
                    EditionsSupported = manifestValidation.EditionsSupported,
                    FileCount = lockValidation.AllowedFiles?.Count ?? 0,
                    Features = Preflight.MapFeatures(manifest)
                };

                foreach (var edition in allEditionTargets)
                {
                    var editionInfo = Preflight.BuildEditionInfo(appId, appRoot, folderName, edition, manifest);
                    if (editionInfo is not null)
                    {
                        extDto.Editions.Add(editionInfo);
                        continue;
                    }

                    var editionRoot = ExtensionEditionHelper.GetEditionRoot(appRoot, edition);
                    var extensionExists = Directory.Exists(Path.Combine(editionRoot, FolderConstants.AppExtensionsFolder, folderName));
                    if (extensionExists)
                        continue;

                    extDto.Editions.Add(new ExtensionEditionDto
                    {
                        Edition = edition,
                        IsInstalled = false
                    });
                }

                result.Extensions.Add(extDto);
            }

            return l.Return(result, $"extensions:{result.Extensions.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            throw;
        }
        finally
        {
            if (tempDir != null)
                Zipping.TryToDeleteDirectory(tempDir, l);
        }
    }

    private static string? EnsureEditionsSupported(bool editionsSupported, string[] editionList)
        => editionList.Any(e => e.HasValue()) && !editionsSupported
            ? "extension does not support editions"
            : null;
}
