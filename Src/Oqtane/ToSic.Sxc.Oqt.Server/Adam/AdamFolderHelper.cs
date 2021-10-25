using Oqtane.Extensions;
using Oqtane.Models;
using Oqtane.Shared;
using System.Collections.Generic;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    public class AdamFolderHelper
    {
        public static Folder NewVirtualFolder(int siteId, int? parentId, string path, string folder)
        {
            return new Folder
            {
                SiteId = siteId,
                ParentId = parentId,
                Name = folder,
                Path = path,
                Order = 1,
                IsSystem = false,
                Type = FolderTypes.Private,
                Permissions = new List<Permission>
                {
                    new Permission(PermissionNames.Browse, RoleNames.Everyone, true),
                    new Permission(PermissionNames.View, RoleNames.Everyone, true),
                    new Permission(PermissionNames.Browse, RoleNames.Admin, true),
                    new Permission(PermissionNames.View, RoleNames.Admin, true),
                    new Permission(PermissionNames.Edit, RoleNames.Admin, true),
                }.EncodePermissions()
            };
        }
    }
}
