using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Security.Claims;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context;

internal class OqtUser(
    LazySvc<IUserRepository> userRepository,
    LazySvc<OqtSecurity> oqtSecurity,
    IHttpContextAccessor httpContextAccessor,
    SiteState siteState)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.User",
        connect: [userRepository, oqtSecurity, httpContextAccessor, siteState]), IUser<User>
{
    protected User UnwrappedUser => _unwrappedUser.Get(GetUser);
    private readonly GetOnce<User> _unwrappedUser = new();
    public User GetContents() => UnwrappedUser;

    private User GetUser()
    {
        var identity = GetUserFromIdentity();
        if (identity.UserId == -1) return identity;
        var user = userRepository.Value.GetUser(identity.UserId);
        user.Roles = identity.Roles;
        // siteId is not user info, but comes from env
        user.SiteId = siteState?.Alias?.SiteId ?? identity.SiteId;
        return user;
    }

    public int Id => oqtSecurity.Value.Id(UnwrappedUser);

    public string Username => oqtSecurity.Value.Username(UnwrappedUser);

    public string Name => oqtSecurity.Value.Name(UnwrappedUser);

    public string Email => oqtSecurity.Value.Email(UnwrappedUser);

    public string IdentityToken => oqtSecurity.Value.UserIdentityToken(UnwrappedUser);

    public Guid Guid { get; private set; }

    public List<int> Roles => _roles.Get(() => oqtSecurity.Value.Roles(UnwrappedUser));
    private readonly GetOnce<List<int>> _roles = new();

    public bool IsSystemAdmin => _isSystemAdmin.Get(() => oqtSecurity.Value.IsSystemAdmin(UnwrappedUser));
    private readonly GetOnce<bool> _isSystemAdmin = new();

    public bool IsSiteAdmin => _isSiteAdmin.Get(() => oqtSecurity.Value.IsSiteAdmin(UnwrappedUser));
    private readonly GetOnce<bool> _isSiteAdmin = new();

    public bool IsContentAdmin => IsSiteAdmin;

    public bool IsContentEditor => IsSiteAdmin;

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

    public bool IsAnonymous => oqtSecurity.Value.IsAnonymous(UnwrappedUser);


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
        if (httpContextAccessor?.HttpContext?.User?.Identity == null) return user;

        // user not auth
        user.IsAuthenticated = httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        if (user.IsAuthenticated == false) return user;

        user.Username = httpContextAccessor.HttpContext.User.Identity.Name;
        user.UserId = UserIdFromClaims();
        user.Roles = UserRolesFromClaims();

        Guid = UserGuidFromIdentity();
        return user;
    }

    private int UserIdFromClaims() 
        => int.Parse(httpContextAccessor.HttpContext!.User.Claims.First(item => item.Type is ClaimTypes.NameIdentifier or ClaimTypes.PrimarySid).Value);

    private string UserRolesFromClaims()
    {
        var roles = httpContextAccessor.HttpContext!.User.Claims.Where(item => item.Type == ClaimTypes.Role)
            .Aggregate("", (current, claim) => current + (claim.Value + ";"));
        if (roles != "") roles = ";" + roles;
        return roles;
    }

    public Guid UserGuidFromIdentity()
    {
        var username = httpContextAccessor.HttpContext!.User.Identity!.Name;
        return string.IsNullOrEmpty(username)
            ? default
            : oqtSecurity.Value.UserGuid(username);
    }

    #endregion

    #region Deprecated in v15

    //[Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
    //public bool IsSuperUser => IsSystemAdmin;

    //[Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
    //public bool IsAdmin => IsSiteAdmin;


    #endregion
}