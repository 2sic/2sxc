using System.Text;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public class UrlParts
    {
        public const char QuerySeparator = '?';
        public const char FragmentSeparator = '#';


        public string Url;
        public string Query = string.Empty;
        public string Fragment = string.Empty;
        public string Path = string.Empty;

        public UrlParts(string url)
        {
            Url = url;
            url = url ?? string.Empty; // avoid errors on string operations
            var fragmentStart = url.IndexOf('#');
            var urlWithoutFragment = url;
            if (fragmentStart >= 0)
            {
                Fragment = url.Substring(fragmentStart + 1);
                urlWithoutFragment = url.Substring(0, fragmentStart);
            }

            var queryStart = urlWithoutFragment.IndexOf('?');
            Path = urlWithoutFragment;
            if (queryStart >= 0)
            {
                Query = urlWithoutFragment.Substring(queryStart + 1);
                Path = urlWithoutFragment.Substring(0, queryStart);
            }

        }

        public string BuildUrl()
        {
            var urlStringBuilder = new StringBuilder(!string.IsNullOrEmpty(Path) ? Path : string.Empty);
            AppendSuffix(urlStringBuilder);
            return urlStringBuilder.ToString();
        }

        // would return the entire suffix starting from the `?` _including_ the `?` or `#` - if nothing is there, empty string
        public string Suffix()
        {
            var urlStringBuilder = new StringBuilder();
            AppendSuffix(urlStringBuilder);
            return urlStringBuilder.ToString();
        }

        private void AppendSuffix(StringBuilder urlStringBuilder)
        {
            if (!string.IsNullOrEmpty(Query)) urlStringBuilder.Append($"{QuerySeparator}{Query}");
            if (!string.IsNullOrEmpty(Fragment)) urlStringBuilder.Append($"{FragmentSeparator}{Fragment}");
        }
    }
}
