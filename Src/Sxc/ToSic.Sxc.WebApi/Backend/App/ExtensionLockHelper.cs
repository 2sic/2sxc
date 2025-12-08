using System.Text.Json;
using ToSic.Eav.Sys;
using ToSic.Sxc.ImportExport.Package.Sys;
using ToSic.Sys.Security.Encryption;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Shared helpers to read and work with extension lock files.
/// </summary>
internal static class ExtensionLockHelper
{
    internal static LockFileReadResult ReadLockFile(string lockFilePath, ILog? parentLog)
    {
        var l = parentLog.Fn<LockFileReadResult>();
        try
        {
            var json = File.ReadAllText(lockFilePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("files", out var filesProp) || filesProp.ValueKind != JsonValueKind.Array)
                return l.Return(new(false, $"{PackageIndexFile.LockFileName} missing 'files' array", null, null));

            var expectedWithHash = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in filesProp.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                    return l.Return(new(false, $"invalid {PackageIndexFile.LockFileName} entry: {item.ValueKind}", null, null));

                if (!item.TryGetProperty("file", out var f) || f.ValueKind != JsonValueKind.String)
                    return l.Return(new(false, $"{PackageIndexFile.LockFileName} entry missing 'file'", null, null));

                if (!item.TryGetProperty("hash", out var h) || h.ValueKind != JsonValueKind.String)
                    return l.Return(new(false, $"{PackageIndexFile.LockFileName} entry missing 'hash'", null, null));

                var file = f.GetString()!.Trim().TrimPrefixSlash().ForwardSlash();
                var hash = h.GetString()!.Trim();

                if (file.ContainsPathTraversal())
                    return l.Return(new(false, $"illegal path:'{file}' in {PackageIndexFile.LockFileName}", null, null));

                allowed.Add(file);
                expectedWithHash[file] = hash;
                l.A($"added lock entry:'{file}'");
            }

            return l.Return(new(true, null, expectedWithHash, allowed), $"entries:{expectedWithHash.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new(false, $"{PackageIndexFile.LockFileName} parse error", null, null));
        }
    }

    internal static IEnumerable<string> EnumerateFilesSafe(string? path)
        => !string.IsNullOrWhiteSpace(path) && Directory.Exists(path)
            ? Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            : Array.Empty<string>();

    internal static string EnsureTrailingBackslash(string path)
        => path.SuffixSlash().Backslash();

    internal static string CalculateHash(string path)
        => Sha256.Hash(File.ReadAllText(path));
}

internal record LockFileReadResult(bool Success, string? Error, Dictionary<string, string>? ExpectedWithHash, HashSet<string>? AllowedFiles);
