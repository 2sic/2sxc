using Oqtane.Extensions;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Integration;

namespace ToSic.Sxc.Oqt.Server.Adam;

internal static class AdamFolderHelper
{
    public static Folder NewVirtualFolder(int siteId, int? parentId, string path, string folder)
    {
        return new()
        {
            SiteId = siteId,
            ParentId = parentId,
            Name = folder,
            Path = path.EnsureOqtaneFolderFormat(),
            Order = 1,
            IsSystem = false,
            Type = FolderTypes.Private,
            Permissions = new List<Permission>
            {
                new(PermissionNames.Browse, RoleNames.Everyone, true),
                new(PermissionNames.View, RoleNames.Everyone, true),
                new(PermissionNames.Browse, RoleNames.Admin, true),
                new(PermissionNames.View, RoleNames.Admin, true),
                new(PermissionNames.Edit, RoleNames.Admin, true),
            }.EncodePermissions()
        };
    }
}