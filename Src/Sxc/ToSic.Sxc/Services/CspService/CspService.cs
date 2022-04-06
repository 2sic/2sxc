using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.PageService;


namespace ToSic.Sxc.Services.CspService
{
    /// <summary>
    /// Very experimental, do not use
    /// </summary>
    [PrivateApi]
    public class CspService
    {

        public CspService(PageServiceShared page, IFeaturesService features)
        {
            _page = page;
            _features = features;
        }
        private readonly PageServiceShared _page;
        private readonly IFeaturesService _features;

        //private bool Enabled =>
        //    _enabled.Get(() => _features.IsEnabled(FeaturesBuiltIn.beta.BlockFileResolveOutsideOfEntityAdam.NameId));
        //private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();

        public void Activate()
        {
            //if (!Enabled) return;

            // var header = "Content-Security-Policy: test;"
            var addHeader = "TEST: 2sxc cps service";
            _page.AddToHttp(addHeader);
        }
    }
}
