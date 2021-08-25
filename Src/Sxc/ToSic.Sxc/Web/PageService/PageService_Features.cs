using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {

        /// <inheritdoc />
        public void Activate(params string[] keys) => Features.Activate(keys);

        [PrivateApi]
        public IPageFeatures Features { get; }
    }
}
