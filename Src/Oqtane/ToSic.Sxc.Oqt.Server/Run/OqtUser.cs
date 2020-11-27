using System;
using System.Collections.Generic;
using Oqtane.Models;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;

// Todo: #Oqtane - just about everything

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtUser: IUser<User>
    {
        private readonly IUserResolver _userResolver;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtUser(IUserResolver userResolver) : this(WipConstants.NullUser)
        {
            _userResolver = userResolver;
        }

        public OqtUser(User user)
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

        public User UnwrappedContents
        {
            get => _unwrappedUser ??= _userResolver.GetUser();
            set => _unwrappedUser = value;
        }

        private User _unwrappedUser;

        public int Id => UnwrappedContents.UserId;

        public bool IsAnonymous => (UnwrappedContents?.UserId ?? -1) != -1;

    }
}
