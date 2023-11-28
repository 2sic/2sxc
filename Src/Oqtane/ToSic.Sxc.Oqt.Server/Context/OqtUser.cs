using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context;

public class OqtUser: ServiceBase, IUser<User>
{
    private readonly LazySvc<IUserRepository> _userRepository;
    private readonly LazySvc<OqtSecurity> _oqtSecurity;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SiteState _siteState;

    /// <summary>
    /// Constructor for DI
    /// </summary>
    public OqtUser(LazySvc<IUserRepository> userRepository,
        LazySvc<OqtSecurity> oqtSecurity,
        IHttpContextAccessor httpContextAccessor,
        SiteState siteState): base($"{OqtConstants.OqtLogPrefix}.User")
    {
        ConnectServices(
            _userRepository = userRepository,
            _oqtSecurity = oqtSecurity,
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

    public int Id => _oqtSecurity.Value.Id(UnwrappedUser);

    public string Username => _oqtSecurity.Value.Username(UnwrappedUser);

    public string Name => _oqtSecurity.Value.Name(UnwrappedUser);

    public string Email => _oqtSecurity.Value.Email(UnwrappedUser);

    public string IdentityToken => _oqtSecurity.Value.UserIdentityToken(UnwrappedUser);

    public Guid Guid { get; private set; }

    public List<int> Roles => _roles.Get(() => _oqtSecurity.Value.Roles(UnwrappedUser));
    private readonly GetOnce<List<int>> _roles = new();

    public bool IsSystemAdmin => _isSystemAdmin.Get(() => _oqtSecurity.Value.IsSystemAdmin(UnwrappedUser));
    private readonly GetOnce<bool> _isSystemAdmin = new();

    public bool IsSiteAdmin => _isSiteAdmin.Get(() => _oqtSecurity.Value.IsSiteAdmin(UnwrappedUser));
    private readonly GetOnce<bool> _isSiteAdmin = new();

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

    public bool IsAnonymous => _oqtSecurity.Value.IsAnonymous(UnwrappedUser);


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
            : _oqtSecurity.Value.UserGuid(username);
    }

    #endregion

    #region Deprecated in v15

    //[Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
    //public bool IsSuperUser => IsSystemAdmin;

    //[Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
    //public bool IsAdmin => IsSiteAdmin;


    #endregion
}