using System;
using System.Collections.Generic;
using ToSic.Eav.Configuration;
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
        public const string ConditionIsPageFeature = "pagefeature";

        #region Constructor

        public PageFeature(
            string key, 
            string name, 
            string description = null,
            string[] needs = null,
            string html = null,
            List<Condition> reqConditions = null)
        {
            Key = key ?? throw new Exception("key is required");
            Name = name ?? throw new Exception("name is required");
            Html = html;
            Description = description ?? "";
            Needs = needs ?? Array.Empty<string>();
            Condition = new Condition(ConditionIsPageFeature, key);
            RequirementsOrNull = reqConditions ?? new List<Condition>();
        }

        #endregion
        
        /// <summary>
        /// Primary identifier to activate the feature
        /// </summary>
        public string Key { get; }
        
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

        public List<Condition> RequirementsOrNull { get; }
    }
}
