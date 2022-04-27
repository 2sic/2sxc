using ToSic.Eav.Configuration.Licenses;
using ToSic.Eav.Documentation;
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
        public const string CspHeaderNamePolicy = "Content-Security-Policy";
        public const string CspHeaderNameReport = "Content-Security-Policy-Report-Only";
        public CspService(PageServiceShared page, IFeaturesService features, ILicenseService licenses)
        {
            _page = page;
            page.AddCspService(this);
            _features = features;
            _licenses = licenses;
        }
        private readonly PageServiceShared _page;
        private readonly IFeaturesService _features;
        private readonly ILicenseService _licenses;
        public CspParameters Policy = new CspParameters();

        public bool ReportOnly { get; set; } = true;

        public string Name => ReportOnly ? CspHeaderNameReport : CspHeaderNamePolicy;

        //private bool Enabled => _enabled.Get(() => _licenses.IsEnabled(BuiltIn.CoreBeta));
        //private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();

        public void Activate()
        {
            //if (!Enabled) return;

            _page.AddToHttp("TEST", "2SXC cps service");
        }

        public void AddPolicy(string policy) => Test(CspHeaderNamePolicy, policy);

        //public void AddPolicy(CspParameters policy) => Test(CspHeaderNamePolicy, policy.ToString());

        public void Add(string name, params string[] values)
        {
            foreach (var v in values) Policy.Add(name, v);
        }

        public void AddReport(string policy) => Test(CspHeaderNameReport, policy);

        //public void AddReport(CspParameters policy) => Test(CspHeaderNameReport, policy.ToString());

        public void Test(string name, string value)
        {
            // If not enabled, just ignore this and don't make changes (WIP)
            // Don't error if not enabled.
            //if (!Enabled) return;
            _page.AddToHttp(name, value);
            _page.AddToHttp(name + "-Note", "added by beta 2sxc CSP service");
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
