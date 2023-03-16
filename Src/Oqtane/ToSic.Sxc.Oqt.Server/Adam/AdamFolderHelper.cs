using Oqtane.Extensions;
using Oqtane.Models;
using Oqtane.Shared;
using System.Collections.Generic;
using System.Reflection;
using ToSic.Sxc.Oqt.Server.Integration;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    public class AdamFolderHelper
    {
        private static List<Permission> _permissionList;
        private static bool? _hasPermissionList;

        public static Folder NewVirtualFolder(int siteId, int? parentId, string path, string folder)
        {
            var newVirtualFolder = new Folder()
            {
                SiteId = siteId,
                ParentId = parentId,
                Name = folder,
                Path = path.EnsureOqtaneFolderFormat(),
                Order = 1,
                IsSystem = false,
                Type = FolderTypes.Private
            };

            // default permission list
            _permissionList ??= new List<Permission>
            {
                new(PermissionNames.Browse, RoleNames.Everyone, true),
                new(PermissionNames.View, RoleNames.Everyone, true),
                new(PermissionNames.Browse, RoleNames.Admin, true),
                new(PermissionNames.View, RoleNames.Admin, true),
                new(PermissionNames.Edit, RoleNames.Admin, true),
            };

            // check if the class has a "PermissionList" property ( Oqtane >= 3.4.0)
            _hasPermissionList ??= typeof(Folder).GetProperty(nameof(Folder.PermissionList)) != null;

            PropertyInfo propInfo;
            if (_hasPermissionList == true)
            {
                propInfo = newVirtualFolder.GetType().GetProperty(nameof(Folder.PermissionList));
                propInfo?.SetValue(newVirtualFolder, _permissionList);
            }
            else
            {
                // for Oqtane < 3.4.0
                propInfo = newVirtualFolder.GetType().GetProperty(nameof(Folder.Permissions));
                propInfo?.SetValue(newVirtualFolder, _permissionList.EncodePermissions());
            }

            return newVirtualFolder;
        }
    }
}
