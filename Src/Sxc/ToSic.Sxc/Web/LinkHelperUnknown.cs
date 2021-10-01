using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Web
{
    [PrivateApi("for testing / un-implemented use")]
    public class LinkHelperUnknown: LinkHelper, IIsUnknown
    {
        public const string MockHost = "https://unknown.2sxc.org";


        public LinkHelperUnknown(ImgResizeLinker imgLinker, WarnUseOfUnknown<LinkHelperUnknown> warn) : base(imgLinker)
        {
        }

        protected override string ToImplementation(int? pageId = null, string parameters = null, string api = null)
        {
            if (!string.IsNullOrEmpty(parameters)) parameters = $"?{parameters}";

            // Page or Api?
            return api == null ? $"{GetDomainName()}/page{pageId}{parameters}" : $"{api}{parameters}";
        }

        // Mock DomainName
        public override string GetDomainName()
        {
            // use a pre-standardized dummy-domain  
            return MockHost;
        }

        // Mock CurrentPage
        public override string GetCurrentRequestUrl()
        {
            return $"{GetDomainName()}/folder/subfolder/page?param=a#fragment";
        }
    }
}
