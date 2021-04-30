using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Oqtane.Security;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtUser: IUser<User>, ICmsUser
    {
        private readonly Lazy<IUserResolver> _userResolver;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IUserRoleRepository> _userRoleRepository;
        private readonly Lazy<IRoleRepository> _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteState _siteState;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtUser(
            Lazy<IUserResolver> userResolver,
            Lazy<IUserRepository> userRepository,
            Lazy<IUserRoleRepository> userRoleRepository,
            Lazy<IRoleRepository> roleRepository,
            IHttpContextAccessor httpContextAccessor,
            SiteState siteState) : this(WipConstants.NullUser)
        {
            _userResolver = userResolver;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor;
            _siteState = siteState;
        }

        public OqtUser(User user)
        {
            UnwrappedContents = user;
        }

        public User UnwrappedContents
        {
            get => _unwrappedUser ??= GetUser();
            set => _unwrappedUser = value;
        }
        private User _unwrappedUser;

        private User GetUser()
        {
            var identity = _userResolver.Value.GetUser();
            var user = _userRepository.Value.GetUser(identity.UserId);
            user.Roles = identity?.Roles;
            user.SiteId = _siteState?.Alias?.SiteId ?? identity.SiteId;
            return user;
        }

        public int Id => (UnwrappedContents?.UserId ?? OqtConstants.Unknown);

        public string IdentityToken => $"{OqtConstants.UserTokenPrefix}:{Id}";

        public Guid? Guid => GetUserGuid();

        public List<int> Roles => _roles ??= _userRoleRepository.Value.GetUserRoles(Id, UnwrappedContents.SiteId).Select(r => r.RoleId).ToList();
        private List<int> _roles;

        public bool IsSuperUser => _isSuperUser ??= UserSecurity.IsAuthorized(UnwrappedContents, RoleNames.Host);
        private bool? _isSuperUser;

        public bool IsAdmin => _isAdmin ??= UserSecurity.IsAuthorized(UnwrappedContents, RoleNames.Admin);
        private bool? _isAdmin;

        public bool IsDesigner => WipConstants.IsDesigner;

        #region New Permission properties for v12

        /// <inheritdoc />
        // This is a hopefully clearer implementation of what the user can do
        public bool IsSiteAdmin => IsAdmin;

        /// <inheritdoc />
        // This is a hopefully clearer implementation of what the user can do
        public bool IsSiteDeveloper => IsDesigner;

        /// <inheritdoc />
        // This is a hopefully clearer implementation of what the user can do
        public bool IsSystemAdmin => IsSuperUser;

        #endregion

        public bool IsAnonymous => Id == OqtConstants.Unknown;


        #region Private methods

        //private int GetRoleId(string roleName) => UnwrappedContents.SiteId > 0
        //    ? _roleRepository.Value.GetRoles(UnwrappedContents.SiteId)
        //        .FirstOrDefault(r => r.Name == roleName)?.RoleId ?? OqtConstants.Unknown
        //    : OqtConstants.Unknown;

        //private bool HasRoleId(int roleId) => Roles.Exists(item => item == roleId);

        private Guid? GetUserGuid()
        {
            try
            {
                var source = $"{_httpContextAccessor?.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value}";
                return new Guid(source);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
