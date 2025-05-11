using static System.StringComparison;

namespace ToSic.Sxc.LookUp;

partial class LookUpForTokenTemplate
{
    public const string TokenRepeater = "Repeater";

    public const string KeyIndex = "index";
    public const string KeyIndex1 = "index1";
    public const string KeyAlternator2 = "alternator2";
    public const string KeyAlternator3 = "alternator3";
    public const string KeyAlternator4 = "alternator4";
    public const string KeyAlternator5 = "alternator5";
    public const string KeyIsFirst = "isfirst";
    public const string KeyIsLast = "islast";
    public const string KeyCount = "count";

    private string ResolveRepeaterTokens(string strPropertyName)
    {
        if (repeaterIndex <= -1 || !strPropertyName.StartsWith(TokenRepeater + ":", OrdinalIgnoreCase))
            return null;

        return strPropertyName.Substring(TokenRepeater.Length + 1).ToLowerInvariant() switch
        {
            KeyIndex => (repeaterIndex).ToString(),
            KeyIndex1 => (repeaterIndex + 1).ToString(),
            KeyAlternator2 => (repeaterIndex % 2).ToString(),
            KeyAlternator3 => (repeaterIndex % 3).ToString(),
            KeyAlternator4 => (repeaterIndex % 4).ToString(),
            KeyAlternator5 => (repeaterIndex % 5).ToString(),
            KeyIsFirst => repeaterIndex == 0 ? "First" : "",
            KeyIsLast => repeaterIndex == repeaterTotal - 1 ? "Last" : "",
            KeyCount => repeaterTotal.ToString(),
            _ => null
        };
    }

}