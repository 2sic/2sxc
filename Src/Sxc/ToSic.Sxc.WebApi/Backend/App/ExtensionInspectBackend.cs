using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Sys;
using ToSic.Sxc.ImportExport.IndexFile.Sys;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Backend.App.ExtensionLockHelper;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInspectBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc)
    : ServiceBase("Bck.ExtInsp", connect: [appReadersLazy, site, appPathSvc])
{
    public ExtensionInspectResultDto Inspect(int appId, string name, string? edition)
    {
        var l = Log.Fn<ExtensionInspectResultDto>($"app:{appId}, name:{name}, edition:{edition}");
        if (!ExtensionFolderNameValidator.IsValid(name))
            throw l.Ex(new ArgumentException("invalid extension name", nameof(name)));

        var editionSegment = ExtensionEditionHelper.NormalizeEdition(edition);
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var appRoot = appPaths.PhysicalPath;
        var editionRoot = ExtensionEditionHelper.GetEditionRoot(appPaths, editionSegment);

        var extensionRoot = ExtensionEditionHelper.GetExtensionRoot(appPaths, name, editionSegment);
        var lockPath = Path.Combine(extensionRoot, FolderConstants.DataFolderProtected, IndexLockFile.LockFileName);
        var foundLock = File.Exists(lockPath);

        var statuses = new List<ExtensionFileStatusDto>();
        var changed = 0;
        var missing = 0;
        var added = 0;

        var lockRead = ReadLockFile(lockPath, l);
        var expected = lockRead.ExpectedWithHash;

        if (expected is not null)
        {
            var basePathFull = EnsureTrailingBackslash(Path.GetFullPath(editionRoot));

            foreach (var kvp in expected)
            {
                var rel = kvp.Key;
                var fullPath = Path.GetFullPath(Path.Combine(editionRoot, rel.Backslash()));

                if (!fullPath.StartsWith(basePathFull, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
                {
                    statuses.Add(new() { Path = rel, Status = "missing" });
                    missing++;
                    continue;
                }

                var actualHash = CalculateHash(fullPath);
                if (!string.Equals(actualHash, kvp.Value, StringComparison.OrdinalIgnoreCase))
                {
                    statuses.Add(new() { Path = rel, Status = "changed" });
                    changed++;
                }
                else
                {
                    statuses.Add(new() { Path = rel, Status = "unchanged" });
                }
            }

            added = AddAddedFiles(appRoot: appRoot,
                edition: editionSegment,
                extensionName: name,
                basePathFull: basePathFull,
                expectedPaths: lockRead.AllowedFiles ?? [],
                statuses: statuses);
        }

        var result = new ExtensionInspectResultDto
        {
            FoundLock = foundLock,
            Files = statuses,
            Summary = new()
            {
                Total = statuses.Count,
                Changed = changed,
                Added = added,
                Missing = missing
            },
            Data = BuildData(appId, name)
        };
        return l.Return(result, $"files:{statuses.Count}, changed:{changed}, added:{added}, missing:{missing}");
    }

    private static int AddAddedFiles(string appRoot, string edition, string extensionName, string basePathFull,
        HashSet<string> expectedPaths, List<ExtensionFileStatusDto> statuses)
    {
        var added = 0;
        var seen = new HashSet<string>(statuses.Select(s => s.Path), StringComparer.OrdinalIgnoreCase);
        
        var ownRoots = new[]
        {
            ExtensionEditionHelper.GetExtensionRoot(appRoot, extensionName, edition),
            ExtensionEditionHelper.GetExtensionAppCodePath(appRoot, extensionName, edition),
        };

        foreach (var ownRoot in ownRoots)
        {
            foreach (var file in EnumerateFilesSafe(ownRoot))
            {
                var normalizedFile = Path.GetFullPath(file);
                if (!normalizedFile.StartsWith(basePathFull, StringComparison.OrdinalIgnoreCase))
                    continue;

                var rel = normalizedFile
                    .Substring(basePathFull.Length)
                    .TrimPrefixSlash()
                    .ForwardSlash();

                if (expectedPaths.Contains(rel))
                    continue;

                if (rel.EndsWith(IndexLockFile.LockFileName, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!seen.Add(rel))
                    continue;

                statuses.Add(new() { Path = rel, Status = "added" });
                added++;
            }
        }

        return added;
    }

    private ExtensionInspectDataDto? BuildData(int appId, string extensionName)
    {
        var l = Log.Fn<ExtensionInspectDataDto?>($"app:{appId}, ext:{extensionName}");

        try
        {
            var appReader = appReadersLazy.Value.Get(appId);
            var extensionTypes = appReader.ContentTypes
                .Where(ct => IsContentTypeFromExtension(ct, extensionName))
                .ToList();

            var localEntityCounts = appReader.GetListNotHavingDrafts()
                .Where(IsLocalEntity)
                .GroupBy(e => e.Type.NameId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

            var contentTypes = extensionTypes
                .Select(ct =>
                {
                    var hasData = localEntityCounts.TryGetValue(ct.NameId, out var count);
                    return new ExtensionInspectContentTypeDto
                    {
                        Name = ct.Name,
                        Guid = ct.NameId,
                        LocalEntities = hasData ? count : 0
                    };
                })
                .ToList();

            return l.Return(new () { ContentTypes = contentTypes }, $"types:{contentTypes.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnNull("error");
        }
    }

    private static bool IsContentTypeFromExtension(IContentType contentType, string extensionName)
    {
        var address = contentType.RepositoryAddress;
        if (address.IsEmptyOrWs())
            return false;

        var normalizedPath = address.ForwardSlash();
        return ContainsExtensionPath(normalizedPath, FolderConstants.AppExtensionsFolder, extensionName)
               || ContainsExtensionPath(normalizedPath, FolderConstants.AppExtensionsLegacyFolder, extensionName)
               || ContainsExtensionPath(normalizedPath,
                   $"{FolderConstants.AppCodeFolder}/{FolderConstants.AppExtensionsFolder}", extensionName);
    }

    private static bool ContainsExtensionPath(string normalizedPath, string containerFolder, string extensionName)
    {
        var needle = $"/{containerFolder.Trim('/')}/{extensionName}/";
        return normalizedPath.Contains(needle, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsLocalEntity(IEntity entity)
        => entity.EntityId > 0;
}
