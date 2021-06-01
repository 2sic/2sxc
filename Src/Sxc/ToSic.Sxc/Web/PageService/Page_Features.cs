using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web.PageService
{
    public partial class Page
    {

        /// <inheritdoc />
        public void Activate(params string[] keys)
        {
            Features.Activate(keys);
        }
        
        [PrivateApi]
        public IPageFeatures Features => _features ?? (_features = new PageFeatures.PageFeatures());
        private IPageFeatures _features;
    }
}
