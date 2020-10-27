using System.IO;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Mvc.TestStuff;

// #todo: not really multi-tenant, incl. url 

namespace ToSic.Sxc.Mvc.Run
{
    /// <summary>
    /// This is a Mvc implementation of a Tenant-object. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class MvcTenant : Tenant<MvcPortalSettings>
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public MvcTenant(IHttpContextAccessor httpContextAccessor) : base(new MvcPortalSettings())
        {
            HttpContext = httpContextAccessor.HttpContext;
        }

        /// <summary>
        /// Constructor for normal initialization
        /// </summary>
        /// <param name="httpContext"></param>
        public MvcTenant(HttpContext httpContext) : base(new MvcPortalSettings())
        {
            HttpContext = httpContext;
        }

        public MvcTenant(HttpContext httpContext, MvcPortalSettings settings) : base(
            settings ?? new MvcPortalSettings())
        {
            HttpContext = httpContext;
        }

        protected HttpContext HttpContext { get; }

        public override ITenant Init(int tenantId)
        {
            UnwrappedContents = new MvcPortalSettings(tenantId);
            return this;
        }

        /// <inheritdoc />
        public override string DefaultLanguage => UnwrappedContents.DefaultLanguage;

        /// <inheritdoc />
        public override int Id => UnwrappedContents.Id;

        // https://localhost:44361
        public override string Url => HttpContext?.Request.Host.ToString();

        /// <inheritdoc />
        public override string Name => UnwrappedContents.Name;

        [PrivateApi]
        public override string AppsRootPhysical => AppsRootPartial();

        private string AppsRootPartial()
        {
            return Path.Combine(UnwrappedContents.HomePath, Settings.AppsRootFolder);
        }

        public override string AppsRootPhysicalFull => Eav.Factory.Resolve<IServerPaths>().FullAppPath(AppsRootPartial());

        [PrivateApi]
        public override bool RefactorUserIsAdmin => false;

        /// <inheritdoc />
        public override string ContentPath => UnwrappedContents.HomePath;

        public override int ZoneId => UnwrappedContents.Id;

    }
}
