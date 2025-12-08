using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sxc.ImportExport.IndexFile.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionDeleteBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    ExtensionManifestService manifestService,
    LazySvc<ExtensionInspectBackend> inspectorLazy,
    LazySvc<EntityApi> entityApiLazy)
    : ServiceBase("Bck.ExtDel", connect: [appReadersLazy, site, appPathSvc, manifestService, inspectorLazy, entityApiLazy])
{
    private ReadOnlyFileHelper ReadOnlyHelper => field ??= new(Log);

    public bool DeleteExtension(int appId, string name, string? edition, bool force, bool withData)
    {
        var l = Log.Fn<bool>($"a:{appId}, name:{name}, edition:{edition}, force:{force}, withData:{withData}");

        if (string.IsNullOrWhiteSpace(name) || !ExtensionFolderNameValidator.IsValid(name))
            throw l.Ex(new ArgumentException("invalid extension name", nameof(name)));

        var editionSegment = ExtensionEditionHelper.NormalizeEdition(edition);
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var extensionPath = ExtensionEditionHelper.GetExtensionRoot(appPaths, name, editionSegment);

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
            throw l.Ex(new InvalidOperationException($"{IndexLockFile.LockFileName} missing; delete manually."));

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

        DeleteFiles(appPaths.PhysicalPath, editionSegment, name);

        return l.ReturnTrue("deleted");
    }

    private void DeleteData(IAppReader appReader, int appId, IEnumerable<ExtensionInspectContentTypeDto> types)
    {
        var l = Log.Fn($"delete data app:{appId}");

        var typeNames = new HashSet<string>(types
            .Select(t => t.Guid)
            .Where(n => n.HasValue()), StringComparer.OrdinalIgnoreCase);

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

    private void DeleteFiles(string appRoot, string edition, string extensionName)
    {
        var l = Log.Fn($"del files ext:{extensionName}, edition:{edition}");

        var extensionPath = ExtensionEditionHelper.GetExtensionRoot(appRoot, extensionName, edition);
        DeleteDirectorySafe(extensionPath);

        var appCodePath = ExtensionEditionHelper.GetExtensionAppCodePath(appRoot, extensionName, edition);
        if (appCodePath.HasValue())
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
        ReadOnlyHelper.RemoveReadOnlyRecursive(path);
        Directory.Delete(path, recursive: true);
        Log.A($"deleted: '{Path.GetFileName(path)}'");
    }
}
