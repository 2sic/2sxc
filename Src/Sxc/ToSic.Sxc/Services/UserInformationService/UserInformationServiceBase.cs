using System;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Services
{
    public abstract class UserInformationServiceBase : ServiceForDynamicCode, IUserInformationService
    {
        public static readonly UserInformationDto AnonymousUser = new UserInformationDto { Id = -1, Name = Constants.Anonymous };


        public static readonly UserInformationDto UnknownUser = new UserInformationDto { Id = -2, Name = Unknown };
        internal const string Unknown = "unknown";

        public const StringComparison StringComparison = System.StringComparison.InvariantCultureIgnoreCase;

        protected UserInformationServiceBase(LazySvc<IContextOfSite> context) : base($"{Constants.SxcLogName}.UsrInfoSrv") =>
            ConnectServices(
                _context = context
            );
        private readonly LazySvc<IContextOfSite> _context;

        public abstract string PlatformIdentityTokenPrefix();

        public abstract UserInformationDto PlatformUserInformationDto(int userId);

        public UserInformationDto Find(string identityToken)
        {
            var wrapLog = Log.Fn<UserInformationDto>($"t:{identityToken}");

            var userId = UserId(identityToken);
            if (userId == AnonymousUser.Id) return wrapLog.ReturnAsOk(AnonymousUser);
            if (userId == UnknownUser.Id) return wrapLog.Return(UnknownUser, "err");

            var userDto = PlatformUserInformationDto(userId);

            return userDto != null ? wrapLog.ReturnAsOk(userDto) : wrapLog.Return(UnknownUser, "err");
        }

        /// <summary>
        /// Helper method to get SiteId.
        /// </summary>
        public int SiteId => _context.Value.Site.Id;

        /// <summary>
        /// Helper method to parse UserID from user identity token.
        /// </summary>
        /// <param name="identityToken"></param>
        /// <returns></returns>
        private int UserId(string identityToken)
        {
            var wrapLog = Log.Fn<int>($"t:{identityToken}");

            if (string.IsNullOrEmpty(identityToken))
                return wrapLog.Return(UnknownUser.Id, "err");

            if (identityToken.Equals(Constants.Anonymous, StringComparison))
                return wrapLog.Return(AnonymousUser.Id, "ok (anonymous)");

            if (identityToken.StartsWith(PlatformIdentityTokenPrefix(), StringComparison))
                identityToken = identityToken.Substring(PlatformIdentityTokenPrefix().Length);

            return int.TryParse(identityToken, out var userId) 
                ? wrapLog.Return(userId, $"ok (u:{userId})") 
                : wrapLog.Return(UnknownUser.Id, "err");
        }
    }
}
