using System.IO.Compression;
using System.Text;
using System.Text.Json;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Persistence.File;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.ImportExport.Package.Sys;
using ToSic.Sys.Security.Encryption;
using ToSic.Sys.Utils;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#endif

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionExportService(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    LazySvc<ContentExportApi> contentExport,
    ExtensionManifestService manifestService
    )
    : ServiceBase("Bck.ExtExp", connect: [appReadersLazy, site, appPathSvc, manifestService])
{
    /// <summary>
    /// ZIP file name format, with placeholders for name and version
    /// </summary>
    private const string ZipFileNameFormat = "app-extension-{0}-v{1}.zip";

    /// <summary>
    /// Default value for version if none is found in extension.json
    /// </summary>
    private const string DefaultVersion = "00.00.01";

    public THttpResponseType Export(int appId, string name)
    {
        var l = Log.Fn<THttpResponseType>($"export extension a#{appId}, name:'{name}'");

        if (string.IsNullOrWhiteSpace(name))
            throw l.Ex(new ArgumentException(@"Extension name is required", nameof(name)));

        // Basic validation to ensure folder is a simple directory name
        name = name.Trim();
        if (!ExtensionFolderNameValidator.IsValid(name))
            throw l.Ex(new ArgumentException(@"Invalid extension name", nameof(name)));

        // 1. Validate and get paths
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var extensionPath = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder, name);

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

        // 3. Modify manifest for export
        var modifiedManifest = ModifyExtensionManifest(manifest, appId);
        l.A($"Modified manifest: version:'{modifiedManifest.Version}', isInstalled:{modifiedManifest.IsInstalled}");

        // compute extension data path under the extension folder (protected data folder)
        var extensionDataPath = Path.Combine(extensionPath, FolderConstants.DataFolderProtected);

        // 4. Collect files to include
        var filesToInclude = CollectFilesToInclude(extensionPath, modifiedManifest, appPaths, name);

        // 5. Handle data bundles
        var bundles = ExportDataBundlesIfNeeded(modifiedManifest, extensionDataPath, name, appId);

        // 6. Create ZIP using Zipping helper
        var versionString = modifiedManifest.Version.UseFallbackIfNoValue(DefaultVersion);
        var fileName = string.Format(ZipFileNameFormat, name, versionString);

        using var memoryStream = CreateZipArchive(filesToInclude, bundles, modifiedManifest, name, versionString);
        memoryStream.Position = 0;
        var fileBytes = memoryStream.ToArray();

        l.A($"Created ZIP with {filesToInclude.Count} files, size: {fileBytes.Length} bytes");

#if NETFRAMEWORK

        return l.ReturnAsOk(HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, MimeTypeConstants.FallbackType,
            new MemoryStream(fileBytes)));
#else
        return l.ReturnAsOk(new FileContentResult(fileBytes, MimeTypeConstants.FallbackType) { FileDownloadName =
 fileName });
