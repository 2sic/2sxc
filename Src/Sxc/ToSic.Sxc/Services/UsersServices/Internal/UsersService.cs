using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Context.Internal.Raw;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Models;
using static System.StringComparison;

namespace ToSic.Sxc.Services.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class UsersService(LazySvc<IContextOfSite> context, IUsersProvider provider)
    : ServiceForDynamicCode($"{SxcLogName}.UsrInfoSrv", connect: [context, provider]), IUserService
{
    public IUserModel Get(string identityToken)
    {
        var l = Log.Fn<IUserModel>($"token:{identityToken}");

        var userId = UserId(identityToken);

        return userId.SpecialUser != null
            ? l.Return(userId.SpecialUser, "special user")
            : l.Return(Get(userId.UserId));
    }

    public IUserModel Get(int userId) 
    {
        var l = Log.Fn<IUserModel>($"id:{userId}");

        if (userId == CmsUserRaw.AnonymousUser.Id)
            return l.Return(CmsUserRaw.AnonymousUser, "anonymous");

        if (userId == CmsUserRaw.UnknownUser.Id)
            return l.Return(CmsUserRaw.UnknownUser, "unknown");

        var userDto = provider.GetUser(userId, context.Value.Site.Id);

        return userDto != null
            ? l.ReturnAsOk(userDto)
            : l.Return(CmsUserRaw.UnknownUser, "err");
    }

    /// <summary>
    /// Helper method to parse UserID from user identity token.
    /// </summary>
    /// <param name="identityToken"></param>
    /// <returns></returns>
    private (IUserModel SpecialUser, int UserId) UserId(string identityToken) 
    {
        var l = Log.Fn<(IUserModel, int)>($"token:{identityToken}");

        if (string.IsNullOrWhiteSpace(identityToken))
            return l.Return((CmsUserRaw.UnknownUser, CmsUserRaw.UnknownUser.Id), "empty identity token");

        if (identityToken.EqualsInsensitive(SxcUserConstants.Anonymous))
            return l.Return((CmsUserRaw.AnonymousUser, CmsUserRaw.AnonymousUser.Id), "ok (anonymous)");

        var prefix = provider.PlatformIdentityTokenPrefix;
        if (identityToken.StartsWith(prefix, InvariantCultureIgnoreCase))
            identityToken = identityToken.Substring(prefix.Length);

        return int.TryParse(identityToken, out var userId)
            ? l.Return((null, userId), $"ok (u:{userId})")
            : l.Return((CmsUserRaw.UnknownUser, CmsUserRaw.UnknownUser.Id), "err");
    }
}