using ToSic.Lib.Logging;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web.ClientAssets;

namespace ToSic.Sxc.Blocks.Output
{
    /// <summary>
    /// System to automatically pick up JS/CSS files which should be bundled
    /// </summary>
    public interface IBlockResourceExtractor: IHasLog
    {
        RenderEngineResult Process(string html);

        /// <summary>
        /// Scan the html for possible JS/CSS files which should be bundled and extract these. 
        /// </summary>
        /// <param name="html">html to extract from</param>
        /// <returns>
        /// Original html without the js/css tags which were bundled (so they get removed here)
        /// Second return-param is an information if the core $2sxc.js should be included
        /// </returns>
        RenderEngineResult Process(string html, ClientAssetsExtractSettings settings);

    }
}
