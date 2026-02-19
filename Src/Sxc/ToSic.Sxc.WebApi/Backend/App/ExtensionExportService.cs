using System.IO.Compression;
using System.Text;
using System.Text.Json;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Models;
using ToSic.Eav.Persistence.File;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.ImportExport.Package.Sys;
using ToSic.Sys.Security.Encryption;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionExportService(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    LazySvc<ContentExportApi> contentExport,
    ExtensionManifestService manifestService
    )
    : ServiceBase("Bck.ExtExp", connect: [appReadersLazy, site, appPathSvc, contentExport, manifestService])
{
    /// <summary>
    /// ZIP file name format, with placeholders for name and version
    /// </summary>
    private const string ZipFileNameFormat = "app-extension-{0}-v{1}.zip";

    /// <summary>
    /// Default value for version if none is found in extension.json
    /// </summary>
    private const string DefaultVersion = "00.00.01";

    public FileToUploadToClient Export(int appId, string name)
    {
        var l = Log.Fn<FileToUploadToClient>($"export extension a#{appId}, name:'{name}'");

        if (string.IsNullOrWhiteSpace(name))
            throw l.Ex(new ArgumentException(@"Extension name is required", nameof(name)));

        // Basic validation to ensure folder is a simple directory name
        name = name.Trim();
        if (!ExtensionFolderNameValidator.IsValid(name))
            throw l.Ex(new ArgumentException(@"Invalid extension name", nameof(name)));

        // 1. Validate and get paths
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var extensionsRoot = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder);
        var extensionPath = ExtensionValidationHelper.GetActualCasedPath(extensionsRoot, name);

        if (!Directory.Exists(extensionPath))
            throw l.Ex(new DirectoryNotFoundException($"Extension folder not found: {name}"));
        l.A($"extension '{name}' folder found: '{extensionPath}'");

        var extensionManifestFile = manifestService.GetManifestFile(new(extensionPath));

        if (!extensionManifestFile.Exists)
            throw l.Ex(new FileNotFoundException($"{FolderConstants.AppExtensionJsonFile} not found in extension: {name}"));

        l.A($"{FolderConstants.AppExtensionJsonFile} found: '{extensionManifestFile.FullName}'");

        // 2. Read manifest
        var manifest = manifestService.LoadManifest(extensionManifestFile)
                       ?? throw l.Ex(new InvalidOperationException(
                           $"{FolderConstants.AppExtensionJsonFile} could not be loaded"));
        l.A($"{FolderConstants.AppExtensionJsonFile} loaded: version:'{manifest.Version}'");

        // 3. Determine all extensions to export (primary + bundled)
        // Note: ExtensionsBundled is stored as a comma-separated string (no spaces) in the manifest.
        var bundled = manifest.ExtensionsBundled
            .UseFallbackIfNoValue(string.Empty)
            .Split([','], StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(bundledName => ExtensionFolderNameValidator.IsValid(bundledName)
                ? bundledName
                : throw l.Ex(new ArgumentException($@"Invalid bundled extension name: {bundledName}", nameof(name))))
            .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var allExtensions = new[] { name }
            .Concat(bundled)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Old, less functional way:
        //var allExtensions = new List<string> { name };
        //foreach (var bundledName in bundled)
        //{
        //    if (string.IsNullOrWhiteSpace(bundledName))
        //        continue;

        //    var cleaned = bundledName.Trim();
        //    if (cleaned.Equals(name, StringComparison.OrdinalIgnoreCase))
        //        continue;
        //    if (!ExtensionFolderNameValidator.IsValid(cleaned))
        //        throw l.Ex(new ArgumentException($@"Invalid bundled extension name: {cleaned}", nameof(name)));
        //    if (allExtensions.Any(e => e.Equals(cleaned, StringComparison.OrdinalIgnoreCase)))
        //        continue;

        //    allExtensions.Add(cleaned);
        //}

        // 4. Build export specs for each extension
        // Primary extension must exist; bundled extensions are best-effort (skip missing)
        var exports = new List<ExtensionExportSpec>();
        foreach (var ext in allExtensions)
        {
            try
            {
                exports.Add(BuildExtensionExport(appId, ext, appPaths));
            }
            catch (Exception ex) when (ext != name && ex is DirectoryNotFoundException or FileNotFoundException)
            {
                l.A($"Skipping bundled extension '{ext}' because it wasn't found or is incomplete. Details: {ex.Message}");
            }
        }

        // 5. Create ZIP using Zipping helper (file name based on the primary extension)
        var primaryVersionString = exports.First().VersionString;
        var fileName = string.Format(ZipFileNameFormat, name, primaryVersionString);

        using var memoryStream = CreateZipArchive(exports, name, primaryVersionString);
        memoryStream.Position = 0;
        var fileBytes = memoryStream.ToArray();

        var totalFiles = exports.Sum(e => e.FilesToInclude.Count);
        l.A($"Created ZIP with {exports.Count} extensions and {totalFiles} files, size: {fileBytes.Length} bytes");

        return l.ReturnAsOk(new()
        {
            FileName = fileName,
            ContentType = MimeTypeConstants.FallbackType,
            FileBytes = fileBytes
        });
    }

    private sealed record ExtensionExportSpec(
        string ExtensionName,
        string VersionString,
        List<(string sourcePath, string zipPath)> FilesToInclude,
        List<(string sourcePath, string zipPath, string content)> Bundles,
        string ExtensionJsonZipPath,
        string ExtensionJsonContent,
        string LockJsonZipPath,
        string LockJsonContent
    );

    private JsonSerializerOptions JsonSerializationIndented => field ??= new(JsonOptions.UnsafeJsonWithoutEncodingHtml)
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private string ToNiceJson(object data) => JsonSerializer.Serialize(data, JsonSerializationIndented);

    private MemoryStream CreateZipArchive(
        List<ExtensionExportSpec> exports,
        string primaryExtensionName,
        string primaryVersionString)
    {
        var l = Log.Fn<MemoryStream>($"exts:{exports.Count}, primary:{primaryExtensionName}");

        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var zipping = new Zipping(l);

            // Add collected files using helper (deduplicate zip paths)
            //var allFiles = new List<(string sourcePath, string zipPath)>();
            //var addedZipPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            //foreach (var export in exports)
            //{
            //    foreach (var file in export.FilesToInclude)
            //    {
            //        if (!addedZipPaths.Add(file.zipPath))
            //            continue;
            //        allFiles.Add(file);
            //    }
            //}

            // 2026-02-09 2dm - simpler and functional
            var allFiles = exports
                .SelectMany(export => export.FilesToInclude)
                .DistinctBy(t => t.zipPath, StringComparer.OrdinalIgnoreCase)
                .ToList();

            zipping.AddFiles(archive, allFiles);
            l.A($"Added {allFiles.Count} files to ZIP");

            // Add extension.json + bundles + lock files for each extension
            foreach (var export in exports)
            {
                zipping.AddTextEntry(archive, export.ExtensionJsonZipPath, export.ExtensionJsonContent, new UTF8Encoding(false));

                foreach (var (_, zipPath, content) in export.Bundles)
                    zipping.AddTextEntry(archive, zipPath, content, new UTF8Encoding(false));

                zipping.AddTextEntry(archive, export.LockJsonZipPath, export.LockJsonContent, new UTF8Encoding(false));
            }

            // Create and add package-install.json listing all extensions
            var packageData = CreatePackageInstallFile(exports);
            var packageJson = ToNiceJson(packageData);
            zipping.AddTextEntry(archive, PackageInstallFile.FileName, packageJson, new UTF8Encoding(false));
        }

        return l.ReturnAsOk(memoryStream);
    }

    private ExtensionExportSpec BuildExtensionExport(int appId, string extensionName, IAppPaths appPaths)
    {
        var l = Log.Fn<ExtensionExportSpec>($"a#{appId}, ext:'{extensionName}'");

        var extensionsRoot = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder);
        var extensionPath = ExtensionValidationHelper.GetActualCasedPath(extensionsRoot, extensionName);
        var extensionDirectory = new DirectoryInfo(extensionPath);
        if (!extensionDirectory.Exists)
            throw l.Ex(new DirectoryNotFoundException($"Extension folder not found: {extensionName}"));

        // use actual casing from file system to preserve paths in the export
        extensionPath = extensionDirectory.FullName;
        var extensionFolderName = extensionDirectory.Name;

        var extensionManifestFile = manifestService.GetManifestFile(new(extensionPath));
        if (!extensionManifestFile.Exists)
            throw l.Ex(new FileNotFoundException($"{FolderConstants.AppExtensionJsonFile} not found in extension: {extensionName}"));

        var originalManifest = manifestService.LoadManifest(extensionManifestFile)
                              ?? throw l.Ex(new InvalidOperationException(
                                  $"{FolderConstants.AppExtensionJsonFile} could not be loaded"));

        // Decide if we should modify (for export / installation) or keep as-is
        // - if not installed -> modify (set isInstalled=true and expand release references)
        // - if installed -> keep exact json + lock (if present)
        var isInstalled = originalManifest.IsInstalled ?? false;
        var effectiveManifest = isInstalled
            ? originalManifest
            : ModifyExtensionManifest(originalManifest, appId);

        var versionString = effectiveManifest.Version.UseFallbackIfNoValue(DefaultVersion);
        var extensionDataPath = Path.Combine(extensionPath, FolderConstants.DataFolderProtected);

        // Read extension.json content (exact for installed extensions)
        var extensionJsonDiskPath = Path.Combine(extensionDataPath, FolderConstants.AppExtensionJsonFile);
        var extensionJsonContent = isInstalled
            ? File.ReadAllText(extensionJsonDiskPath)
            : ExtensionManifestSerializer.Serialize(effectiveManifest, JsonSerializationIndented);

        // Collect files to include (never include extension.json or package-index.json from disk)
        var filesToInclude = CollectFilesToInclude(extensionPath, effectiveManifest, appPaths, extensionFolderName);

        // Handle data bundles
        var bundles = ExportDataBundlesIfNeeded(effectiveManifest, extensionDataPath, extensionFolderName, appId);

        // Lock file content (exact for installed extensions if it exists)
        var basePath = $"{FolderConstants.AppExtensionsFolder}/{extensionFolderName}/{FolderConstants.DataFolderProtected}";
        var extensionJsonZipPath = $"{basePath}/{FolderConstants.AppExtensionJsonFile}";
        var lockJsonZipPath = $"{basePath}/{PackageIndexFile.LockFileName}";

        var lockJsonDiskPath = Path.Combine(extensionDataPath, PackageIndexFile.LockFileName);
        var lockJsonContent = isInstalled && File.Exists(lockJsonDiskPath)
            ? File.ReadAllText(lockJsonDiskPath)
            : ToNiceJson(CreatePackageIndexFile(filesToInclude, bundles, versionString, extensionJsonZipPath, extensionJsonContent));

        return l.ReturnAsOk(new ExtensionExportSpec(
            ExtensionName: extensionFolderName,
            VersionString: versionString,
            FilesToInclude: filesToInclude,
            Bundles: bundles,
            ExtensionJsonZipPath: extensionJsonZipPath,
            ExtensionJsonContent: extensionJsonContent,
            LockJsonZipPath: lockJsonZipPath,
            LockJsonContent: lockJsonContent
        ));
    }

    private ExtensionManifest ModifyExtensionManifest(ExtensionManifest manifest, int appId)
    {
        var l = Log.Fn<ExtensionManifest>();

        l.A($"Modifying {FolderConstants.AppExtensionJsonFile}");

        // Set isInstalled to true
        var modified = manifest with { IsInstalled = true };
        l.A("set isInstalled to true");

        // Process releases if they exist
        if (manifest.Releases.ValueKind != JsonValueKind.Array)
            return l.ReturnAsOk(modified);

        var releases = manifest.Releases.EnumerateArray().ToList();
        if (releases.Count <= 0)
            return l.ReturnAsOk(modified);

        var appReader = appReadersLazy.Value.Get(appId);

        l.A($"Processing {releases.Count} release references");
        var expandedReleases = releases
            .Select(releaseRef =>
            {
                var lr = Log.Fn<AppExtensionRelease>();
                if (releaseRef.ValueKind != JsonValueKind.String)
                    return lr.ReturnNull($"Skipping non-string release reference: {releaseRef.ValueKind}");

                var guidString = releaseRef.GetString();
                if (string.IsNullOrEmpty(guidString) || !Guid.TryParse(guidString, out var guid))
                    return lr.ReturnNull($"Skipping invalid release GUID: {guidString}");

                // Look up release entity from app data
                var releaseEntity = appReader.List.FirstOrDefault(e => e.EntityGuid == guid);

                return releaseEntity == null
                    ? lr.ReturnNull($"Release entity not found for GUID: {guid}")
                    : lr.Return(releaseEntity.As<AppExtensionRelease>()!, "found");
            })
            .Where(r => r != null)!
            .ToList();

        // Serialize expanded releases back to JsonElement
        var expandedJson = JsonSerializer.Serialize(expandedReleases, JsonSerializationIndented);
        using var expandedDoc = JsonDocument.Parse(expandedJson);
        var expandedElement = expandedDoc.RootElement.Clone();
        modified = modified with { Releases = expandedElement };

        l.A($"Expanded {expandedReleases.Count} releases");

        return l.ReturnAsOk(modified);
    }

    private List<(string sourcePath, string zipPath)> CollectFilesToInclude(
        string extensionPath,
        ExtensionManifest manifest,
        IAppPaths appPaths,
        string extensionName)
    {
        var l = Log.Fn<List<(string sourcePath, string zipPath)>>();

        l.A("Collecting files to include");

        // 1. Always include /extensions/[name] folder (except App_Data/extension.json and App_Data/package-index.json which we handle separately)
        var files = AddDirectoryFiles(extensionPath, extensionPath,
            $"{FolderConstants.AppExtensionsFolder}/{extensionName}",
            exclude:
            [
                $"{FolderConstants.DataFolderProtected}\\{FolderConstants.AppExtensionJsonFile}",
                $"{FolderConstants.DataFolderProtected}\\{PackageIndexFile.LockFileName}",
            ]);

        // 2. Check for hasAppCode setting
        if (manifest.AppCodeInside)
        {
            l.A($"Extension has AppCode, including AppCode/{FolderConstants.AppExtensionsFolder} folder");
            files = TryAddAppCodeFiles(files, appPaths, extensionName);

            // Also check for hyphen-less version of extension name for AppCode folder, since C# would have that convention
            files = TryAddAppCodeFiles(files, appPaths, ExtensionValidationHelper.AppCodeExtensionFolderName(extensionName));
        }

        l.A($"Collected {files.Count} files");
        return l.ReturnAsOk(files);
    }

    private List<(string, string)> TryAddAppCodeFiles(List<(string, string)> files, IAppPaths appPaths, string extensionName)
    {
        var appCodeExtensionsRoot = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder);
        var appCodeExtPath = ExtensionValidationHelper.GetActualCasedPath(appCodeExtensionsRoot, extensionName);
        var appCodeExtDir = new DirectoryInfo(appCodeExtPath);
        if (!appCodeExtDir.Exists)
            return files;

        var more = AddDirectoryFiles(appCodeExtPath, appCodeExtPath,
            $"{FolderConstants.AppCodeFolder}/{FolderConstants.AppExtensionsFolder}/{appCodeExtDir.Name}");
        return more.Any()
            ? files.Concat(more).ToList()
            : files;
    }

    private List<(string bundlePath, string zipFile, string fileContents)> ExportDataBundlesIfNeeded(
        ExtensionManifest manifest,
        string extensionDataPath,
        string extensionName,
        int appId)
    {
        var l = Log.Fn<List<(string, string, string)>>();

        if (!manifest.DataInside)
            return l.Return([], "no data bundles");

        var bundles = manifest.DataBundles;
        if (bundles.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return l.Return([], "dataBundles is null/undefined");

        // Convert to list of GUIDs
        var guidList = new List<Guid>();
        switch (bundles.ValueKind)
        {
            // TODO: WHY DO WE have two scenarios? I think the JSON should be standardized to always be an array.
            case JsonValueKind.String:
                // Single bundle as string
                var singleGuidStr = bundles.GetString();
                if (!string.IsNullOrEmpty(singleGuidStr) && Guid.TryParse(singleGuidStr, out var singleGuid))
                {
                    guidList = [singleGuid];
                    l.A($"Single bundle found: {singleGuid}");
                }
                break;
            case JsonValueKind.Array:
                // Array of bundles
                var dataToUse = bundles.EnumerateArray()
                    .Where(bundleRef => bundleRef.ValueKind == JsonValueKind.String)
                    .Select(bundleRef => bundleRef.GetString())
                    .Select(guidString => !string.IsNullOrEmpty(guidString) && Guid.TryParse(guidString, out var guid) ? guid : Guid.Empty)
                    .Where(guid => guid != Guid.Empty)
                    .ToList();

                guidList = guidList.Concat(dataToUse).ToList();

                // old, less functional way:
                //foreach (var bundleRef in bundles.EnumerateArray())
                //{
                //    if (bundleRef.ValueKind == JsonValueKind.String)
                //    {
                //        var guidString = bundleRef.GetString();
                //        if (!string.IsNullOrEmpty(guidString) && Guid.TryParse(guidString, out var guid))
                //            guidList.Add(guid);
                //    }
                //}
                l.A($"Found {guidList.Count} bundle references");
                break;
            default:
                l.A($"Unexpected bundles type: {bundles.ValueKind}, skipping");
                break;
        }

        if (guidList.Count == 0)
            return l.Return([], "no valid bundle GUIDs");

        l.A($"Exporting {guidList.Count} data bundles");
        var bundlesExport = contentExport.Value.Init(appId);
        var bundlesDir = Path.Combine(extensionDataPath, FolderConstants.DataSubFolderSystem,
            AppDataFoldersConstants.BundlesFolder);

        var files = guidList
            .Select(guid =>
            {
                try
                {
                    // Export bundle entity as JSON
                    var (export, fileContent) = bundlesExport.CreateBundleExport(guid, 2);
                    var bundlePath = Path.Combine(bundlesDir, export.FileName);
                    var zipPath = $"{FolderConstants.AppExtensionsFolder}/{extensionName}/" +
                                  $"{FolderConstants.DataFolderProtected}/{FolderConstants.DataSubFolderSystem}/" +
                                  $"{AppDataFoldersConstants.BundlesFolder}/{export.FileName}";
                    return (bundlePath, zipPath, fileContent);
                }
                catch (KeyNotFoundException ex)
                {
                    var wrapped = new KeyNotFoundException(
                        $"Data bundle '{guid}' referenced by extension '{extensionName}' was not found in app {appId}. " +
                        $"Update or remove the entry in extension.json:dataBundles. Details: {ex.Message}", ex);
                    l.Ex(wrapped);
                    throw wrapped;
                }
            })
            .ToList();

        // Old, non functional way:
        //foreach (var guid in guidList)
        //{
        //    try
        //    {
        //        // Export bundle entity as JSON
        //        var (export, fileContent) = bundlesExport.CreateBundleExport(guid, 2);
        //        var bundlePath = Path.Combine(bundlesDir, export.FileName);
        //        var zipPath =
        //            $"{FolderConstants.AppExtensionsFolder}/{extensionName}/{FolderConstants.DataFolderProtected}/{FolderConstants.DataSubFolderSystem}/{AppDataFoldersConstants.BundlesFolder}/{export.FileName}";
        //        files.Add((bundlePath, zipPath, fileContent));
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        var wrapped = new KeyNotFoundException(
        //            $"Data bundle '{guid}' referenced by extension '{extensionName}' was not found in app {appId}. " +
        //            $"Update or remove the entry in extension.json:dataBundles. Details: {ex.Message}", ex);
        //        l.Ex(wrapped);
        //        throw wrapped;
        //    }
        //}

        return l.ReturnAsOk(files);
    }

    private List<(string, string)> AddDirectoryFiles(string sourcePath, string baseSourcePath, string baseZipPath, string[]? exclude = null)
    {
        var l = Log.Fn<List<(string, string)>>($"source:{sourcePath}, base:{baseSourcePath}, zipBase:{baseZipPath}");

        if (!Directory.Exists(sourcePath))
            return l.Return([], "Directory doesn't exist, returning");

        var allFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
        l.A($"Found {allFiles.Length} files");

        var filePathList = allFiles
            .Select(file =>
            {
                // Use string manipulation instead of Path.GetRelativePath for .NET Framework compatibility
                var relativePath = file.StartsWith(baseSourcePath)
                    ? file.Substring(baseSourcePath.Length).TrimStart('\\', '/')
                    : file;

                // Check exclusions
                if (exclude != null && exclude.Any(f => relativePath.StartsWith(f, StringComparison.OrdinalIgnoreCase)))
                {
                    l.A($"Excluding: {relativePath}");
                    return (null!, null!);
                }

                var zipPath = Path.Combine(baseZipPath, relativePath).Replace("\\", "/");
                l.A($"Add file '{file}' to ZIP as '{zipPath}'");
                return (file, zipPath);
            })
            .Where(pair => pair.file != null)
            .ToList();

        return l.Return(filePathList!, $"Added {filePathList.Count} files to collection");
        
    }

    // Old, less functional way:
    //private void AddDirectoryFiles(string sourcePath, string baseSourcePath, string baseZipPath,
    //    List<(string, string)> files, string[]? exclude = null)
    //{
    //    var l = Log.Fn($"source:{sourcePath}, base:{baseSourcePath}, zipBase:{baseZipPath}");

    //    if (!Directory.Exists(sourcePath))
    //    {
    //        l.Done("Directory doesn't exist, returning");
    //        return;
    //    }

    //    var allFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
    //    l.A($"Found {allFiles.Length} files");

    //    foreach (var file in allFiles)
    //    {
    //        // Use string manipulation instead of Path.GetRelativePath for .NET Framework compatibility
    //        var relativePath = file.StartsWith(baseSourcePath)
    //            ? file.Substring(baseSourcePath.Length).TrimStart('\\', '/')
    //            : file;

    //        // Check exclusions
    //        if (exclude != null && exclude.Any(f => relativePath.StartsWith(f, StringComparison.OrdinalIgnoreCase)))
    //        {
    //            l.A($"Excluding: {relativePath}");
    //            continue;
    //        }

    //        var zipPath = Path.Combine(baseZipPath, relativePath).Replace("\\", "/");
    //        files.Add((file, zipPath));
    //        l.A($"Add file '{file}' to ZIP as '{zipPath}'");
    //    }

    //    l.Done($"Added {files.Count} files to collection");
    //}

    private PackageIndexFile CreatePackageIndexFile(
        List<(string sourcePath, string zipPath)> files,
        List<(string sourcePath, string zipPath, string content)> bundles,
        string version,
        string extensionJsonZipPath,
        string finalExtensionJson)
    {
        var l = Log.Fn<PackageIndexFile>($"Creating {PackageIndexFile.LockFileName} file");

        l.A($"{nameof(version)}:{version}");

        var fileList = files
            .Select(f => new PackageIndexFileEntry
            {
                File = "/" + f.zipPath,
                Hash = Sha256.Hash(File.ReadAllBytes(f.sourcePath))
            })
            .ToList();

        // Include bundles
        fileList.AddRange(
            bundles
                .Select(b => new PackageIndexFileEntry
                {
                    File = "/" + b.zipPath,
                    Hash = Sha256.Hash(b.content)
                })
        );

        // Also add the (final) extension.json hash
        fileList.Add(new()
        {
            File = $"/{extensionJsonZipPath}",
            Hash = Sha256.Hash(finalExtensionJson)
        });

        l.A($"{PackageIndexFile.LockFileName} file created with {fileList.Count} entries");

        return l.ReturnAsOk(new()
        {
            Version = version,
            Files = fileList
        });
    }

    private PackageInstallFile CreatePackageInstallFile(List<ExtensionExportSpec> exports)
    {
        var l = Log.Fn<PackageInstallFile>();

        // Create the package object
        var package = new PackageInstallFile
        {
            Header = new()
            {
                PackageType = PackageTypes.AppExtension,
            },
            About = new()
            {
                Title = "2sxc App Extension (standalone)",
                Description = "A standalone app extension, exported directly from 2sxc",
            },
            Extensions =
            [
                .. exports.Select(e => new PackageInstallExtension(
                    Name: e.ExtensionName,
                    DefinitionFile: e.ExtensionJsonZipPath,
                    IndexFile: e.LockJsonZipPath,
                    IndexFileHash: Sha256.Hash(e.LockJsonContent)
                )),
            ],
        };

        return l.Return(package, $"Created package object for {exports.Count} extensions");
    }

}
