using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sys.Logging;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsDeleteBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    ExtensionManifestService manifestService,
    LazySvc<ExtensionsInspectorBackend> inspectorLazy,
    LazySvc<EntityApi> entityApiLazy)
    : ServiceBase("Bck.ExtDel", connect: [appReadersLazy, site, appPathSvc, manifestService, inspectorLazy, entityApiLazy])
{
    public bool DeleteExtension(int appId, string name, string? edition, bool force, bool withData)
    {
        var l = Log.Fn<bool>($"a:{appId}, name:{name}, edition:{edition}, force:{force}, withData:{withData}");

        if (string.IsNullOrWhiteSpace(name) || !ExtensionFolderNameValidator.IsValid(name))
            throw l.Ex(new ArgumentException("invalid extension name", nameof(name)));

        var editionSegment = NormalizeEdition(edition);
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var editionRoot = string.IsNullOrEmpty(editionSegment)
            ? appPaths.PhysicalPath
            : Path.Combine(appPaths.PhysicalPath, editionSegment);
        var extensionPath = Path.Combine(editionRoot, FolderConstants.AppExtensionsFolder, name);

        if (!Directory.Exists(extensionPath))
            throw l.Ex(new DirectoryNotFoundException($"Extension folder not found: {name}"));

        var manifestFile = manifestService.GetManifestFile(new DirectoryInfo(extensionPath));
        if (!manifestFile.Exists)
            throw l.Ex(new FileNotFoundException($"{FolderConstants.AppExtensionJsonFile} not found in extension '{name}'"));

        var manifest = manifestService.LoadManifest(manifestFile)
            ?? throw l.Ex(new InvalidOperationException($"{FolderConstants.AppExtensionJsonFile} could not be loaded"));

        if (manifest.IsInstalled != true)
            throw l.Ex(new InvalidOperationException("Extension is not marked as installed; delete manually."));

        var inspect = inspectorLazy.Value.Inspect(appId, name, editionSegment);
        if (!inspect.FoundLock)
            throw l.Ex(new InvalidOperationException($"{FolderConstants.AppExtensionLockJsonFile} missing; delete manually."));

        var summary = inspect.Summary ?? new ExtensionInspectSummaryDto();
        var hasFileChanges = summary.Changed > 0 || summary.Added > 0 || summary.Missing > 0;
        if (hasFileChanges && !force)
            throw l.Ex(new InvalidOperationException("Extension has file changes; use force to delete."));

        var contentTypesWithData = inspect.Data?.ContentTypes ?? [];
        var hasData = contentTypesWithData.Any(ct => ct.LocalEntities > 0);
        if (hasData && !force)
            throw l.Ex(new InvalidOperationException("Extension has data; use force to delete."));

        if (hasData && withData && force)
            DeleteData(appReader, appId, contentTypesWithData);

        DeleteFiles(extensionPath, appPaths.PhysicalPath, editionSegment, name);

        return l.ReturnTrue("deleted");
    }

    private void DeleteData(IAppReader appReader, int appId, IEnumerable<ExtensionInspectContentTypeDto> types)
    {
        var l = Log.Fn($"delete data app:{appId}");

        var typeNames = new HashSet<string>(types
            .Select(t => t.Guid)
            .Where(n => !string.IsNullOrWhiteSpace(n))!, StringComparer.OrdinalIgnoreCase);

        if (typeNames.Count == 0)
        {
            l.Done("no type names to delete");
            return;
        }

        var entityApi = entityApiLazy.Value.Init(appId, showDrafts: true);

        var entityIds = appReader.List
            .Where(e => e.EntityId > 0 && typeNames.Any(t => e.Type.Is(t)))
            .Select(e => new { e.EntityId, e.Type.NameId })
            .DistinctBy(e => e.EntityId)
            .ToList();

        foreach (var entity in entityIds)
            entityApi.Delete(entity.NameId, entity.EntityId, force: true);

        l.Done($"deleted:{entityIds.Count}");
    }

    private void DeleteFiles(string extensionPath, string appRoot, string edition, string extensionName)
    {
        var l = Log.Fn($"del files ext:{extensionName}, edition:{edition}");

        DeleteDirectorySafe(extensionPath);

        var appCodePath = GetExtensionAppCodePath(appRoot, extensionName, edition);
        if (!string.IsNullOrWhiteSpace(appCodePath))
            DeleteDirectorySafe(appCodePath);

        l.Done();
    }

    private void DeleteDirectorySafe(string path)
    {
        var l = Log.Fn();
        if (path.IsEmpty())
        {
            l.Done("path empty");
            return;
        }
        if (!Directory.Exists(path))
        {
            l.Done("path not exist");
            return;
        }

        Log.A($"clear readonly + delete: '{Path.GetFileName(path)}'");
        RemoveReadOnlyRecursive(path);
        Directory.Delete(path, recursive: true);
        Log.A($"deleted: '{Path.GetFileName(path)}'");
    }

    private static string NormalizeEdition(string? edition)
    {
        if (edition.IsEmpty())
            return string.Empty;

        var normalized = edition.Trim().TrimPrefixSlash().TrimEnd('/', '\\');
        return normalized.ContainsPathTraversal()
            ? throw new ArgumentException("edition contains invalid path traversal", nameof(edition))
            : normalized;
    }

    private static string GetExtensionAppCodePath(string appRoot, string extensionName, string edition)
    {
        var editionAppCode = Path.Combine(appRoot, edition, FolderConstants.AppCodeFolder);
        return Directory.Exists(editionAppCode)
            ? Path.Combine(editionAppCode, FolderConstants.AppExtensionsFolder, extensionName)
            : Path.Combine(appRoot, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, extensionName);
    }

    private void RemoveReadOnlyRecursive(string directory)
    {
        var l = Log.Fn();
        if (!Directory.Exists(directory))
        {
            l.Done("path not exist");
            return;
        }

        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
            RemoveReadOnlyIfNeeded(file);

        foreach (var dir in Directory.GetDirectories(directory, "*", SearchOption.AllDirectories))
            ClearDirectoryReadOnly(dir);

        ClearDirectoryReadOnly(directory);
        l.Done();
    }

    private void RemoveReadOnlyIfNeeded(string path)
    {
        var l = Log.Fn();
        if (!File.Exists(path))
        {
            l.Done("file do not exist");
            return;
        }

        var attributes = File.GetAttributes(path);
        if (!attributes.HasFlag(FileAttributes.ReadOnly))
        {
            l.Done("file is not readonly");
            return;
        }

        File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
        l.Done($"cleared readonly:'{path}'");
    }

    private void ClearDirectoryReadOnly(string directory)
    {
        var l = Log.Fn();
        var info = new DirectoryInfo(directory);
        var attributes = info.Attributes;
        if (!attributes.HasFlag(FileAttributes.ReadOnly))
        {
            l.Done("directory is not readonly");
            return;
        }

        info.Attributes = attributes & ~FileAttributes.ReadOnly;
        l.Done($"cleared readonly dir:'{directory}'");
    }
}
