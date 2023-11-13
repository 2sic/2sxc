using System.Collections.Specialized;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class CspParameters: NameValueCollection
    {
        public CspParameters() { }

        public CspParameters(NameValueCollection originals) : base(originals)
        {
        }

        public override string ToString() => UrlHelpers.NvcToString(this, " ", "; ", ";", "", false, " ");
    }
}
