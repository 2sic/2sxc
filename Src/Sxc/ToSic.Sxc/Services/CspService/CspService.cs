using ToSic.Eav.Configuration.Licenses;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Very experimental, do not use
    /// </summary>
    [PrivateApi]
    public class CspService : ICspService
    {
        public const string CspName = "Content-Security-Policy";
        public const string CspReport = "Content-Security-Policy-Report-Only";
        public CspService(PageServiceShared page, IFeaturesService features, ILicenseService licenses)
        {
            _page = page;
            _features = features;
            _licenses = licenses;
        }
        private readonly PageServiceShared _page;
        private readonly IFeaturesService _features;
        private readonly ILicenseService _licenses;

        //private bool Enabled => _enabled.Get(() => _licenses.IsEnabled(BuiltIn.CoreBeta));
        //private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();

        public void Activate()
        {
            //if (!Enabled) return;

            // var header = "Content-Security-Policy: test;"
            var addHeader = "TEST: 2sxc cps service";
            _page.AddToHttp(addHeader);
        }

        public void AddPolicy(string policy) => Test(CspName, policy);

        public void AddPolicy(CspParameters policy) => Test(CspName, policy.ToString());

        public void AddReport(string policy) => Test(CspReport, policy);
        public void AddReport(CspParameters policy) => Test(CspReport, policy.ToString());


        public void Test(string name, string value)
        {
            //if (!Enabled) return;

            var addHeader = name + ": " + value;
            _page.AddToHttp(addHeader);
            _page.AddToHttp(name + "-Note: added by beta 2sxc CSP service");
        }

        public CspParameters Build(
            string noParamOrder = Eav.Parameters.Protector,
            string defaultSrc = null,
            string xyz = null
        )
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(Build), $"todo");

            var cspParams = new CspParameters();
            if (defaultSrc != null)
            {
                var defSrcTemp = defaultSrc.Trim('\'').ToLowerInvariant();
                cspParams.Add("default-src", defSrcTemp == "self" ? "'self'" : defaultSrc);
            }

            return cspParams;
        }
    }
}
