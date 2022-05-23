using System;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    public abstract class UserInformationServiceBase : HasLog, IUserInformationService
    {
        public static readonly UserInformationDto AnonymousUser = new UserInformationDto() { Id = -1, Name = Constants.Anonymous };


        public static readonly UserInformationDto UnknownUser = new UserInformationDto() { Id = -2, Name = Unknown };
        internal const string Unknown = "unknown";

        public const StringComparison StringComparison = System.StringComparison.InvariantCultureIgnoreCase;

        protected UserInformationServiceBase(LazyInitLog<IContextOfSite> context) : base($"{Constants.SxcLogName}.UsrInfoSrv")
        {
            _context = context.SetLog(Log);
        }
        private readonly LazyInitLog<IContextOfSite> _context;

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot.Log);
        }

        public abstract string PlatformIdentityTokenPrefix();

        public abstract UserInformationDto PlatformUserInformationDto(int userId);

        public UserInformationDto Find(string identityToken)
        {
            var wrapLog = Log.Fn<UserInformationDto>($"t:{identityToken}");

            var userId = UserId(identityToken);
            if (userId == AnonymousUser.Id) return wrapLog.Return(AnonymousUser, "ok");
            if (userId == UnknownUser.Id) return wrapLog.Return(UnknownUser, "err");

            var userDto = PlatformUserInformationDto(userId);

            return userDto != null ? wrapLog.Return(userDto, "ok") : wrapLog.Return(UnknownUser, "err");
        }

        /// <summary>
        /// Helper method to get SiteId.
        /// </summary>
        public int SiteId => _context.Ready.Site.Id;

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
