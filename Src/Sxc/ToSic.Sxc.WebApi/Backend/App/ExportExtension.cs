using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Persistence.File;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sys.Security.Encryption;
using ToSic.Sys.Utils;

#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using Microsoft.AspNetCore.Mvc;
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExportExtension(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    LazySvc<ContentExportApi> contentExport)
    : ServiceBase("Bck.ExtExp", connect: [appReadersLazy, site, appPathSvc])
{
    /// <summary>
    /// ZIP file name format, with placeholders for name and version
    /// </summary>
    private const string ZipFileNameFormat = "app-extension-{0}-v{1}.zip";

    /// <summary>
    /// Default value for version if none is found in extension.json
    /// </summary>
    private const string DefaultVersion = "00.00.01";

    public THttpResponseType Export(int zoneId, int appId, string name)
    {
        var l = Log.Fn<THttpResponseType>($"export extension z#{zoneId}, a#{appId}, name:'{name}'");

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

        var extensionDataPath = Path.Combine(extensionPath, FolderConstants.DataFolderProtected);
        var extensionJsonPath = Path.Combine(extensionDataPath, FolderConstants.AppExtensionJsonFile);

        if (!File.Exists(extensionJsonPath))
            throw l.Ex(new FileNotFoundException($"{FolderConstants.AppExtensionJsonFile} not found in extension: {name}"));
        l.A($"{FolderConstants.AppExtensionJsonFile} found: '{extensionJsonPath}'");

        // 2. Read and modify extension.json
        var extensionJson = JsonNode.Parse(File.ReadAllText(extensionJsonPath)) as JsonObject
            ?? throw l.Ex(new InvalidOperationException($"{FolderConstants.AppExtensionJsonFile} is not a valid JSON object"));
        l.A($"{FolderConstants.AppExtensionJsonFile} parsed");
        l.A($"\n{ToNiceJson(extensionJson)}\n");

        var modifiedJson = ModifyExtensionJson(extensionJson, appId);
        l.A($"\n{ToNiceJson(modifiedJson)}\n");

        // 3. Collect files to include
        var filesToInclude = CollectFilesToInclude(extensionPath, modifiedJson, appPaths, name);

        // 4. Handle data bundles
        var bundles = new List<(string sourcePath, string zipPath, string content)>();
        var hasDataBundles = extensionJson.TryGetPropertyValue("hasDataBundles", out var hasDataBundlesNode)
                             && hasDataBundlesNode?.GetValue<bool>() == true;
        if (hasDataBundles && modifiedJson.TryGetPropertyValue("bundles", out var bundlesNode))
        {
            // Convert bundlesNode to JsonArray, handling both string and array cases
            JsonArray bundlesArray;
            switch (bundlesNode)
            {
                case JsonValue jsonValue when jsonValue.TryGetValue<string>(out var singleBundle):
                    // Single bundle as string - convert to array
                    bundlesArray = [singleBundle];
                    l.A($"Single bundle found, converted to array: {singleBundle}");
                    break;
                case JsonArray existingArray:
                    // Already an array
                    bundlesArray = existingArray;
                    break;
                default:
                    // Unexpected type
                    l.A($"Unexpected bundles type: {bundlesNode?.GetType().Name}, skipping");
                    bundlesArray = [];
                    break;
            }

            if (bundlesArray.Count > 0)
            {
                l.A($"Exporting {bundlesArray.Count} data bundles");
                bundles = ExportDataBundles(bundlesArray, extensionDataPath, name, appId);
            }
        }

        // 5. Create ZIP using Zipping helper
        var version = (modifiedJson["version"]?.GetValue<string>())
            .UseFallbackIfNoValue(DefaultVersion);
        var fileName = string.Format(ZipFileNameFormat, name, version);
        
        using var memoryStream = CreateZipArchive(filesToInclude, bundles, modifiedJson, name);
        memoryStream.Position = 0;
        var fileBytes = memoryStream.ToArray();

        l.A($"Created ZIP with {filesToInclude.Count} files, size: {fileBytes.Length} bytes");

#if NETFRAMEWORK

        return l.ReturnAsOk(HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, MimeTypeConstants.FallbackType, new MemoryStream(fileBytes)));
#else
        return l.ReturnAsOk(new FileContentResult(fileBytes, MimeTypeConstants.FallbackType) { FileDownloadName = fileName });
#endif
    }

    private JsonSerializerOptions JsonSerializationIndented => field ??= new() { WriteIndented = true };
    private string ToNiceJson(object data) => JsonSerializer.Serialize(data, JsonSerializationIndented);

    private MemoryStream CreateZipArchive(
        List<(string sourcePath, string zipPath)> filesToInclude,
        List<(string sourcePath, string zipPath, string content)> bundles,
        JsonObject extensionJson, 
        string extensionName)
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
            var modifiedJsonPath = $"{basePath}/{FolderConstants.AppExtensionJsonFile}";
            var modifiedJsonText = ToNiceJson(extensionJson);
            zipping.AddTextEntry(archive, modifiedJsonPath, modifiedJsonText, new UTF8Encoding(false));
            l.A($"Added modified {FolderConstants.AppExtensionJsonFile} to ZIP");

            // Handle data bundles
            foreach (var (_, zipPath, content) in bundles)
            {
                zipping.AddTextEntry(archive, zipPath, content, new UTF8Encoding(false));
                l.A($"Added data bundle to ZIP: {zipPath}");
            }

            // Create and add extension.lock file
            var lockData = CreateLockObject(filesToInclude, bundles, extensionJson);
            var lockPath = $"{basePath}/{FolderConstants.AppExtensionLockJsonFile}";
            var lockJson = ToNiceJson(lockData);
            zipping.AddTextEntry(archive, lockPath, lockJson, new UTF8Encoding(false));
            l.A($"Added {FolderConstants.AppExtensionLockJsonFile} lock file to ZIP");
        }

        return l.ReturnAsOk(memoryStream);
    }

    private JsonObject ModifyExtensionJson(JsonObject json, int appId)
    {
        var l = Log.Fn<JsonObject>();

        l.A($"Modifying {FolderConstants.AppExtensionJsonFile}");
        
        // Set isInstalled to true
        json["isInstalled"] = true;
        l.A("set isInstalled to true");

        // Process releases if they exist
        if (json.TryGetPropertyValue("releases", out var releasesNode) && releasesNode is JsonArray releases)
        {
            var appReader = appReadersLazy.Value.Get(appId);

            l.A($"Processing {releases.Count} release references");
            var newReleases = new JsonArray();

            foreach (var releaseRef in releases)
            {
                var guidString = releaseRef?.GetValue<string>();
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
                var releaseObj = new JsonObject
                {
                    ["version"] = releaseEntity.Get<string>("Version") ?? DefaultVersion,
                    ["breaking"] = releaseEntity.Get<bool>("Breaking"),
                    ["notes"] = releaseEntity.Get<string>("Notes") ?? ""
                };
                
                newReleases.Add(releaseObj);
            }

            json["releases"] = newReleases;
            l.A($"Expanded {newReleases.Count} releases");
        }

        return l.ReturnAsOk(json);
    }

    private List<(string sourcePath, string zipPath)> CollectFilesToInclude(
        string extensionPath,
        JsonObject extensionJson,
        IAppPaths appPaths,
        string extensionName)
    {
        var l = Log.Fn<List<(string sourcePath, string zipPath)>>();

        l.A("Collecting files to include");

        var files = new List<(string, string)>();

        // 1. Always include /extensions/[name] folder (except App_Data which we handle separately)
        AddDirectoryFiles(extensionPath, extensionPath, $"{FolderConstants.AppExtensionsFolder}/{extensionName}", files, exclude: [$"{FolderConstants.DataFolderProtected}\\{FolderConstants.AppExtensionJsonFile}"]);

        // 2. Check for hasAppCode setting
        var hasAppCode = extensionJson.TryGetPropertyValue("hasAppCode", out var hasAppCodeNode) && hasAppCodeNode?.GetValue<bool>() == true;
        if (hasAppCode)
        {
            l.A($"Extension has AppCode, including AppCode/{FolderConstants.AppExtensionsFolder} folder");
            var appCodeExtPath = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, extensionName);
            if (Directory.Exists(appCodeExtPath)) 
                AddDirectoryFiles(appCodeExtPath, appCodeExtPath, $"{FolderConstants.AppCodeFolder}/{FolderConstants.AppExtensionsFolder}/{extensionName}", files);
        }

        l.A($"Collected {files.Count} files");
        return l.ReturnAsOk(files);
    }

    private List<(string bundlePath, string zipFile, string fileContents)> ExportDataBundles(JsonArray bundles, string extensionDataPath, string extensionName, int appId)
    {
        var l = Log.Fn<List<(string, string, string)>>();
        
        var bundlesExport = contentExport.Value.Init(appId);
        var files = new List<(string, string, string)>();
        var bundlesDir = Path.Combine(extensionDataPath, FolderConstants.DataSubFolderSystem, AppDataFoldersConstants.BundlesFolder);

        foreach (var bundleRef in bundles)
        {
            var guidString = bundleRef?.GetValue<string>();
            if (string.IsNullOrEmpty(guidString) || !Guid.TryParse(guidString, out var guid))
            {
                l.A($"Skipping invalid bundle GUID: {guidString}");
                continue;
            }

            // Export bundle entity as JSON
            var (export, fileContent) = bundlesExport.CreateBundleExport(guid, 2);
            var bundlePath = Path.Combine(bundlesDir, export.FileName);
            var zipPath = $"extensions/{extensionName}/{FolderConstants.DataFolderProtected}/{FolderConstants.DataSubFolderSystem}/{AppDataFoldersConstants.BundlesFolder}/{export.FileName}";
            files.Add((bundlePath, zipPath, fileContent));
        }

        return l.ReturnAsOk(files);
    }

    private void AddDirectoryFiles(string sourcePath, string baseSourcePath, string baseZipPath, List<(string, string)> files, string[]? exclude = null)
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

    private object CreateLockObject(List<(string sourcePath, string zipPath)> files, List<(string sourcePath, string zipPath, string content)> bundles, JsonObject extensionJson)
    {
        var l = Log.Fn<object>();
        l.A($"Creating {FolderConstants.AppExtensionLockJsonFile} file");

        var version = extensionJson["version"]?.GetValue<string>() ?? "1.0.0";
        l.A($"{nameof(version)}:{version}");
        
        var fileList = files.Select(f => new
        {
            file = "/" + f.zipPath,
            hash = Sha256.Hash(File.ReadAllText(f.sourcePath))
        }).ToList();

        // Include bundles
        fileList.AddRange(
            bundles.Select(b => new
            {
                file = "/" + b.zipPath,
                hash = Sha256.Hash(b.content)
            }).ToList()
            );

        // Also add the modified extension.json hash
        fileList.Add(new
        {
            file = $"/{FolderConstants.AppExtensionsFolder}/{Path.GetFileName(Path.GetDirectoryName(files.FirstOrDefault().zipPath) ?? "")}/{FolderConstants.DataFolderProtected}/{FolderConstants.AppExtensionJsonFile}",
            hash = Sha256.Hash(extensionJson.ToJsonString())
        });

        l.A($"{FolderConstants.AppExtensionLockJsonFile} lock file created with {fileList.Count} entries");

        return l.ReturnAsOk(new
        {
            version,
            files = fileList
        });
    }
}
