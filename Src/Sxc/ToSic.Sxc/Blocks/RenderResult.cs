using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Blocks
{
    /// <inheritdoc />
    [PrivateApi]
    public class RenderResult : HybridHtmlString, IRenderResult
    {
        /// <inheritdoc />
        public string Html { get; set; }

        /// <inheritdoc />
        public IList<IPageFeature> Features { get; set; }

        /// <inheritdoc />
        public IList<IClientAsset> Assets { get; set; }

        /// <inheritdoc />
        public IList<PagePropertyChange> PageChanges { get; set; }

        /// <inheritdoc />
        public IList<HeadChange> HeadChanges { get; set; }

        /// <inheritdoc />
        public IList<IPageFeature> FeaturesFromSettings { get; set; }

        /// <inheritdoc />
        public int? HttpStatusCode { get; set; }

        /// <inheritdoc />
        public string HttpStatusMessage { get; set; }

        /// <inheritdoc />
        public IList<int> DependentApps { get; } = new List<int>();


        public int ModuleId { get; set; }

        public override string ToString() => Html;
    }
}
