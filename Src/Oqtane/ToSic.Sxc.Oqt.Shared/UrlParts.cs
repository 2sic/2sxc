using System;
using System.Linq;
using System.Text;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Oqt.Shared;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class UrlParts
{
    public const char QuerySeparator = '?';
    public const char FragmentSeparator = '#';
    public const char ValuePairSeparator = '&';
    public const string ProtocolColon = ":";
    public const string Slash = "/";
    public const string ProtocolSeparator = "://";
    public const string RootWithoutProtocol = "//";


    public string Url;
    public string Query = string.Empty;
    public string Fragment = string.Empty;
    public string Path = string.Empty;
    public string Protocol = string.Empty;
    public string Domain = string.Empty;

    public UrlParts(string url)
    {
        url = (url ?? "").Trim();
        Url = url;
        if (string.IsNullOrEmpty(url)) return;

        var rest = url;
        rest = ExtractFragment(url, rest);
        Path = rest;
        rest = ExtractQuery(rest);
        Path = rest;
        rest = ExtractProtocol(rest);
        Path = rest;

        // now check domain - only valid if the link had a "//" in it to start with or after the protocol
        var domainAtStartPossible = !string.IsNullOrEmpty(Protocol);
        rest = ExtractDomain(domainAtStartPossible, rest);
        Path = rest;

    }

    public void ReplaceRoot(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        if (!url.Contains(Slash) && !url.Contains(ProtocolColon))
            Domain = url;
        else
        {
            var newParts = new UrlParts(url);
            if (!string.IsNullOrEmpty(newParts.Domain)) Domain = newParts.Domain;
            if (!string.IsNullOrEmpty(newParts.Protocol)) Protocol = newParts.Protocol;
        }
        if (!string.IsNullOrEmpty(Domain) && string.IsNullOrEmpty(Protocol))
            Protocol = RootWithoutProtocol;
    }

    private string ExtractDomain(bool domainAtStartPossible, string rest)
    {
        if (domainAtStartPossible && !string.IsNullOrEmpty(rest))
        {
            var postDomainSlash = rest.IndexOf('/');
            if (postDomainSlash == -1) postDomainSlash = rest.Length;
            Domain = rest.Substring(0, postDomainSlash);
            rest = rest.Substring(postDomainSlash);
        }

        return rest;
    }

    private string ExtractProtocol(string rest)
    {
        if (rest.StartsWith(RootWithoutProtocol))
        {
            Protocol = RootWithoutProtocol;
            rest = rest.Substring(RootWithoutProtocol.Length);
            return rest;
        }

        var separator = rest.IndexOf(ProtocolSeparator, StringComparison.Ordinal);
        // protocol would have to be at least 1 char
        if (separator > 1)
        {
            Protocol = rest.Substring(0, separator + ProtocolSeparator.Length);
            rest = rest.Substring(Protocol.Length); // drop protocol + ':'
        }

        return rest;
    }

    private string ExtractQuery(string rest)
    {
        var queryStart = rest.IndexOf(QuerySeparator);
        if (queryStart < 0) return rest;
        Query = rest.Substring(queryStart + 1);
        return rest.Substring(0, queryStart);
    }

    private string ExtractFragment(string url, string rest)
    {
        var fragmentStart = rest.IndexOf(FragmentSeparator);
        if (fragmentStart < 0) return rest;
        Fragment = url.Substring(fragmentStart + 1);
        return url.Substring(0, fragmentStart);
    }

    public bool IsAbsolute => !string.IsNullOrEmpty(Protocol); // Path.StartsWith("//") || Path.StartsWith("http://") || Path.StartsWith("https://");
    public bool IsRelative => Path.StartsWith(".") && !IsAbsolute && !string.IsNullOrEmpty(Domain);


    public string ToLink(string format = null, bool suffix = true)
    {
        var endPart = Path + (suffix ? Suffix() : "");
        if (format == "/") return endPart;
        if (format == "//")
            return string.IsNullOrEmpty(Domain)
                ? endPart
                : RootWithoutProtocol + Domain + endPart;

        return Protocol + Domain + endPart;
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

    public static string ConnectParameters(params string[] parameters)
    {
        if (parameters == null || parameters.Length == 0) return "";
        var realParams = parameters.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
        return string.Join(ValuePairSeparator.ToString(), realParams);
    }
}