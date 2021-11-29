using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// WIP - should contain everything which results from a render
    /// Incl. all the features that are activated, page changes etc.
    /// It's kind of like a bundle of things the CMS must then do to deliver to the page
    /// </summary>
    [PrivateApi]
    public class RenderResult
    {
        /// <summary>
        /// True if the work has been done and this is populated
        /// </summary>
        public bool Ready;

        /// <summary>
        /// The resulting HTML to add to the page
        /// </summary>
        public string Html;

        /// <summary>
        /// The features which are activated
        /// </summary>
        public List<IPageFeature> Features;

        /// <summary>
        /// Assets which must be added to the page
        /// </summary>
        public List<ClientAssetInfo> Assets;

        public IList<PagePropertyChange> PageChanges;

        public IList<HeadChange> HeadChanges;

        public IList<IPageFeature> ManualChanges;

        public int? HttpStatusCode;
        public string HttpStatusMessage;


        /// <summary>
        /// WIP Additional infos
        /// </summary>
        public List<int> DependentApps = new List<int>();

        public int ModuleId;
    }
}
