using System;
using System.Collections.Generic;
using Oqtane.Models;
using ToSic.Eav.Run;
using ToSic.Sxc.OqtaneModule.Shared;
using ToSic.Sxc.OqtaneModule.Shared.Dev;

// Todo: #Oqtane - just about everything

namespace ToSic.Sxc.OqtaneModule.Server.Run
{
    public class OqtaneUser: IUser<User>
    {
        public OqtaneUser(User user)
        {
            UnwrappedContents = user;
        }

        public string IdentityToken => $"{OqtConstants.UserTokenPrefix}:{UnwrappedContents.UserId}";
        public Guid? Guid => WipConstants.UserGuid;
        public List<int> Roles => WipConstants.UserRoles;

        // temp: set to true, so we can see better errors

        public bool IsSuperUser => WipConstants.IsSuperUser;
        public bool IsAdmin => WipConstants.IsAdmin;
        public bool IsDesigner => WipConstants.IsDesigner;
        public User UnwrappedContents { get; }
    }
}
