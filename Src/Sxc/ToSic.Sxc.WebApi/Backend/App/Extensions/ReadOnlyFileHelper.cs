using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Helper to manage readonly flags on files and directories.
/// </summary>
internal class ReadOnlyFileHelper(ILog? parentLog) : HelperBase(parentLog, "Bck.RoHlp")
{
    internal void RemoveReadOnlyRecursive(string directory)
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

    internal void RemoveReadOnlyIfNeeded(string path, string? relPath = null)
    {
        var l = Log.Fn();

        if (!File.Exists(path))
        {
            l.Done(relPath.HasValue() ? $"file not found: {relPath}" : "file do not exist");
            return;
        }

        var attributes = File.GetAttributes(path);
        if (!attributes.HasFlag(FileAttributes.ReadOnly))
        {
            l.Done(relPath.HasValue() ? $"file is not readonly: {relPath}" : "file is not readonly");
            return;
        }

        File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
        l.Done($"cleared readonly:'{relPath ?? path}'");
    }

    internal void EnsureReadOnly(string path, string relPath)
    {
        var l = Log.Fn();

        if (!File.Exists(path))
        {
            l.Done($"file not found: {relPath}");
            return;
        }

        var attributes = File.GetAttributes(path);
        if (attributes.HasFlag(FileAttributes.ReadOnly))
        {
            l.Done($"file is already readonly: {relPath}");
            return;
        }

        File.SetAttributes(path, attributes | FileAttributes.ReadOnly);
        l.Done($"set readonly:'{relPath}'");
    }

    internal void ClearDirectoryReadOnly(string directory)
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
