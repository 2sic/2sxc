using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web.PageFeatures
{
    /// <summary>
    /// A feature describes something that can be enabled on a page. It can be a script, some css, an inline JS or combinations thereof.
    /// This is just the information which is prepared. It will be in a list of features to activate.
    /// </summary>
    [PrivateApi("Internal / not final - neither name, namespace or anything")]
    public class PageFeature : IPageFeature
    {
        public PageFeature(
            string key, 
            string name, 
            string description = null,
            string[] requires = null,
            string html = null)
        {
            Key = key ?? throw new Exception("key is required");
            Name = name ?? throw new Exception("name is required");
            Html = html;
            Description = description ?? "";
            Requires = requires ?? Array.Empty<string>();
        }
        
        /// <summary>
        /// Primary identifier to activate the feature
        /// </summary>
        public string Key { get; }
        
        /// <summary>
        /// Name of this feature. 
        /// </summary>
        public string Name { get; }

        public string Html { get; }

        /// <summary>
        /// Nice description of the feature.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// List of other features required to run this feature.
        /// </summary>
        public IEnumerable<string> Requires { get; }

        public bool AlreadyProcessed { get; set; }
    }
}
