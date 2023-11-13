using System;
using System.Collections.Generic;
using ToSic.Eav.Configuration;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Web.PageFeatures
{
    /// <summary>
    /// A feature describes something that can be enabled on a page. It can be a script, some css, an inline JS or combinations thereof.
    /// This is just the information which is prepared. It will be in a list of features to activate.
    /// </summary>
    [PrivateApi("Internal / not final - neither name, namespace or anything")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class PageFeature : IPageFeature
    {
        public const string ConditionIsPageFeature = "pagefeature";

        #region Constructor

        public PageFeature(
            string key, 
            string name, 
            string description = default,
            string[] needs = default,
            string html = default,
            List<Condition> requirements = default,
            string urlWip = default)
        {
            NameId = key ?? throw new Exception("key is required");
            Name = name ?? throw new Exception("name is required");
            Html = html;
            Description = description ?? "";
            Needs = needs ?? Array.Empty<string>();
            Condition = new Condition(ConditionIsPageFeature, key);
            Requirements = requirements ?? new List<Condition>();
            UrlWip = urlWip;
        }

        #endregion
        
        /// <summary>
        /// Primary identifier to activate the feature
        /// </summary>
        public string NameId { get; }
        
        /// <summary>
        /// Name of this feature. 
        /// </summary>
        public string Name { get; }

        public string Html { get; set; }

        /// <summary>
        /// Nice description of the feature.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// List of other features required to run this feature.
        /// </summary>
        public IEnumerable<string> Needs { get; }

        public Condition Condition { get; }

        public List<Condition> Requirements { get; }

        /// <summary>
        /// Temporary URL for internal features which need to store the URL someplace
        /// This is not a final solution, in future it should probably
        /// be more sophisticated, like contain a list of configuration objects to construct the url.
        /// </summary>
        public string UrlWip { get; }

        /// <summary>
        /// ToString for easier debugging
        /// </summary>
        public override string ToString() => base.ToString() + "(" + NameId + ")";
    }
}
