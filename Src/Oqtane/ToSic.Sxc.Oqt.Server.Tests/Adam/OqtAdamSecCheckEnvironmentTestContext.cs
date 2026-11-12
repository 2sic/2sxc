using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Adam.Sys.Security;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Users.Permissions;
using OqtSite = ToSic.Sxc.Oqt.Server.Context.OqtSite;

namespace ToSic.Sxc.Oqt.Adam;

internal sealed class OqtAdamSecCheckEnvironmentTestContext : IDisposable
{
    internal const int SiteId = 10;
    internal const int FolderId = 20;
    internal const int FileId = 30;
    internal const int UserId = 40;
    internal const string EditorRole = "Folder Editors";

    internal OqtAdamSecCheckEnvironmentTestContext(bool isSiteAdmin = false)
    {
        var user = new User
        {
            UserId = UserId,
            Username = "folder-editor",
            SecurityStamp = "test-security-stamp",
            Roles = $";{RoleNames.Registered};{(isSiteAdmin ? RoleNames.Admin : EditorRole)};"
        };
        var alias = new Alias { SiteId = SiteId, TenantId = 1 };
        HttpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = new(UserSecurity.CreateClaimsIdentity(alias, user))
        };
        var site = new OqtSite(null!, null!, null!, null!, null!, null!, null!).Init(new Site { SiteId = SiteId });
        var context = new ContextOfApp(site, null!, null)
        {
            InternalMachinery = null!,
            SettingsStackSvc = null!,
            Permissions = new EffectivePermissions(isSiteAdmin: isSiteAdmin, isContentAdmin: isSiteAdmin)
        };
        Security = new FakeAdamSecurityCheckService(new FakeAdamContext(context));
        Folders.Add(FolderId, Folder);
        var userPermissions = new UserPermissions(null!, new FakeUserRoleRepository(), HttpContextAccessor);
        Sut = new(null!, new FakeFolderRepository(Folders), userPermissions, HttpContextAccessor);
    }

    internal OqtAdamSecCheckEnvironment Sut { get; }
    internal IAdamSecurityCheckService Security { get; }
    internal HttpContextAccessor HttpContextAccessor { get; } = new();
    internal Dictionary<int, Folder> Folders { get; } = [];
    internal Folder Folder { get; } = new() { FolderId = FolderId, SiteId = SiteId, PermissionList = [] };

    internal IAsset Asset(bool isFile) => isFile
        ? new FakeFile
        {
            SysId = FileId, ParentSysId = FolderId, Folder = "Documents", FullName = "file.txt", Extension = "txt",
            Created = default, Modified = default, Name = "file", Path = "Documents/file.txt", PhysicalPath = ""
        }
        : new FakeFolder
        {
            SysId = FolderId, ParentSysId = 99,
            Created = default, Modified = default, Name = "Documents", Path = "Documents/", PhysicalPath = ""
        };

    public void Dispose() => HttpContextAccessor.HttpContext = null;

    private sealed class FakeFolder : Eav.Apps.Assets.Sys.Folder<int, int>;
    private sealed class FakeFile : Eav.Apps.Assets.Sys.File<int, int>;

    private sealed class FakeAdamContext(IContextOfApp context) : AdamContext(null!, null!, null!, null!, null!, null!)
    {
        protected override Options GetDefaultOptions() => new(context, "", "", Guid.Empty, true);
    }

    private sealed class FakeAdamSecurityCheckService(AdamContext adamContext) : IAdamSecurityCheckService
    {
        public AdamContext AdamContext => adamContext;
        public bool UserIsRestricted => throw new NotSupportedException();
        public void Setup(AdamContext options) => throw new NotSupportedException();
        public bool CanEditFolder(IAsset? item) => throw new NotSupportedException();
        public bool FieldPermissionOk(List<Grants> requiredGrant) => throw new NotSupportedException();
        public bool ExtensionIsNotOk(string fileName, out HttpExceptionAbstraction? preparedException) => throw new NotSupportedException();
        public bool UserIsRestrictedOrItemIsNotDraft(Guid guid, out HttpExceptionAbstraction? exp) => throw new NotSupportedException();
        public bool FieldDoesNotSupportFiles(out HttpExceptionAbstraction? preparedException) => throw new NotSupportedException();
        public bool UserNotPermittedOnField(List<Grants> requiredPermissions, out HttpExceptionAbstraction? preparedException) => throw new NotSupportedException();
        public bool MustThrowIfAccessingRootButNotAllowed(bool usePortalRoot, out HttpExceptionAbstraction? preparedException) => throw new NotSupportedException();
        public bool UserIsRestrictedAndAccessingItemOutsideOfFolder(string? path, out HttpExceptionAbstraction? preparedException) => throw new NotSupportedException();
    }

    private sealed class FakeFolderRepository(Dictionary<int, Folder> folders) : IFolderRepository
    {
        public Folder GetFolder(int folderId) => folders.GetValueOrDefault(folderId)!;
        public Folder GetFolder(int folderId, bool tracking) => GetFolder(folderId);
        public IEnumerable<Folder> GetFolders(int siteId) => throw new NotSupportedException();
        public Folder GetFolder(int siteId, string path) => throw new NotSupportedException();
        public Folder AddFolder(Folder folder) => throw new NotSupportedException();
        public Folder UpdateFolder(Folder folder) => throw new NotSupportedException();
        public void DeleteFolder(int folderId) => throw new NotSupportedException();
        public string GetFolderPath(int folderId) => throw new NotSupportedException();
        public string GetFolderPath(Folder folder) => throw new NotSupportedException();
    }

    private sealed class FakeUserRoleRepository : IUserRoleRepository
    {
        public IEnumerable<UserRole> GetUserRoles(int userId, int siteId) => [];
        public IEnumerable<UserRole> GetUserRoles(int siteId) => throw new NotSupportedException();
        public IEnumerable<UserRole> GetUserRoles(string roleName, int siteId) => throw new NotSupportedException();
        public UserRole AddUserRole(UserRole userRole) => throw new NotSupportedException();
        public UserRole UpdateUserRole(UserRole userRole) => throw new NotSupportedException();
        public UserRole GetUserRole(int userRoleId) => throw new NotSupportedException();
        public UserRole GetUserRole(int userRoleId, bool tracking) => throw new NotSupportedException();
        public UserRole GetUserRole(int userId, int roleId) => throw new NotSupportedException();
        public UserRole GetUserRole(int userId, int roleId, bool tracking) => throw new NotSupportedException();
        public void DeleteUserRole(int userRoleId) => throw new NotSupportedException();
        public void DeleteUserRoles(int userId) => throw new NotSupportedException();
    }
}
