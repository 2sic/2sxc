using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using ToSic.Eav.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context
{
    public class OqtUser: IUser<User>
    {
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IUserRoleRepository> _userRoleRepository;
        private readonly Lazy<UserManager<IdentityUser>> _identityUserManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteState _siteState;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtUser(Lazy<IUserRepository> userRepository,
            Lazy<IUserRoleRepository> userRoleRepository,
            Lazy<UserManager<IdentityUser>> identityUserManager,
            IHttpContextAccessor httpContextAccessor,
            SiteState siteState)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _identityUserManager = identityUserManager;
            _httpContextAccessor = httpContextAccessor;
            _siteState = siteState;
        }

        public User UnwrappedContents
        {
            get => _unwrappedUser ??= GetUser();
            set => _unwrappedUser = value;
        }
        private User _unwrappedUser;
        public User GetContents() => UnwrappedContents;

        private User GetUser()
        {
            var identity = GetUserFromIdentity();
            if (identity.UserId == -1) return identity;
            var user = _userRepository.Value.GetUser(identity.UserId);
            user.Roles = identity.Roles;
            // siteId is not user info, but comes from env
            user.SiteId = _siteState?.Alias?.SiteId ?? identity.SiteId;
            return user;
        }

        public int Id => UnwrappedContents?.UserId ?? -1;

        public string Username => UnwrappedContents?.Username;

        public string Name => UnwrappedContents?.DisplayName;

        public string Email => UnwrappedContents?.Email;

        public string IdentityToken => $"{OqtConstants.UserTokenPrefix}:{Id}";

        public Guid? Guid { get; private set; }

        public List<int> Roles => _roles ??= _userRoleRepository.Value.GetUserRoles(Id, UnwrappedContents.SiteId).Select(r => r.RoleId).ToList();
        private List<int> _roles;


        public bool IsSystemAdmin => _isSystemAdmin ??= UserSecurity.IsAuthorized(UnwrappedContents, RoleNames.Host);
        private bool? _isSystemAdmin;

        [Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
        public bool IsSuperUser => IsSystemAdmin;

        [Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
        public bool IsAdmin => IsSiteAdmin;
        public bool IsSiteAdmin => _isSiteAdmin ??= UserSecurity.IsAuthorized(UnwrappedContents, RoleNames.Admin);
        private bool? _isSiteAdmin;

        public bool IsContentAdmin => IsSiteAdmin;

        public bool IsDesigner => IsSystemAdmin;

        #region New Permission properties for v12

        ///// <inheritdoc />
        //// This is a hopefully clearer implementation of what the user can do
        //public bool IsSiteAdmin => IsAdmin;

        ///// <inheritdoc />
        //// This is a hopefully clearer implementation of what the user can do
        //public bool IsSiteDeveloper => IsDesigner;

        ///// <inheritdoc />
        //// This is a hopefully clearer implementation of what the user can do
        //public bool IsSystemAdmin => IsSuperUser;

        #endregion

        public bool IsAnonymous => Id == -1;


        #region Private methods

        public User GetUserFromIdentity()
        {
            var user = new User
            {
                IsAuthenticated = false, 
                Username = string.Empty, 
                UserId = -1, 
                Roles = string.Empty
            };

            // missing identity from http context
            if (_httpContextAccessor?.HttpContext?.User?.Identity == null) return user;

            // user not auth
            user.IsAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            if (user.IsAuthenticated == false) return user;

            user.Username = _httpContextAccessor.HttpContext.User.Identity.Name;
            user.UserId = UserIdFromClaims();
            user.Roles = UserRolesFromClaims();

            Guid = UserGuidFromIdentity();
            return user;
        }

        private int UserIdFromClaims() 
            => int.Parse(_httpContextAccessor.HttpContext!.User.Claims.First(item => item.Type is ClaimTypes.NameIdentifier or ClaimTypes.PrimarySid).Value);

        private string UserRolesFromClaims()
        {
            var roles = _httpContextAccessor.HttpContext!.User.Claims.Where(item => item.Type == ClaimTypes.Role)
                .Aggregate("", (current, claim) => current + (claim.Value + ";"));
            if (roles != "") roles = ";" + roles;
            return roles;
        }

        public Guid? UserGuidFromIdentity()
        {
            var username = _httpContextAccessor.HttpContext!.User.Identity!.Name;
            if (string.IsNullOrEmpty(username)) return null;
            return new Guid((_identityUserManager.Value.FindByNameAsync(username).Result).Id);
        }

        #endregion
    }
}
