using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Sys;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Prepares extraction and validation of extension zip packages.
/// </summary>
internal class ExtensionExtractionHelper(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    IGlobalConfiguration globalConfiguration,
    ExtensionValidationHelper validation,
    ILog? parentLog)
    : HelperBase(parentLog, "Bck.ExtPrep")
{
    internal ExtensionExtractionResult PrepareExtraction(int appId, Stream zipStream, string[]? editions)
    {
        var l = Log.Fn<ExtensionExtractionResult>($"prep a:{appId}");

        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var appRoot = appPaths.PhysicalPath;
        var editionList = ExtensionEditionHelper.NormalizeEditions(editions).ToArray();

        var tempDir = Path.Combine(globalConfiguration.TemporaryFolder(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            new Zipping(Log).ExtractZipStream(zipStream, tempDir, allowCodeImport: true);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            throw new InvalidOperationException("invalid zip", ex);
        }

        var extensionsDir = Path.Combine(tempDir, FolderConstants.AppExtensionsFolder);
        if (!Directory.Exists(extensionsDir))
            return l.Return(new(false, $"zip missing top-level '{FolderConstants.AppExtensionsFolder}' folder", tempDir, appRoot, editionList, new(), new()));

        var candidateDirs = Directory.GetDirectories(extensionsDir, "*", SearchOption.TopDirectoryOnly);
        if (candidateDirs.Length == 0)
            return l.Return(new(false, $"'{FolderConstants.AppExtensionsFolder}' folder empty", tempDir, appRoot, editionList, new(), new()));

        var (error, lockResults, manifestResults) = validation.ValidateCandidateSubfolders(tempDir, candidateDirs);
        if (error != null)
            return l.Return(new(false, error, tempDir, appRoot, editionList, lockResults, manifestResults));

        return l.Return(new(true, null, tempDir, appRoot, editionList, lockResults, manifestResults));
    }
}
