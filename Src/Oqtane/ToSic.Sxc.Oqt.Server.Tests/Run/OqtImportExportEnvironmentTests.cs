using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sys.DI;
using File = Oqtane.Models.File;

namespace ToSic.Sxc.Oqt.Run;

public class OqtImportExportEnvironmentTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void TransferFilesToSite_CopiesAndRegistersFile_WithIndependentDiskAndDatabaseState(bool directoryExists, bool folderExists)
    {
        using var context = new OqtImportExportEnvironmentTestContext();
        if (directoryExists)
            Directory.CreateDirectory(context.Destination);
        if (folderExists)
            context.Folders.AddFolder(new Folder { SiteId = 7, ParentId = 1, Path = "images/", Name = "images", Type = FolderTypes.Private });

        var messages = context.Environment.TransferFilesToSite(context.Source, "images");

        Empty(messages);
        Equal("imported", System.IO.File.ReadAllText(context.DestinationFile));
        var folder = context.Folders.GetFolder(7, "images/");
        Equal(FolderTypes.Private, folder.Type);
        Equal(1, folder.ParentId);
        Equal(folder.FolderId, Single(context.Files.Files).FolderId);
        Equal(2, context.Folders.Folders.Count);
    }

    [Fact]
    public void TransferFilesToSite_PreservesExistingFile_OnRepeatedImport()
    {
        using var context = new OqtImportExportEnvironmentTestContext();
        context.Environment.TransferFilesToSite(context.Source, "images");
        System.IO.File.WriteAllText(context.SourceFile, "replacement");

        var messages = context.Environment.TransferFilesToSite(context.Source, "images");

        Single(messages);
        Equal("imported", System.IO.File.ReadAllText(context.DestinationFile));
        Single(context.Files.Files);
    }

    [Fact]
    public void TransferFilesToSite_FailsBeforeCopying_WhenFolderCannotBeRegistered()
    {
        using var context = new OqtImportExportEnvironmentTestContext();
        context.Folders.FailAdd = true;

        Throws<InvalidOperationException>(() => context.Environment.TransferFilesToSite(context.Source, "images"));

        False(System.IO.File.Exists(context.DestinationFile));
        Empty(context.Files.Files);
    }
}

internal sealed class OqtImportExportEnvironmentTestContext : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"2sxc-import-{Guid.NewGuid():N}");
    private readonly ServiceProvider _services;
    public string Source => Path.Combine(_root, "source");
    public string SourceFile => Path.Combine(Source, "sample.txt");
    public string Destination => Path.Combine(_root, "content", "images");
    public string DestinationFile => Path.Combine(Destination, "sample.txt");
    public FakeFolderRepository Folders { get; } = new();
    public FakeFileRepository Files { get; } = new();
    public OqtImportExportEnvironment Environment { get; }

    public OqtImportExportEnvironmentTestContext()
    {
        Directory.CreateDirectory(Source);
        System.IO.File.WriteAllText(SourceFile, "imported");
        _services = new ServiceCollection()
            .AddSingleton(new OqtSiteGroup(null!, new FakeSiteGroupMemberRepository()))
            .BuildServiceProvider();
        var aliasResolver = new AliasResolver(new SiteState { Alias = new Alias { AliasId = 1, TenantId = 1, SiteId = 7 } }, null!, null!, null!, null!);
        var site = new OqtSite(aliasResolver, null!, null!, null!, null!, new LazySvc<OqtSiteGroup>(_services), null!)
            .Init(new Site { SiteId = 7 });
        Environment = new(new(site, null!, null!, null!), site, new FakeServerPaths(Path.Combine(_root, "content")), Files, Folders);
    }

    public void Dispose()
    {
        _services.Dispose();
        Directory.Delete(_root, recursive: true);
    }

    private sealed class FakeServerPaths(string root) : IServerPaths
    {
        public string FullContentPath(string virtualPath)
        {
            const string contentPath = "Content/Tenants/1/Sites/7/";
            return Path.Combine(root, virtualPath.Replace('\\', '/')[contentPath.TrimEnd('/').Length..].TrimStart('/'));
        }
        public string FullAppPath(string virtualPath) => throw new NotSupportedException();
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(fileReference))]
        public string? FullPathOfReference(string fileReference) => throw new NotSupportedException();
    }

    internal sealed class FakeFolderRepository : IFolderRepository
    {
        public List<Folder> Folders { get; } = [new() { FolderId = 1, SiteId = 7, Path = "", Name = "Root" }];
        public bool FailAdd { get; set; }
        public Folder AddFolder(Folder folder)
        {
            if (FailAdd)
                throw new DbUpdateException("Test folder registration failure.");
            folder.FolderId = Folders.Count + 1;
            Folders.Add(folder);
            return folder;
        }
        public Folder GetFolder(int siteId, string path) => Folders.FirstOrDefault(folder => folder.SiteId == siteId && folder.Path == path)!;
        public IEnumerable<Folder> GetFolders(int siteId) => throw new NotSupportedException();
        public IEnumerable<Folder> GetFolders(int siteId, int userId) => throw new NotSupportedException();
        public Folder UpdateFolder(Folder folder) => throw new NotSupportedException();
        public Folder GetFolder(int folderId) => throw new NotSupportedException();
        public Folder GetFolder(int folderId, bool tracking) => throw new NotSupportedException();
        public Folder GetFolder(int siteId, string path, int userId) => throw new NotSupportedException();
        public void DeleteFolder(int folderId) => throw new NotSupportedException();
        public string GetFolderPath(int folderId) => throw new NotSupportedException();
        public string GetFolderPath(Folder folder) => throw new NotSupportedException();
    }

    internal sealed class FakeFileRepository : IFileRepository
    {
        public List<File> Files { get; } = [];
        public IEnumerable<File> GetFiles(int folderId, bool tracking) => Files.Where(file => file.FolderId == folderId);
        public File AddFile(File file)
        {
            file.FileId = Files.Count + 1;
            Files.Add(file);
            return file;
        }
        public IEnumerable<File> GetFiles(int folderId) => throw new NotSupportedException();
        public File UpdateFile(File file) => throw new NotSupportedException();
        public File GetFile(int fileId) => Files.FirstOrDefault(file => file.FileId == fileId)!;
        public File GetFile(int fileId, bool tracking) => throw new NotSupportedException();
        public File GetFile(int folderId, string fileName) => throw new NotSupportedException();
        public File GetFile(int siteId, string folderPath, string fileName) => throw new NotSupportedException();
        public void DeleteFile(int fileId) => throw new NotSupportedException();
        public string GetFilePath(int fileId) => throw new NotSupportedException();
        public string GetFilePath(File file) => throw new NotSupportedException();
    }

    private sealed class FakeSiteGroupMemberRepository : ISiteGroupMemberRepository
    {
        public IEnumerable<SiteGroupMember> GetSiteGroupMembers(int siteId, int siteGroupId) => [];
        public IEnumerable<SiteGroupMember> GetSiteGroupMembers() => throw new NotSupportedException();
        public SiteGroupMember AddSiteGroupMember(SiteGroupMember member) => throw new NotSupportedException();
        public SiteGroupMember UpdateSiteGroupMember(SiteGroupMember member) => throw new NotSupportedException();
        public SiteGroupMember GetSiteGroupMember(int id) => throw new NotSupportedException();
        public SiteGroupMember GetSiteGroupMember(int id, bool tracking) => throw new NotSupportedException();
        public void DeleteSiteGroupMember(int id) => throw new NotSupportedException();
    }
}
