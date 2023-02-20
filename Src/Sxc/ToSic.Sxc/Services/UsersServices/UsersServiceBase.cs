using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context.Raw;
using static System.StringComparison;

namespace ToSic.Sxc.Services
{
    public abstract class UsersServiceBase : ServiceForDynamicCode, IUsersService
    {

        protected UsersServiceBase(LazySvc<IContextOfSite> context) : base($"{Constants.SxcLogName}.UsrInfoSrv")
        {
            ConnectServices(
                _context = context
            );
        }

        private readonly LazySvc<IContextOfSite> _context;

        public abstract string PlatformIdentityTokenPrefix { get; }

        public abstract IUser PlatformUserInformationDto(int userId);

        public IUser Get(string identityToken) => Log.Func($"t:{identityToken}", () =>
        {
            var userId = UserId(identityToken);
            return Get(userId);
        });

        public IUser Get(int userId) => Log.Func($"{userId}", () =>
        {
            if (userId == CmsUserRaw.AnonymousUser.Id) return (CmsUserRaw.AnonymousUser, "ok");
            if (userId == CmsUserRaw.UnknownUser.Id) return (CmsUserRaw.UnknownUser, "err");

            var userDto = PlatformUserInformationDto(userId);

            return userDto != null
                ? (userDto, "ok")
                : (CmsUserRaw.UnknownUser, "err");
        });

        /// <summary>
        /// Helper method to get SiteId.
        /// </summary>
        protected int SiteId => _context.Value.Site.Id;

        /// <summary>
        /// Helper method to parse UserID from user identity token.
        /// </summary>
        /// <param name="identityToken"></param>
        /// <returns></returns>
        private int UserId(string identityToken) => Log.Func($"t:{identityToken}", () =>
        {
            if (string.IsNullOrWhiteSpace(identityToken))
                return (CmsUserRaw.UnknownUser.Id, "empty identity token");

            if (identityToken.EqualsInsensitive(Constants.Anonymous))
                return (CmsUserRaw.AnonymousUser.Id, "ok (anonymous)");

            var prefix = PlatformIdentityTokenPrefix;
            if (identityToken.StartsWith(prefix, InvariantCultureIgnoreCase))
                identityToken = identityToken.Substring(prefix.Length);

            return int.TryParse(identityToken, out var userId)
                ? (userId, $"ok (u:{userId})")
                : (CmsUserRaw.UnknownUser.Id, "err");
        });
    }
}
