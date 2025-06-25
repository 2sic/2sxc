using System.Collections.Specialized;
using ToSic.Sxc.Web.Sys.Url;

namespace ToSic.Sxc.Web.Sys.ContentSecurityPolicy;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CspParameters: NameValueCollection
{
    public CspParameters() { }

    public CspParameters(NameValueCollection originals) : base(originals) { }

    public override string ToString() =>
        UrlHelpers.NvcToString(this, " ", "; ", ";", "", false, " ");
}