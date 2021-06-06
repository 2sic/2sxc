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
        public IPageFeatures Features { get; } // => _features ?? (_features = new PageFeatures.PageFeatures(this));
        // private IPageFeatures _features; 
    }
}
