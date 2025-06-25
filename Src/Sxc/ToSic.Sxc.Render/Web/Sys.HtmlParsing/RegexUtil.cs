using System.Text.RegularExpressions;
using ToSic.Sxc.Web.Internal.ClientAssets;

namespace ToSic.Sxc.Web.Internal.HtmlParsing;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class RegexUtil
{
    /// <summary>
    /// Used in ScriptSrc and StyleSrc Formulas
    /// </summary>
    public const string SrcKey = "Src";

    public const string IntegrityKey = "Integrity";

    // language=regex
    private const string AttributesFormula = "\\s(?<Key>[\\w-]+(?=[^<]*>))=([\"'])(?<Value>.*?[^\\1][\\s\\S]+?)\\1|\\s(?<Key>[\\w-]+(?=.*?))";
    // language=regex
    private const string ImagesWithDataCmsidFormula = "<img[^>]*data-cmsid=['\"](?<cmsid>[^'\"]+)['\"][^>]*>";
    // language=regex
    private const string ScriptSrcFormula = "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))";
    // language=regex
    private const string IntegrityAttributeFormula = "(?<Integrity>integrity=('|\").*?('|\"))";

    // language=regex
    private const string ScriptContentFormula = @"<script[^>]*>(?<Content>(.|\n)*?)</script[^>]*>";
    // language=regex
    private const string StyleSrcFormula = "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/?>)";
    // language=regex
    private const string StyleRelFormula = "('|\"|\\s)rel=('|\")stylesheet('|\")";
    // language=regex
    private const string IdFormula = "('|\"|\\s)id=('|\")(?<Id>.*?)('|\")";
    // language=regex
    private const string ClientDependencyRegex =
        "\\s" + ClientAssetConstants.AssetOptimizationsAttributeName + "=('|\")(?<Priority>true|[0-9]+)?(?::)?(?<Position>bottom|head|body)?('|\")(>|\\s)";

    public const string PriorityKey = "Priority";
    public const string PositionKey = "Position";

    public static readonly Lazy<Regex> AttributesDetection = new(() => new(AttributesFormula, RegexOptions.IgnoreCase));
    public static readonly Lazy<Regex> ImagesDetection = new(() => new(ImagesWithDataCmsidFormula, RegexOptions.IgnoreCase));
    public static readonly Lazy<Regex> ScriptSrcDetection = new(() => new(ScriptSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline));
    // note: 2dm created this, because I wasn't sure if changing the original to ML would have side effects
    public static Regex ScriptSrcDetectionMultiLine = new(ScriptSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Multiline);

    public static readonly Lazy<Regex> ScriptContentDetection = new(() => new(ScriptContentFormula, RegexOptions.IgnoreCase | RegexOptions.Multiline));
    public static readonly Lazy<Regex> StyleDetection = new(() => new(StyleSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline));
    // note: 2dm created this, because I wasn't sure if changing the original to ML would have side effects
    public static Regex StyleDetectionMultiLine = new(StyleSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Multiline);
    public static Regex IntegrityAttribute = new(IntegrityAttributeFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
    public static readonly Lazy<Regex> StyleRelDetect = new(() => new(StyleRelFormula, RegexOptions.IgnoreCase));
    public static readonly Lazy<Regex> OptimizeDetection = new(() => new(ClientDependencyRegex, RegexOptions.IgnoreCase));
    public static readonly Lazy<Regex> IdDetection = new(() => new(IdFormula, RegexOptions.IgnoreCase));

    //// language=regex
    //private const string WysiwygWidthNumFormula = "wysiwyg-width(?<num>\\d+)of(?<all>\\d+)";
    //private static readonly Lazy<Regex> WysiwygWidthNumDetectionLazy = new Lazy<Regex>(() => new Regex(WysiwygWidthNumFormula, RegexOptions.IgnoreCase));
    //public static Regex WysiwygWidthNumDetection => WysiwygWidthNumDetectionLazy.Value;

    // language=regex
    private const string WysiwygWidthFormula = "wysiwyg-(?<percent>\\d+)";
    public static readonly Lazy<Regex> WysiwygWidthLazy = new(() => new(WysiwygWidthFormula, RegexOptions.IgnoreCase));

}