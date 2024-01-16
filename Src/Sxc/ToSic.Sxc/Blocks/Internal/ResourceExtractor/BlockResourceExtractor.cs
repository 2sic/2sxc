using System.Text.RegularExpressions;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.HtmlParsing;
using ToSic.Sxc.Web.Internal.PageService;
using static System.StringComparer;

namespace ToSic.Sxc.Blocks.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract partial class BlockResourceExtractor(PageServiceShared pageServiceShared)
    : ServiceBase("Sxc.AstOpt"), IBlockResourceExtractor
{

    #region Settings

    protected virtual ClientAssetsExtractSettings Settings => _settings.Get(() => new(
        extractAll: false
    ));
    private readonly GetOnce<ClientAssetsExtractSettings> _settings = new();

    #endregion

    /// <summary>
    /// List of extracted assets - this must be processed later by the caller
    /// </summary>
    protected List<IClientAsset> Assets { get; private set; }

    public RenderEngineResult Process(string html) => Process(html, Settings);

    /// <summary>
    /// Run the sequence to extract assets
    /// </summary>
    /// <returns></returns>
    public RenderEngineResult Process(string html, ClientAssetsExtractSettings settings)
    {
        // Pre-Flush Assets, so each call gets its own list
        Assets = [];
        var (template, include2SxcJs) = ExtractFromHtml(html, settings);
        return new(template, include2SxcJs, Assets);
    }

    protected abstract (string Template, bool Include2sxcJs) ExtractFromHtml(string html, ClientAssetsExtractSettings settings);




    /// <summary>
    /// Extract dictionary of html attributes
    /// </summary>
    /// <param name="htmlTag"></param>
    /// <returns></returns>
    public static (IDictionary<string, string> Attributes, string CspCode) GetHtmlAttributes(string htmlTag)
    {
        if (string.IsNullOrWhiteSpace(htmlTag)) return (null, null);

        var attributesMatch = RegexUtil.AttributesDetection.Value.Matches(htmlTag);

        if (attributesMatch.Count == 0) return (null, null);

        var attributes = new Dictionary<string, string>(InvariantCultureIgnoreCase);
        foreach (Match attributeMatch in attributesMatch)
        {
            if (!attributeMatch.Success) continue;
            var key = attributeMatch.Groups["Key"]?.Value.ToLowerInvariant();

            // skip special attributes like "src", "id", "data-enableoptimizations"
            if (string.IsNullOrEmpty(key) || ClientAssetConstants.SpecialHtmlAttributes.Contains(key, InvariantCultureIgnoreCase)) continue;

            var value = attributeMatch.Groups["Value"]?.Value;

            attributes[key] = value; // add or update
        }

        var cspCode = attributesMatch.Cast<Match>()
            .FirstOrDefault(m =>
                m.Groups["Key"]?.Value.EqualsInsensitive(CspConstants.CspWhitelistAttribute) == true)
            ?.Groups["Value"]?.Value;
        return (attributes, cspCode);
    }




    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// check special case: the 2sxc.api script. only check the first part of the path
    /// because it could be .min, or have versions etc.
    /// </remarks>
    /// <param name="url"></param>
    /// <returns></returns>
    private static bool Is2SxcApiJs(string url) => url.ToLowerInvariant()
        .ForwardSlash()
        .Contains("/js/2sxc.api");

    /// <summary>
    /// Because of an issue with spaces, prepend tilde to urls that start at root
    /// and contain spaces: https://github.com/2sic/2sxc/issues/1566
    /// </summary>
    /// <returns></returns>
    private string FixUrlWithSpaces(string url)
    {
        if (!url.Contains(" ")) return url;
        if (!url.StartsWith("/") || url.StartsWith("//")) return url;
        return "~" + url;
    }




}