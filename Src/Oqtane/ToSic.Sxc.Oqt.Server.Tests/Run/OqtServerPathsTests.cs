using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sys.DI;

namespace ToSic.Sxc.Oqt.Run;

public class OqtServerPathsTests
{
    [Theory]
    [InlineData(FolderTypes.Private, false)]
    [InlineData(FolderTypes.Public, true)]
    public void FullPathOfReference_UsesStorageRootAndFileSite_ForLocalProviders(string storageType, bool isPublic)
    {
        var files = new OqtImportExportEnvironmentTestContext.FakeFileRepository();
        files.Files.Add(new() { FileId = 23, Name = "photo.jpg", Folder = new Folder { SiteId = 7, Path = "images/", Type = storageType } });
        using var services = new ServiceCollection().AddSingleton<IFileRepository>(files).AddSingleton<ITenantManager, FakeTenantManager>().BuildServiceProvider();
        var hosting = new FakeWebHostEnvironment();
        var sut = CreateServerPaths(services, hosting);

        var path = sut.FullPathOfReference("file:23");

        Equal(Path.Combine(isPublic ? hosting.WebRootPath : hosting.ContentRootPath, "Content", "Tenants", "3", "Sites", "7", "images", "photo.jpg"), path);
    }

    [Fact]
    public void FullPathOfReference_PreservesReference_WhenFileDoesNotExist()
    {
        using var services = new ServiceCollection()
            .AddSingleton<IFileRepository>(new OqtImportExportEnvironmentTestContext.FakeFileRepository())
            .BuildServiceProvider();
        var sut = CreateServerPaths(services, new FakeWebHostEnvironment());

        Equal("file:23", sut.FullPathOfReference("file:23"));
    }

    [Fact]
    public void FullPathOfReference_PreservesReference_ForNonLocalStorage()
    {
        var files = new OqtImportExportEnvironmentTestContext.FakeFileRepository();
        files.Files.Add(new() { FileId = 23, Name = "photo.jpg", Folder = new Folder { SiteId = 7, Path = "images/", Type = "Custom" } });
        using var services = new ServiceCollection().AddSingleton<IFileRepository>(files).AddSingleton<ITenantManager, FakeTenantManager>().BuildServiceProvider();
        var sut = CreateServerPaths(services, new FakeWebHostEnvironment());

        Equal("file:23", sut.FullPathOfReference("file:23"));
    }

    private static OqtServerPaths CreateServerPaths(System.IServiceProvider services, IWebHostEnvironment hosting)
        => new(hosting, new LazySvc<IFileRepository>(services), new LazySvc<ITenantManager>(services));

    [Fact]
    public void FullContentPath_WorksFromSingleton_WithoutResolvingScopedTenantState()
    {
        var hosting = new FakeWebHostEnvironment();
        using var services = new ServiceCollection()
            .AddSingleton<IWebHostEnvironment>(hosting)
            .AddScoped<ITenantManager, FakeTenantManager>()
            .AddTransient(typeof(LazySvc<>))
            .AddTransient<IServerPaths, OqtServerPaths>()
            .AddSingleton<SingletonPathConsumer>()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var paths = services.GetRequiredService<SingletonPathConsumer>().Paths;

        Equal(Path.Combine(hosting.ContentRootPath, "Content", "2sxc"), paths.FullContentPath("Content/2sxc"));
    }

    private sealed class SingletonPathConsumer(IServerPaths paths)
    {
        public IServerPaths Paths { get; } = paths;
    }

    private sealed class FakeTenantManager : ITenantManager
    {
        public Alias GetAlias() => throw new System.NotSupportedException();
        public Tenant GetTenant() => new() { TenantId = 3 };
        public void SetAlias(Alias alias) => throw new System.NotSupportedException();
        public void SetAlias(int tenantId, int siteId) => throw new System.NotSupportedException();
        public void SetTenant(int tenantId) => throw new System.NotSupportedException();
    }

    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests";
        public string EnvironmentName { get; set; } = "Testing";
        public string ContentRootPath { get; set; } = Path.Combine(Path.GetTempPath(), "2sxc-private");
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = Path.Combine(Path.GetTempPath(), "2sxc-public");
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
