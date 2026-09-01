using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using ToSic.Sxc.Oqt.Server.StartUp;

namespace ToSic.Sxc.Oqt.Razor;

public class CaseInsensitiveSxcFileProviderTests
{
    private const string ActualPath = "/2sxc/Tenants/1/Sites/1/Content/bs5/Shared/Content List Header.cshtml";

    [Fact]
    public void GetFileInfo_FindsSxcFile_WhenCasingDiffers()
    {
        // Arrange
        var provider = new CaseInsensitiveSxcFileProvider(new TestFileProvider(ActualPath));

        // Act
        var result = provider.GetFileInfo("/2sxc/Tenants/1/Sites/1/Content/bs5/shared/content list header.cshtml");

        // Assert
        True(result.Exists);
    }

    [Fact]
    public void GetFileInfo_DoesNotFallback_OutsideSxcRoot()
    {
        // Arrange
        var provider = new CaseInsensitiveSxcFileProvider(new TestFileProvider("/Views/Shared/Host.cshtml"));

        // Act
        var result = provider.GetFileInfo("/views/shared/host.cshtml");

        // Assert
        False(result.Exists);
    }

    [Fact]
    public void GetFileInfo_DoesNotChoose_WhenCaseMatchIsAmbiguous()
    {
        // Arrange
        var provider = new CaseInsensitiveSxcFileProvider(new TestFileProvider(
            "/2sxc/Tenants/1/Sites/1/Content/bs5/Shared/Header.cshtml",
            "/2sxc/Tenants/1/Sites/1/Content/bs5/shared/Header.cshtml"));

        // Act
        var result = provider.GetFileInfo("/2sxc/Tenants/1/Sites/1/Content/bs5/SHARED/Header.cshtml");

        // Assert
        False(result.Exists);
    }

    [Fact]
    public void GetDirectoryContents_FindsSxcDirectory_WhenCasingDiffers()
    {
        // Arrange
        var provider = new CaseInsensitiveSxcFileProvider(new TestFileProvider(ActualPath));

        // Act
        var result = provider.GetDirectoryContents("/2sxc/Tenants/1/Sites/1/Content/bs5/shared");

        // Assert
        Equal("Content List Header.cshtml", Single(result).Name);
    }

    [Fact]
    public void Watch_UsesActualCasing_WhenFileExists()
    {
        // Arrange
        var inner = new TestFileProvider(ActualPath);
        var provider = new CaseInsensitiveSxcFileProvider(inner);

        // Act
        provider.Watch("/2sxc/Tenants/1/Sites/1/Content/bs5/shared/Content List Header.cshtml");

        // Assert
        Equal(ActualPath, inner.LastWatchFilter);
    }

    [Fact]
    public void Wrap_AddsFallbackToConfiguredProviders()
    {
        // Arrange
        IList<IFileProvider> providers = [new TestFileProvider(ActualPath)];

        // Act
        CaseInsensitiveSxcFileProvider.Wrap(providers);
        var result = providers[0].GetFileInfo("/2sxc/Tenants/1/Sites/1/Content/bs5/shared/Content List Header.cshtml");

        // Assert
        True(result.Exists);
    }

    [Fact]
    public void GetFileInfo_FindsPhysicalSxcFile_WhenDirectoryCasingDiffers()
    {
        // Arrange
        var root = Path.Combine(Path.GetTempPath(), $"2sxc-case-{Guid.NewGuid():N}");
        var file = Path.Combine(root, "2sxc", "Tenants", "1", "Sites", "1", "Content", "bs5", "Shared", "Header.cshtml");
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, "test");

        try
        {
            using var physical = new PhysicalFileProvider(root);
            var provider = new CaseInsensitiveSxcFileProvider(physical);

            // Act
            var result = provider.GetFileInfo("/2sxc/Tenants/1/Sites/1/Content/bs5/shared/Header.cshtml");

            // Assert
            True(result.Exists);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class TestFileProvider(params string[] files) : IFileProvider
    {
        private readonly HashSet<string> _files = files
            .Select(Normalize)
            .ToHashSet(StringComparer.Ordinal);

        public string? LastWatchFilter { get; private set; }

        public IFileInfo GetFileInfo(string subpath)
        {
            var path = Normalize(subpath);
            return _files.Contains(path)
                ? new TestFileInfo(Path.GetFileName(path), isDirectory: false)
                : new NotFoundFileInfo(Path.GetFileName(path));
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var path = Normalize(subpath);
            var prefix = path.Length == 0 ? "" : $"{path}/";
            var entries = _files
                .Where(file => file.StartsWith(prefix, StringComparison.Ordinal))
                .Select(file => file[prefix.Length..])
                .Where(remainder => remainder.Length > 0)
                .Select(remainder => new
                {
                    Name = remainder.Split('/')[0],
                    IsDirectory = remainder.Contains('/'),
                })
                .GroupBy(item => item.Name, StringComparer.Ordinal)
                .Select(group => (IFileInfo)new TestFileInfo(group.Key, group.Any(item => item.IsDirectory)))
                .ToArray();

            return new TestDirectoryContents(entries.Length == 0 ? null : entries);
        }

        public IChangeToken Watch(string filter)
        {
            LastWatchFilter = filter;
            return NullChangeToken.Singleton;
        }

        private static string Normalize(string path)
            => path.Replace('\\', '/').Trim('/');
    }

    private sealed class TestDirectoryContents(IReadOnlyList<IFileInfo>? entries) : IDirectoryContents
    {
        public bool Exists => entries != null;

        public IEnumerator<IFileInfo> GetEnumerator()
            => (entries ?? []).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    private sealed class TestFileInfo(string name, bool isDirectory) : IFileInfo
    {
        public bool Exists => true;
        public long Length => 0;
        public string? PhysicalPath => null;
        public string Name => name;
        public DateTimeOffset LastModified => DateTimeOffset.UnixEpoch;
        public bool IsDirectory => isDirectory;
        public Stream CreateReadStream() => new MemoryStream();
    }
}
