using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Adam;

public class OqtAdamSecCheckEnvironmentTests
{
    private const string SiteList = "jpg, PNG,webm";

    [Theory]
    [InlineData("photo.jpg", true, "listed")]
    [InlineData("photo.JPG", true, "file extension in other case")]
    [InlineData("photo.png", true, "list entry in other case, with space")]
    [InlineData("video.webm", true, "listed, not in Oqtane default")]
    [InlineData("document.pdf", false, "in Oqtane default, but not in site list")]
    [InlineData("archive.jpg.exe", false, "only the last extension counts")]
    [InlineData("file", false, "no extension")]
    [InlineData("file.", false, "empty extension")]
    public void IsUploadableExtension_UsesSiteList_WhenSiteHasList(string fileName, bool expected, string _)
        => Equal(expected, OqtAdamSecCheckEnvironment.IsUploadableExtension(SiteList, fileName));

    [Theory]
    [InlineData(null, "document.pdf", true)]
    [InlineData("", "document.pdf", true)]
    [InlineData(null, "video.webm", false)]
    [InlineData("", "file", false)]
    public void IsUploadableExtension_UsesOqtaneDefault_WhenSiteHasNoList(string? siteList, string fileName, bool expected)
        => Equal(expected, OqtAdamSecCheckEnvironment.IsUploadableExtension(siteList, fileName));

    #region Native folder permissions

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CanEditFolder_AllowsUserGrant_ForFolderOrFile(bool isFile)
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext();
        context.Folder.PermissionList = [new(PermissionNames.Edit, OqtAdamSecCheckEnvironmentTestContext.UserId, true)];

        True(context.Sut.CanEditFolder(context.Security, context.Asset(isFile)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CanEditFolder_AllowsRoleGrant_ForFolderOrFile(bool isFile)
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext();
        context.Folder.PermissionList = [new(PermissionNames.Edit, OqtAdamSecCheckEnvironmentTestContext.EditorRole, true)];

        True(context.Sut.CanEditFolder(context.Security, context.Asset(isFile)));
    }

    [Fact]
    public void CanEditFolder_DeniesExplicitRoleDenial_EvenWithUserGrant()
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext();
        context.Folder.PermissionList =
        [
            new(PermissionNames.Edit, OqtAdamSecCheckEnvironmentTestContext.UserId, true),
            new(PermissionNames.Edit, OqtAdamSecCheckEnvironmentTestContext.EditorRole, false)
        ];

        False(context.Sut.CanEditFolder(context.Security, context.Asset(isFile: false)));
    }

    [Fact]
    public void CanEditFolder_DeniesExplicitUserDenial_ForSiteAdmin()
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext(isSiteAdmin: true);
        context.Folder.PermissionList =
        [
            new(PermissionNames.Edit, RoleNames.Admin, true),
            new(PermissionNames.Edit, OqtAdamSecCheckEnvironmentTestContext.UserId, false)
        ];

        False(context.Sut.CanEditFolder(context.Security, context.Asset(isFile: false)));
    }

    [Fact]
    public void CanEditFolder_DeniesViewOnlyGrant()
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext();
        context.Folder.PermissionList = [new(PermissionNames.View, OqtAdamSecCheckEnvironmentTestContext.UserId, true)];

        False(context.Sut.CanEditFolder(context.Security, context.Asset(isFile: false)));
    }

    [Fact]
    public void CanEditFolder_DeniesMissingGrant()
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext();

        False(context.Sut.CanEditFolder(context.Security, context.Asset(isFile: false)));
    }

    [Fact]
    public void CanEditFolder_DeniesGrantOnAnotherFolder()
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext();
        context.Folders.Add(OqtAdamSecCheckEnvironmentTestContext.FileId, new Folder
        {
            FolderId = OqtAdamSecCheckEnvironmentTestContext.FileId,
            SiteId = OqtAdamSecCheckEnvironmentTestContext.SiteId,
            PermissionList = [new(PermissionNames.Edit, OqtAdamSecCheckEnvironmentTestContext.UserId, true)]
        });

        False(context.Sut.CanEditFolder(context.Security, context.Asset(isFile: true)));
    }

    [Fact]
    public void CanEditFolder_DeniesFolderFromAnotherSite_EvenWithGrant()
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext(isSiteAdmin: true);
        context.Folder.SiteId++;
        context.Folder.PermissionList = [new(PermissionNames.Edit, OqtAdamSecCheckEnvironmentTestContext.UserId, true)];

        False(context.Sut.CanEditFolder(context.Security, context.Asset(isFile: false)));
    }

    [Fact]
    public void CanEditFolder_DeniesMissingFolder()
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext(isSiteAdmin: true);
        context.Folders.Clear();

        False(context.Sut.CanEditFolder(context.Security, context.Asset(isFile: false)));
    }

    [Fact]
    public void CanEditFolder_DeniesMissingRequest_EvenWithGrant()
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext(isSiteAdmin: true);
        context.Folder.PermissionList = [new(PermissionNames.Edit, OqtAdamSecCheckEnvironmentTestContext.UserId, true)];
        context.HttpContextAccessor.HttpContext = null;

        False(context.Sut.CanEditFolder(context.Security, context.Asset(isFile: false)));
    }

    [Fact]
    public void CanEditFolder_DeniesNullAsset()
    {
        using var context = new OqtAdamSecCheckEnvironmentTestContext(isSiteAdmin: true);

        False(context.Sut.CanEditFolder(context.Security, null!));
    }

    #endregion
}
