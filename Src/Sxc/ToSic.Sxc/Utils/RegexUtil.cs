using System;
using System.Text.RegularExpressions;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Utils
{
    public class RegexUtil
    {
        private const string AttributesFormula = "\\s(?<Key>[\\w-]+(?=[^<]*>))=([\"'])(?<Value>.*?[^\\1][\\s\\S]+?)\\1|\\s(?<Key>[\\w-]+(?=.*?))";
        private const string ImagesWithDataCmsidFormula = "<img[^>]*data-cmsid=['\"](?<cmsid>[^'\"]+)['\"][^>]*>";
        private const string WysiwygWidthNumFormula = "wysiwyg-width(?<num>\\d+)of(?<all>\\d+)";
        private const string ScriptSrcFormula = "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))";
        private const string ScriptContentFormula = @"<script[^>]*>(?<Content>(.|\n)*?)</script[^>]*>";
        private const string StyleSrcFormula = "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/?>)";
        private const string StyleRelFormula = "('|\"|\\s)rel=('|\")stylesheet('|\")";
        private const string IdFormula = "('|\"|\\s)id=('|\")(?<Id>.*?)('|\")";
        private const string ClientDependencyRegex =
            "\\s" + PageService.AssetOptimizationsAttributeName + "=('|\")(?<Priority>true|[0-9]+)?(?::)?(?<Position>bottom|head|body)?('|\")(>|\\s)";

        private static readonly Lazy<Regex> AttributesDetectionLazy = new Lazy<Regex>(() => new Regex(AttributesFormula, RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> ImagesDetectionLazy = new Lazy<Regex>(() => new Regex(ImagesWithDataCmsidFormula, RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> WysiwygWidthNumDetectionLazy = new Lazy<Regex>(() => new Regex(WysiwygWidthNumFormula, RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> ScriptSrcDetectionLazy = new Lazy<Regex>(() => new Regex(ScriptSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline));
        private static readonly Lazy<Regex> ScriptContentDetectionLazy = new Lazy<Regex>(() => new Regex(ScriptContentFormula, RegexOptions.IgnoreCase | RegexOptions.Multiline));
        private static readonly Lazy<Regex> StyleDetectionLazy = new Lazy<Regex>(() => new Regex(StyleSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline));
        private static readonly Lazy<Regex> StyleRelDetectLazy = new Lazy<Regex>(() => new Regex(StyleRelFormula, RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> OptimizeDetectionLazy = new Lazy<Regex>(() => new Regex(ClientDependencyRegex, RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> IdDetectionLazy = new Lazy<Regex>(() => new Regex(IdFormula, RegexOptions.IgnoreCase));

        public static Regex AttributesDetection => AttributesDetectionLazy.Value;
        public static Regex ImagesDetection => ImagesDetectionLazy.Value;
        public static Regex WysiwygWidthNumDetection => WysiwygWidthNumDetectionLazy.Value;
        public static Regex ScriptSrcDetection => ScriptSrcDetectionLazy.Value;
        public static Regex ScriptContentDetection => ScriptContentDetectionLazy.Value;
        public static Regex StyleDetection => StyleDetectionLazy.Value;
        public static Regex StyleRelDetect => StyleRelDetectLazy.Value;
        public static Regex OptimizeDetection => OptimizeDetectionLazy.Value;
        public static Regex IdDetection => IdDetectionLazy.Value;
    }
}