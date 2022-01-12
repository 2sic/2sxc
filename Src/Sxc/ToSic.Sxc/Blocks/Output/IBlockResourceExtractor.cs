using System;
using System.Collections.Generic;
using ToSic.Eav.Logging;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Blocks.Output
{
    /// <summary>
    /// System to automatically pick up JS/CSS files which should be bundled
    /// </summary>
    public interface IBlockResourceExtractor: IHasLog<IBlockResourceExtractor>
    {
        /// <summary>
        /// Scan the html for possible JS/CSS files which should be bundled and extract these. 
        /// </summary>
        /// <param name="renderedTemplate">html to be rendered</param>
        /// <returns>
        /// Original html without the js/css tags which were bundled (so they get removed here)
        /// Second return-param is an information if the core $2sxc.js should be included
        /// </returns>
        Tuple<string, bool> Process(string renderedTemplate);

        /// <summary>
        /// The assets which were extracted from the process
        /// </summary>
        List<ClientAssetInfo> Assets { get; }
    }
}
