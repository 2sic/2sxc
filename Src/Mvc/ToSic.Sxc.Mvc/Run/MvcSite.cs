using System.IO;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Context;
using ToSic.Eav.Run;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Context;
using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Mvc.Web;

// #todo: not really multi-tenant, incl. url 

namespace ToSic.Sxc.Mvc.Run
{
    /// <summary>
    /// This is a Mvc implementation of a Tenant-object. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public sealed class MvcSite : Site<object>, ICmsSite
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public MvcSite(IHttpContextAccessor httpContextAccessor)
        {
            UnwrappedContents = null;
            HttpContext = httpContextAccessor.HttpContext;
        }


        private HttpContext HttpContext { get; }

        public override ISite Init(int siteId)
        {
            _siteId = siteId;
            return this;
        }

        /// <inheritdoc />
        public override string DefaultCultureCode => TestIds.DefaultLanguage;

        /// <inheritdoc />
        public override string CurrentCultureCode => TestIds.DefaultLanguage;


        /// <inheritdoc />
        public override int Id => _siteId;

        private int _siteId;

        // https://localhost:44361
        public override string Url => HttpContext?.Request.Host.ToString();

        /// <inheritdoc />
        public override string Name => "Dummy Site Name";

        [PrivateApi]
        public override string AppsRootPhysical => AppsRootPartial();

        private string AppsRootPartial()
        {
            return Path.Combine(MvcConstants.WwwRoot, Settings.AppsRootFolder);
        }

        public override string AppsRootPhysicalFull => Eav.Factory.Resolve<IServerPaths>().FullAppPath(AppsRootPartial());


        /// <inheritdoc />
        public override string ContentPath => MvcConstants.WwwRoot;

        public override int ZoneId => _siteId;

    }
}
