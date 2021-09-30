using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Web
{
    [PrivateApi("for testing / un-implemented use")]
    public class LinkHelperUnknown: LinkHelper, IIsUnknown
    {
        public LinkHelperUnknown(ImgResizeLinker imgLinker, WarnUseOfUnknown<LinkHelperUnknown> warn) : base(imgLinker)
        {
        }

        protected override string ToImplementation(int? pageId = null, string parameters = null, string api = null)
        {
            return base.To(pageId: pageId, parameters: parameters, api: api);
        }

        // Mock DomainName
        public override string GetDomainName()
        {
            // use a pre-standardized dummy-domain  
            return "https://unknown.2sxc.org";
        }

        // Mock CurrentPage
        public override string GetCurrentRequestUrl()
        {
            return $"{GetDomainName()}/folder/subfolder/page?param=a#fragment";
        }
    }
}
