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
using ToSic.Lib;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context
{
    public class OqtUser: ServiceBase, IUser<User>
    {
        private readonly LazySvc<IUserRepository> _userRepository;
        private readonly LazySvc<IUserRoleRepository> _userRoleRepository;
        private readonly LazySvc<UserManager<IdentityUser>> _identityUserManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteState _siteState;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtUser(LazySvc<IUserRepository> userRepository,
            LazySvc<IUserRoleRepository> userRoleRepository,
            LazySvc<UserManager<IdentityUser>> identityUserManager,
            IHttpContextAccessor httpContextAccessor,
            SiteState siteState): base(OqtConstants.OqtLogPrefix + ".User")
        {
            ConnectServices(
                _userRepository = userRepository,
                _userRoleRepository = userRoleRepository,
                _identityUserManager = identityUserManager,
                _httpContextAccessor = httpContextAccessor,
                _siteState = siteState
            );
        }

        protected User UnwrappedUser => _unwrappedUser.Get(GetUser);
        private readonly GetOnce<User> _unwrappedUser = new();
        public User GetContents() => UnwrappedUser;

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

        public int Id => UnwrappedUser?.UserId ?? -1;

        public string Username => UnwrappedUser?.Username;

        public string Name => UnwrappedUser?.DisplayName;

        public string Email => UnwrappedUser?.Email;

        public string IdentityToken => $"{OqtConstants.UserTokenPrefix}{Id}";

        public Guid Guid { get; private set; }

        public List<int> Roles => _roles ??= _userRoleRepository.Value.GetUserRoles(Id, UnwrappedUser.SiteId).Select(r => r.RoleId).ToList();
        private List<int> _roles;


        public bool IsSystemAdmin => _isSystemAdmin ??= UserSecurity.IsAuthorized(UnwrappedUser, RoleNames.Host);
        private bool? _isSystemAdmin;

        public bool IsSiteAdmin => _isSiteAdmin ??= UserSecurity.IsAuthorized(UnwrappedUser, RoleNames.Admin);
        private bool? _isSiteAdmin;

        public bool IsContentAdmin => IsSiteAdmin;

        public bool IsSiteDeveloper => IsSystemAdmin;

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

        public Guid UserGuidFromIdentity()
        {
            var username = _httpContextAccessor.HttpContext!.User.Identity!.Name;
            return string.IsNullOrEmpty(username)
                ? default
                : new((_identityUserManager.Value.FindByNameAsync(username).Result).Id);
        }

        #endregion

        #region Deprecated in v15

        //[Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
        //public bool IsSuperUser => IsSystemAdmin;

        //[Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
        //public bool IsAdmin => IsSiteAdmin;


        #endregion
    }
}
