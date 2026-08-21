#nullable enable

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.StartUp;

/// <summary>
/// Adds Windows-compatible casing fallback for dynamically compiled 2sxc Razor files.
/// </summary>
internal sealed class CaseInsensitiveSxcFileProvider(IFileProvider inner) : IFileProvider
{
    private const string SxcRoot = OqtConstants.AppRoot;

    public IFileInfo GetFileInfo(string subpath)
    {
        var exact = inner.GetFileInfo(subpath);
        if (exact.Exists)
            return exact;

        var resolved = ResolvePath(subpath);
        return resolved == null ? exact : inner.GetFileInfo(resolved);
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var exact = inner.GetDirectoryContents(subpath);
        if (exact.Exists)
            return exact;

        var resolved = ResolvePath(subpath);
        return resolved == null ? exact : inner.GetDirectoryContents(resolved);
    }

    public IChangeToken Watch(string filter)
        => inner.Watch(ResolvePath(filter, allowMissingLastSegment: true) ?? filter);

    internal static void Wrap(IList<IFileProvider> fileProviders)
    {
        if (fileProviders.Count == 0
            || fileProviders.Count == 1 && fileProviders[0] is CaseInsensitiveSxcFileProvider)
            return;

        var inner = fileProviders.Count == 1
            ? fileProviders[0]
            : new CompositeFileProvider(fileProviders.ToArray());

        fileProviders.Clear();
        fileProviders.Add(new CaseInsensitiveSxcFileProvider(inner));
    }

    private string? ResolvePath(string subpath, bool allowMissingLastSegment = false)
    {
        var normalized = subpath.Replace('\\', '/');
        var withoutLeadingSlash = normalized.TrimStart('/');
        if (!withoutLeadingSlash.Equals(SxcRoot, StringComparison.OrdinalIgnoreCase)
            && !withoutLeadingSlash.StartsWith($"{SxcRoot}/", StringComparison.OrdinalIgnoreCase))
            return null;

        var segments = withoutLeadingSlash.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Any(segment => segment is "." or ".."))
            return null;

        var resolved = new List<string>(segments.Length);
        for (var i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];
            if (segment.IndexOfAny(['*', '?', '[']) >= 0)
            {
                resolved.AddRange(segments[i..]);
                break;
            }

            var contents = inner.GetDirectoryContents(ToPath(resolved, normalized.StartsWith('/')));
            var match = FindMatch(contents, segment);
            if (match == null)
            {
                if (allowMissingLastSegment && i == segments.Length - 1 && contents.Exists)
                {
                    resolved.Add(segment);
                    break;
                }

                return null;
            }

            resolved.Add(match.Name);
        }

        return ToPath(resolved, normalized.StartsWith('/'));
    }

    private static IFileInfo? FindMatch(IDirectoryContents contents, string segment)
    {
        if (!contents.Exists)
            return null;

        IFileInfo? match = null;
        foreach (var item in contents)
        {
            if (item.Name.Equals(segment, StringComparison.Ordinal))
                return item;

            if (!item.Name.Equals(segment, StringComparison.OrdinalIgnoreCase))
                continue;

            if (match != null)
                return null;

            match = item;
        }

        return match;
    }

    private static string ToPath(IEnumerable<string> segments, bool leadingSlash)
    {
        var path = string.Join('/', segments);
        return leadingSlash && path.Length > 0 ? $"/{path}" : path;
    }
}
