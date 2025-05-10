using System.Collections.Specialized;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CspParameters: NameValueCollection
{
    public CspParameters() { }

    public CspParameters(NameValueCollection originals) : base(originals) { }

    public override string ToString() =>
        UrlHelpers.NvcToString(this, " ", "; ", ";", "", false, " ");
}