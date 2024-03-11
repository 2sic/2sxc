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

        switch (strPropertyName.Substring(TokenRepeater.Length + 1).ToLowerInvariant())
        {
            case KeyIndex: return (repeaterIndex).ToString();
            case KeyIndex1: return (repeaterIndex + 1).ToString();
            case KeyAlternator2: return (repeaterIndex % 2).ToString();
            case KeyAlternator3: return (repeaterIndex % 3).ToString();
            case KeyAlternator4: return (repeaterIndex % 4).ToString();
            case KeyAlternator5: return (repeaterIndex % 5).ToString();
            case KeyIsFirst: return repeaterIndex == 0 ? "First" : "";
            case KeyIsLast: return repeaterIndex == repeaterTotal - 1 ? "Last" : "";
            case KeyCount: return repeaterTotal.ToString();
            default: return null;
        }
    }

}