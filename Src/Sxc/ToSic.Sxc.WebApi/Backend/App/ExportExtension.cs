using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Services;
using ToSic.Sys.Security.Encryption;
using ToSic.Sys.Users;
using EavJsonSerializer = ToSic.Eav.ImportExport.Json.Sys.JsonSerializer;
using ToSic.Sxc.Data;



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
    LazySvc<IJsonService> jsonLazy,
    IUser user,
    IResponseMaker responseMaker,
    Generator<EavJsonSerializer> jsonSerializerGenerator)
    : ServiceBase("Bck.ExtExp", connect: [appReadersLazy, site, appPathSvc, jsonLazy, user, responseMaker, jsonSerializerGenerator])
{
    public THttpResponseType Export(int zoneId, int appId, string name)
    {
        var l = Log.Fn<THttpResponseType>($"export extension z#{zoneId}, a#{appId}, name:'{name}'");

        if (string.IsNullOrWhiteSpace(name))
            throw l.Ex(new ArgumentException("Extension name is required", nameof(name)));
        
        // Basic validation to ensure folder is a simple directory name
        name = name.Trim();
        if (!ExtensionsBackend.IsValidFolderName(name))
            throw l.Ex(new ArgumentException("Invalid extension name", nameof(name)));

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
        l.A($"\n{extensionJson.ToJsonString()}\n");

        var modifiedJson = ModifyExtensionJson(extensionJson, appId, extensionDataPath, l);
        l.A($"\n{modifiedJson.ToJsonString()}\n");

        // 3. Collect files to include
        var filesToInclude = CollectFilesToInclude(extensionPath, extensionDataPath, modifiedJson, appPaths, name, l);

        // 4. Create ZIP using Zipping helper
        var version = modifiedJson["version"]?.GetValue<string>() ?? "1.0.0";
        var fileName = $"{name}_{version}.zip";
        
        using var memoryStream = CreateZipArchive(filesToInclude, modifiedJson, name, l);
        memoryStream.Position = 0;
        var fileBytes = memoryStream.ToArray();
        var mimeType = MimeTypeConstants.FallbackType;

        l.A($"Created ZIP with {filesToInclude.Count} files, size: {fileBytes.Length} bytes");

#if NETFRAMEWORK
        return l.ReturnAsOk(HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, mimeType, new MemoryStream(fileBytes)));
#else
        return l.ReturnAsOk(new FileContentResult(fileBytes, mimeType) { FileDownloadName = fileName });
#endif
    }

    private MemoryStream CreateZipArchive(
        List<(string sourcePath, string zipPath)> filesToInclude, 
        JsonObject modifiedJson, 
        string extensionName,
        ILog parentLog)
    {
        var l = parentLog.Fn<MemoryStream>($"files:{filesToInclude.Count}, ext:{extensionName}");

        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var zipping = new Zipping(l);
            
            // Add collected files using helper
            zipping.AddFiles(archive, filesToInclude);
            l.A($"Added {filesToInclude.Count} files to ZIP");

            // Add modified extension.json
            var modifiedJsonPath = $"{FolderConstants.AppExtensionsFolder}/{extensionName}/{FolderConstants.DataFolderProtected}/{FolderConstants.AppExtensionJsonFile}";
            var modifiedJsonText = modifiedJson.ToJsonString();
            zipping.AddTextEntry(archive, modifiedJsonPath, modifiedJsonText, new UTF8Encoding(false));
            l.A($"Added modified {FolderConstants.AppExtensionJsonFile} to ZIP");

            // Create and add lock file
            var lockData = CreateLockFile(filesToInclude, modifiedJson, l);
            var lockPath = $"{FolderConstants.AppExtensionsFolder}/{extensionName}/{FolderConstants.DataFolderProtected}/{FolderConstants.AppExtensionLockJsonFile}";
            var lockJson = JsonSerializer.Serialize(lockData, new JsonSerializerOptions { WriteIndented = true });
            zipping.AddTextEntry(archive, lockPath, lockJson, new UTF8Encoding(false));
            l.A($"Added {FolderConstants.AppExtensionLockJsonFile} lock file to ZIP");
        }

        return l.ReturnAsOk(memoryStream);
    }

    private JsonObject ModifyExtensionJson(JsonObject json, int appId, string extensionDataPath, ILog parentLog)
    {
        var l = parentLog.Fn<JsonObject>();

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
                    ["version"] = releaseEntity.Get<string>("Version") ?? "0.0.0",
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
        string extensionDataPath,
        JsonObject extensionJson,
        IAppPaths appPaths,
        string extensionName,
        ILog parentLog)
    {
        var l = parentLog.Fn<List<(string sourcePath, string zipPath)>>();

        l.A("Collecting files to include");

        var files = new List<(string, string)>();

        // 1. Always include /extensions/[name] folder (except App_Data which we handle separately)
        AddDirectoryFiles(extensionPath, extensionPath, $"{FolderConstants.AppExtensionsFolder}/{extensionName}", files, exclude: new[] { $"{FolderConstants.DataFolderProtected}\\{FolderConstants.AppExtensionJsonFile}" }, l);

        // 2. Check for hasAppCode setting
        var hasAppCode = extensionJson.TryGetPropertyValue("hasAppCode", out var hasAppCodeNode) && hasAppCodeNode?.GetValue<bool>() == true;
        if (hasAppCode)
        {
            l.A($"Extension has AppCode, including AppCode/{FolderConstants.AppExtensionsFolder} folder");
            var appCodeExtPath = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, extensionName);
            if (Directory.Exists(appCodeExtPath)) 
                AddDirectoryFiles(appCodeExtPath, appCodeExtPath, $"AppCode/{FolderConstants.AppExtensionsFolder}/{extensionName}", files, parentLog: l);
        }

        // 3. Check for data-bundles
        if (extensionJson.TryGetPropertyValue("data-bundles", out var bundlesNode) && bundlesNode is JsonArray { Count: > 0 } bundles)
        {
            l.A($"Extension has {bundles.Count} data bundles");
            ExportDataBundles(bundles, extensionDataPath, extensionName, files, l);
        }

        l.A($"Collected {files.Count} files");
        return l.ReturnAsOk(files);
    }

    private void ExportDataBundles(JsonArray bundles, string extensionDataPath, string extensionName, List<(string, string)> files, ILog parentLog)
    {
        var l = parentLog.Fn();

        var bundlesDir = Path.Combine(extensionDataPath, "system", "bundles");
        Directory.CreateDirectory(bundlesDir);

        foreach (var bundleRef in bundles)
        {
            var guidString = bundleRef?.GetValue<string>();
            if (string.IsNullOrEmpty(guidString) || !Guid.TryParse(guidString, out var guid))
            {
                l.A($"Skipping invalid bundle GUID: {guidString}");
                continue;
            }

            // Export bundle entity as JSON
            var bundlePath = Path.Combine(bundlesDir, $"{guid}.json");
            
            // Note: This is simplified - you may need to use proper entity export logic
            // For now, we'll create a placeholder that should be replaced with actual bundle export
            var bundleJson = new JsonObject
            {
                ["_guid"] = guid.ToString(),
                ["_note"] = "Bundle export not yet fully implemented - needs entity serialization"
            };
            
            File.WriteAllText(bundlePath, bundleJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            
            var zipPath = $"extensions/{extensionName}/App_Data/system/bundles/{guid}.json";
            files.Add((bundlePath, zipPath));
        }

        l.Done();
    }

    private void AddDirectoryFiles(string sourcePath, string baseSourcePath, string baseZipPath, List<(string, string)> files, string[]? exclude = null, ILog? parentLog = null)
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

    private object CreateLockFile(List<(string sourcePath, string zipPath)> files, JsonObject extensionJson, ILog parentLog)
    {
        var l = parentLog.Fn<object>();
        l.A($"Creating {FolderConstants.AppExtensionLockJsonFile} file");

        var version = extensionJson["version"]?.GetValue<string>() ?? "1.0.0";
        l.A($"{nameof(version)}:{version}");
        
        var fileList = files.Select(f => new
        {
            file = "/" + f.zipPath,
            hash = Sha256.Hash(File.ReadAllText(f.sourcePath))
        }).ToList();

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
