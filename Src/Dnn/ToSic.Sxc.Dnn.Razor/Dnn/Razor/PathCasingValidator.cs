using ToSic.Sxc.Dnn.Razor.Sys;

namespace ToSic.Sxc.Dnn.Razor;

// TODO: VERIFY THIS WORKS ON A REAL WEB SERVER
// It may throw exceptions on higher-up folders, since it probably doesn't allow enumerating the file system in those folders.
// So we may need to catch exceptions and return false in those cases.
// For our check it would not matter, since those paths would be out of-range and not relevant for
// cshtml casing.

/// <summary>
/// Helper to check for valid path casing.
/// </summary>
public static class PathCasingValidator
{
    public static bool IsPathCasingExact(string path)
    {
        // First check for incompatible slashes
        if (path.Contains("\\"))
            return false;
        
        if (!File.Exists(path) && !Directory.Exists(path))
            return false;

        var fullPath = Path.GetFullPath(path);
        var current = Path.GetPathRoot(fullPath);

        var relativePath = fullPath.Substring(current.Length);

        var separators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        var segments = relativePath
            .Split(separators, StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in segments)
        {
            var match = Directory
                .GetFileSystemEntries(current, segment)
                .FirstOrDefault();

            // Compare exact string casing
            if (match == null || Path.GetFileName(match) != segment)
                return false;

            current = match;
        }

        return true;
    }

    internal static PathCaseCheckResult IsPathOkForLinux(HtmlHelperContextWithPaths fullOptions)
    {
        // Only check the segments passed in, + 1 (otherwise a `File.cshtml` would have 0 segments)
        var slashCount = fullOptions.Relative.Count(c => c == '/');
        return IsPathOkForLinux(fullOptions.Relative, fullOptions.FullPath, slashCount + 1);
    }

    public static PathCaseCheckResult IsPathOkForLinux(string original, string path, int segments = 50)
    {
        // First check for incompatible slashes
        if (original.Contains("\\"))
            return new(false, -1, ErrIncompatibleSlashes);
        return IsPathCasingExactReversed(path, segments);
    }

    public static PathCaseCheckResult IsPathCasingExactReversed(string path, int segments = 50)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
            return new(false, -1, ErrPathDoesNotExist);

        var current = Path.GetFullPath(path);

        var countdown = segments;
        while (!string.IsNullOrEmpty(current))
        {
            // Make sure we don't go too deep if not desired
            // If it start w/0, exit on the first loop
            if (countdown <= 0)
                return new(true, 0, $"{OkMaxSegmentsReached}: {segments}");
            countdown--;
            
            var parent = Path.GetDirectoryName(current);

            // Reached the drive root (e.g., "C:\")
            if (string.IsNullOrEmpty(parent))
                return new(true, -countdown, OkTopReached);

            var expectedSegment = Path.GetFileName(current);

            // Get actual on-disk entry casing within the parent folder
            var match = Directory
                .GetFileSystemEntries(parent!, expectedSegment)
                .FirstOrDefault();

            // Fail immediately if the file or subfolder casing doesn't match
            if (match == null || Path.GetFileName(match) != expectedSegment)
                return new(false, --countdown, $"'{Path.GetFileName(match)}' != '{expectedSegment}'");

            // Move UP one level in the directory tree
            current = parent;
            
        }

        return new(true, segments, "casing matches");
    }
    public const string ErrIncompatibleSlashes = "incompatible slashes";
    public const string ErrPathDoesNotExist = "path does not exist";
    public const string OkMaxSegmentsReached = "max segments reached";
    public const string OkTopReached = "top reached";
    public record PathCaseCheckResult(bool IsOk, int Segment, string Message);
}