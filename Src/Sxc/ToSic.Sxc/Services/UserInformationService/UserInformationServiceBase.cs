using System;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    public abstract class UserInformationServiceBase : HasLog, IUserInformationService
    {
        public static readonly UserInformationDto UserUnknown = new UserInformationDto() { Id = -1, Name = Unknown };
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

        public abstract UserInformationDto Find(string identityToken);

        /// <summary>
        /// Helper method to get SiteId.
        /// </summary>
        public int SiteId => _context.Ready.Site.Id;

        /// <summary>
        /// Helper method to parse UserID from user identity token.
        /// </summary>
        /// <param name="identityToken"></param>
        /// <returns></returns>
        public int UserId(string identityToken)
        {
            var wrapLog = Log.Call<int>($"t:{identityToken}");

            if (string.IsNullOrEmpty(identityToken) || identityToken.Equals(Constants.Anonymous, StringComparison))
                return wrapLog("Ok (anonymous/null)", -1);

            if (identityToken.StartsWith(PlatformIdentityTokenPrefix(), StringComparison))
                identityToken = identityToken.Substring(PlatformIdentityTokenPrefix().Length);

            return int.TryParse(identityToken, out var userId) ? wrapLog($"Ok (u:{userId})", userId) : wrapLog("Err", -1);
        }
    }
}