#endif
    }

    private JsonSerializerOptions JsonSerializationIndented => field ??= new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private string ToNiceJson(object data) => JsonSerializer.Serialize(data, JsonSerializationIndented);

    private MemoryStream CreateZipArchive(List<(string sourcePath, string zipPath)> filesToInclude,
        List<(string sourcePath, string zipPath, string content)> bundles,
        ExtensionManifest manifest,
        string extensionName,
        string versionString)
    {
        var l = Log.Fn<MemoryStream>($"files:{filesToInclude.Count}, ext:{extensionName}");

        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var zipping = new Zipping(l);

            // Add collected files using helper
            zipping.AddFiles(archive, filesToInclude);
            l.A($"Added {filesToInclude.Count} files to ZIP");

            // Add modified extension.json
            var basePath = $"{FolderConstants.AppExtensionsFolder}/{extensionName}/{FolderConstants.DataFolderProtected}";
            var extensionJsonPath = $"{basePath}/{FolderConstants.AppExtensionJsonFile}";
            var finalJsonString = ExtensionManifestSerializer.Serialize(manifest, JsonSerializationIndented);
            zipping.AddTextEntry(archive, extensionJsonPath, finalJsonString, new UTF8Encoding(false));
            l.A($"Added modified {FolderConstants.AppExtensionJsonFile} to ZIP");

            // Handle data bundles
            foreach (var (_, zipPath, content) in bundles)
            {
                zipping.AddTextEntry(archive, zipPath, content, new UTF8Encoding(false));
                l.A($"Added data bundle to ZIP: {zipPath}");
            }

            // Create and add extension.lock file
            var lockData = CreateLockObject(filesToInclude, bundles, versionString, finalJsonString);
            var lockJsonPath = $"{basePath}/{PackageIndexFile.LockFileName}";
            var lockJson = ToNiceJson(lockData);
            zipping.AddTextEntry(archive, lockJsonPath, lockJson, new UTF8Encoding(false));

            // Create and add 2sxc-package.json file
            var packageData = CreatePackageObject(extensionName, extensionJsonPath, lockJsonPath, lockJson);
            var packageJson = ToNiceJson(packageData);
            zipping.AddTextEntry(archive, PackageInstallFile.FileName, packageJson, new UTF8Encoding(false));

            l.A($"Added {PackageIndexFile.LockFileName} lock file to ZIP");
        }

        return l.ReturnAsOk(memoryStream);
    }


    private ExtensionManifest ModifyExtensionManifest(ExtensionManifest manifest, int appId)
    {
        var l = Log.Fn<ExtensionManifest>();

        l.A($"Modifying {FolderConstants.AppExtensionJsonFile}");

        // Set isInstalled to true
        var modified = manifest with { IsInstalled = true };
        l.A("set isInstalled to true");

        // Process releases if they exist
        if (manifest.Releases.ValueKind == JsonValueKind.Array)
        {
            var releases = manifest.Releases.EnumerateArray().ToList();
            if (releases.Count > 0)
            {
                var appReader = appReadersLazy.Value.Get(appId);

                l.A($"Processing {releases.Count} release references");
                var expandedReleases = new List<object>();

                foreach (var releaseRef in releases)
                {
                    if (releaseRef.ValueKind != JsonValueKind.String)
                    {
                        l.A($"Skipping non-string release reference: {releaseRef.ValueKind}");
                        continue;
                    }

                    var guidString = releaseRef.GetString();
                    if (string.IsNullOrEmpty(guidString) || !Guid.TryParse(guidString, out var guid))
                    {
                        l.A($"Skipping invalid release GUID: {guidString}");
                        continue;
                    }

                    // Look up release entity from app data
                    var releaseEntity = appReader.List.FirstOrDefault(e => e.EntityGuid == guid);

                    if (releaseEntity == null)
                    {
                        l.A($"Release entity not found for GUID: {guid}");
                        continue;
                    }

                    // Create release object
                    var releaseObj = new
                    {
                        version = releaseEntity.Get<string>("Version") ?? DefaultVersion,
                        breaking = releaseEntity.Get<bool>("Breaking"),
                        notes = releaseEntity.Get<string>("Notes") ?? ""
                    };

                    expandedReleases.Add(releaseObj);
                }

                // Serialize expanded releases back to JsonElement
                var expandedJson = JsonSerializer.Serialize(expandedReleases, JsonSerializationIndented);
                using var expandedDoc = JsonDocument.Parse(expandedJson);
                var expandedElement = expandedDoc.RootElement.Clone();
                modified = modified with { Releases = expandedElement };

                l.A($"Expanded {expandedReleases.Count} releases");
            }
        }

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

        var files = new List<(string, string)>();

        // 1. Always include /extensions/[name] folder (except App_Data which we handle separately)
        AddDirectoryFiles(extensionPath, extensionPath, $"{FolderConstants.AppExtensionsFolder}/{extensionName}", files,
            exclude: [$"{FolderConstants.DataFolderProtected}\\{FolderConstants.AppExtensionJsonFile}"]);

        // 2. Check for hasAppCode setting
        if (manifest.AppCodeInside)
        {
            l.A($"Extension has AppCode, including AppCode/{FolderConstants.AppExtensionsFolder} folder");
            var appCodeExtPath = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppCodeFolder,
                FolderConstants.AppExtensionsFolder, extensionName);
            if (Directory.Exists(appCodeExtPath))
                AddDirectoryFiles(appCodeExtPath, appCodeExtPath,
                    $"{FolderConstants.AppCodeFolder}/{FolderConstants.AppExtensionsFolder}/{extensionName}", files);
        }

        l.A($"Collected {files.Count} files");
        return l.ReturnAsOk(files);
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
        if (bundles.ValueKind == JsonValueKind.Undefined || bundles.ValueKind == JsonValueKind.Null)
            return l.Return([], "dataBundles is null/undefined");

        // Convert to list of GUIDs
        var guidList = new List<Guid>();
        switch (bundles.ValueKind)
        {
            case JsonValueKind.String:
                // Single bundle as string
                var singleGuidStr = bundles.GetString();
                if (!string.IsNullOrEmpty(singleGuidStr) && Guid.TryParse(singleGuidStr, out var singleGuid))
                {
                    guidList.Add(singleGuid);
                    l.A($"Single bundle found: {singleGuid}");
                }
                break;
            case JsonValueKind.Array:
                // Array of bundles
                foreach (var bundleRef in bundles.EnumerateArray())
                {
                    if (bundleRef.ValueKind == JsonValueKind.String)
                    {
                        var guidString = bundleRef.GetString();
                        if (!string.IsNullOrEmpty(guidString) && Guid.TryParse(guidString, out var guid))
                            guidList.Add(guid);
                    }
                }
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
        var files = new List<(string, string, string)>();
        var bundlesDir = Path.Combine(extensionDataPath, FolderConstants.DataSubFolderSystem,
            AppDataFoldersConstants.BundlesFolder);

        foreach (var guid in guidList)
        {
            // Export bundle entity as JSON
            var (export, fileContent) = bundlesExport.CreateBundleExport(guid, 2);
            var bundlePath = Path.Combine(bundlesDir, export.FileName);
            var zipPath =
                $"{FolderConstants.AppExtensionsFolder}/{extensionName}/{FolderConstants.DataFolderProtected}/{FolderConstants.DataSubFolderSystem}/{AppDataFoldersConstants.BundlesFolder}/{export.FileName}";
            files.Add((bundlePath, zipPath, fileContent));
        }

        return l.ReturnAsOk(files);
    }

    private void AddDirectoryFiles(string sourcePath, string baseSourcePath, string baseZipPath,
        List<(string, string)> files, string[]? exclude = null)
    {
        var l = Log.Fn($"source:{sourcePath}, base:{baseSourcePath}, zipBase:{baseZipPath}");

        if (!Directory.Exists(sourcePath))
        {
            l.Done("Directory doesn't exist, returning");
            return;
        }

        var allFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
        l.A($"Found {allFiles.Length} files");

        foreach (var file in allFiles)
        {
            // Use string manipulation instead of Path.GetRelativePath for .NET Framework compatibility
            var relativePath = file.StartsWith(baseSourcePath)
                ? file.Substring(baseSourcePath.Length).TrimStart('\\', '/')
                : file;

            // Check exclusions
            if (exclude != null && exclude.Any(f => relativePath.StartsWith(f, StringComparison.OrdinalIgnoreCase)))
            {
                l.A($"Excluding: {relativePath}");
                continue;
            }

            var zipPath = Path.Combine(baseZipPath, relativePath).Replace("\\", "/");
            files.Add((file, zipPath));
            l.A($"Add file '{file}' to ZIP as '{zipPath}'");
        }

        l.Done($"Added {files.Count} files to collection");
    }

    private object CreateLockObject(List<(string sourcePath, string zipPath)> files,
        List<(string sourcePath, string zipPath, string content)> bundles, string version, string finalExtensionJson)
    {
        var l = Log.Fn<object>($"Creating {PackageIndexFile.LockFileName} file");

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

        // Also add the modified extension.json hash
        var path = Path.GetFileName(Path.GetDirectoryName(files.FirstOrDefault().zipPath) ?? "");
        fileList.Add(new()
        {
            File = $"/{FolderConstants.AppExtensionsFolder}/{path}/{FolderConstants.DataFolderProtected}/{FolderConstants.AppExtensionJsonFile}",
            Hash = Sha256.Hash(finalExtensionJson)
        });

        l.A($"{PackageIndexFile.LockFileName} file created with {fileList.Count} entries");

        return l.ReturnAsOk(new PackageIndexFile
        {
            Version = version,
            Files = fileList
        });
    }

    private PackageInstallFile CreatePackageObject(string extensionName, string extensionJsonPath, string lockPath, string lockJson)
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
                new(extensionName, extensionJsonPath, lockPath, Sha256.Hash(lockJson)),
            ],
        };

        return l.Return(package, $"Created package object for {extensionName}");
    }

}
