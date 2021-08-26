using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Razor.Html5;

namespace ToSic.Sxc.Web.PageFeatures
{
    [PrivateApi]
    public interface IPageFeature
    {
        /// <summary>
        /// Primary identifier to activate the feature
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Name of this feature. 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Nice description of the feature.
        /// </summary>
        string Description { get; }

        string Html { get; }

        //IList<Script> ScriptFiles { get; }
        //IList<Script> Scripts { get; }
        //IList<Link> StyleFiles { get; }
        //IList<Style> Styles { get; }

        /// <summary>
        /// List of other features required to run this feature.
        /// </summary>
        IEnumerable<string> Requires { get; }

        bool AlreadyProcessed { get; set; }
    }
}