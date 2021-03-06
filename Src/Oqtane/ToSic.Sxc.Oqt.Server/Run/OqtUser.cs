﻿using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtUser: IUser<User>, ICmsUser
    {
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IUserRoleRepository> _userRoleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteState _siteState;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtUser(Lazy<IUserRepository> userRepository,
            Lazy<IUserRoleRepository> userRoleRepository,
            IHttpContextAccessor httpContextAccessor,
            SiteState siteState) /*: this(WipConstants.NullUser)*/
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _httpContextAccessor = httpContextAccessor;
            _siteState = siteState;
        }

        //public OqtUser(User user)
        //{
        //    UnwrappedContents = user;
        //}

        public User UnwrappedContents
        {
            get => _unwrappedUser ??= GetUser();
            set => _unwrappedUser = value;
        }
        private User _unwrappedUser;

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

        public string IdentityToken => $"{OqtConstants.UserTokenPrefix}:{Id}";

        public Guid? Guid { get; private set; }

        public List<int> Roles => _roles ??= _userRoleRepository.Value.GetUserRoles(Id, UnwrappedContents.SiteId).Select(r => r.RoleId).ToList();
        private List<int> _roles;

        public bool IsSuperUser => _isSuperUser ??= UserSecurity.IsAuthorized(UnwrappedContents, RoleNames.Host);
        private bool? _isSuperUser;

        public bool IsAdmin => _isAdmin ??= UserSecurity.IsAuthorized(UnwrappedContents, RoleNames.Admin);
        private bool? _isAdmin;

        public bool IsDesigner => IsSuperUser;

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

        public bool IsAnonymous => Id == -1;


        #region Private methods

        public User GetUserFromIdentity()
        {
            var user = new User { IsAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false, Username = "", UserId = -1, Roles = "" };
            if (!user.IsAuthenticated) return user;

            user.Username = _httpContextAccessor.HttpContext.User.Identity.Name;
            user.UserId = int.Parse(_httpContextAccessor.HttpContext.User.Claims.First(item => item.Type == ClaimTypes.PrimarySid).Value);
            var roles = _httpContextAccessor.HttpContext.User.Claims.Where(item => item.Type == ClaimTypes.Role).Aggregate("", (current, claim) => current + (claim.Value + ";"));
            if (roles != "") roles = ";" + roles;
            user.Roles = roles;
            
            Guid = GetUserGuid();
            return user;
        }

        public Guid? GetUserGuid()
        {
            // Sometimes user guid is not available.
            var guidValue = $"{_httpContextAccessor?.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}";
            return System.Guid.TryParse(guidValue, out var result) ? result : null;
        }

        #endregion
    }
}
