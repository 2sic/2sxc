using System.Collections.Specialized;

namespace ToSic.Sxc.Oqt.Shared.Models
{
    public class CspParameters : NameValueCollection
    {
        public CspParameters() { }

        public CspParameters(NameValueCollection originals) : base(originals)
        {
        }

        public override string ToString() => UrlHelpers.NvcToString(this, " ", "; ", ";", "", false, " ");
    }
}