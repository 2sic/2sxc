using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.Helpers;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Utils;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Blocks.Output
{
    public abstract partial class BlockResourceExtractor: ServiceBase, IBlockResourceExtractor
    {
        #region Construction / DI

        protected BlockResourceExtractor(PageServiceShared pageServiceShared) : base("Sxc.AstOpt") 
            => _pageServiceShared = pageServiceShared;
        private readonly PageServiceShared _pageServiceShared;

        #endregion

        #region Settings

        /// <summary>
        /// Priority of a CSS - will be used for sorting when added to page
        /// </summary>
        protected int CssDefaultPriority = 99;

        /// <summary>
        /// Priority of a CSS - will be used for sorting when added to page
        /// </summary>
        protected int JsDefaultPriority = 100;

        /// <summary>
        /// Extract only script tags which are marked for extraction
        /// </summary>
        public bool ExtractOnlyEnableOptimization = true;

        /// <summary>
        /// List of special attributes like "src", "id", "data-enableoptimizations"
        /// that we need to skip from adding in general HtmlAttributes dictionary
        /// because this special attributes are handled in custom way.
        /// </summary>
        private static readonly List<string> SkipHtmlAttributes = new List<string> { "src", "id", PageService.AssetOptimizationsAttributeName, CspConstants.CspWhitelistAttribute };

        #endregion

        /// <summary>
        /// List of extracted assets - this must be processed later by the caller
        /// </summary>
        protected List<IClientAsset> Assets { get; private set; }

        /// <summary>
        /// Run the sequence to extract assets
        /// </summary>
        /// <param name="renderedTemplate"></param>
        /// <returns></returns>
        public RenderEngineResult Process(string renderedTemplate)
        {
            // Pre-Flush Assets, so each call gets its own list
            Assets = new List<IClientAsset>();
            var (template, include2SxcJs) = ExtractFromHtml(renderedTemplate);
            return new RenderEngineResult(template, include2SxcJs, Assets);
        }

        protected abstract (string Template, bool Include2sxcJs) ExtractFromHtml(string renderedTemplate);




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

            var attributes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (Match attributeMatch in attributesMatch)
            {
                if (!attributeMatch.Success) continue;
                var key = attributeMatch.Groups["Key"]?.Value.ToLowerInvariant();

                // skip special attributes like "src", "id", "data-enableoptimizations"
                if (string.IsNullOrEmpty(key) || SkipHtmlAttributes.Contains(key, StringComparer.InvariantCultureIgnoreCase)) continue;

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
}