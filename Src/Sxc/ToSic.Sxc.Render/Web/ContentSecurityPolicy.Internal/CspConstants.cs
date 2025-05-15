namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

internal class CspConstants
{
    public const string LogPrefix = "Csp";
    public const string AllSrcName = "all-src";
    public const string DefaultSrcName = "default-src";
    public const string SuffixSrc = "-src";

    public const string CspHeaderNamePolicy = "Content-Security-Policy";
    public const string CspHeaderNameReport = "Content-Security-Policy-Report-Only";
    public const string CspUrlParameter = "csp";
    public const string CspUrlTrue = "true";
    public const string CspUrlDev = "dev";

    public const string CspWhitelistAttribute = "csp-whitelist";
}