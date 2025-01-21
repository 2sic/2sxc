using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Cms.Users.Internal;
using ToSic.Sxc.Internal;
using static System.StringComparison;

namespace ToSic.Sxc.Services.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class UsersService(LazySvc<IContextOfSite> context, IUsersProvider provider)
    : ServiceForDynamicCode($"{SxcLogName}.UsrInfoSrv", connect: [context, provider]), IUserService
{
    public IUserModel Get(string nameId)
    {
        var l = Log.Fn<IUserModel>($"token:{nameId}");

        var userId = UserId(nameId);

        return userId.SpecialUser != null
            ? l.Return(userId.SpecialUser, "special user")
            : l.Return(Get(userId.UserId));
    }

    public IUserModel Get(int userId) 
    {
        var l = Log.Fn<IUserModel>($"id:{userId}");

        var unknown = UserConstants.UnknownUser;
        var anon = UserConstants.AnonymousUser;
        if (userId == anon.Id)
            return l.Return(anon, "anonymous");

        if (userId == unknown.Id)
            return l.Return(unknown, "unknown");

        var userDto = provider.GetUser(userId, context.Value.Site.Id);

        return userDto != null
            ? l.ReturnAsOk(userDto)
            : l.Return(unknown, "err");
    }

    /// <summary>
    /// Helper method to parse UserID from user identity token.
    /// </summary>
    /// <param name="identityToken"></param>
    /// <returns></returns>
    private (IUserModel SpecialUser, int UserId) UserId(string identityToken) 
    {
        var l = Log.Fn<(IUserModel, int)>($"token:{identityToken}");

        var unknown = UserConstants.UnknownUser;
        var anon = UserConstants.AnonymousUser;
        if (string.IsNullOrWhiteSpace(identityToken))
            return l.Return((unknown, unknown.Id), "empty identity token");

        if (identityToken.EqualsInsensitive(SxcUserConstants.Anonymous))
            return l.Return((anon, anon.Id), "ok (anonymous)");

        var prefix = provider.PlatformIdentityTokenPrefix;
        if (identityToken.StartsWith(prefix, InvariantCultureIgnoreCase))
            identityToken = identityToken.Substring(prefix.Length);

        return int.TryParse(identityToken, out var userId)
            ? l.Return((null, userId), $"ok (u:{userId})")
            : l.Return((unknown, unknown.Id), "err");
    }
}