using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    /// <inheritdoc />
    public class ToolbarBuilder: HybridHtmlString, IEnumerable<string>, IToolbarBuilder
    {
        #region Constructors
        internal ToolbarBuilder() { }

        /// <summary>
        /// Create a ToolbarBuilder which clones a previous configuration
        /// </summary>
        /// <param name="original"></param>
        internal ToolbarBuilder(ToolbarBuilder original): this()
        {
            Rules = original.Rules.Select(r => r).ToList();
        }
        #endregion
        public List<ToolbarRuleBase> Rules { get; } = new List<ToolbarRuleBase>();

        /// <inheritdoc />
        [PrivateApi]
        public IToolbarBuilder Add(params string[] rules) => InnerAdd(rules);

        /// <inheritdoc />
        public IToolbarBuilder Add(params object[] rules) => InnerAdd(rules);

        private IToolbarBuilder InnerAdd(params object[] rules)
        {
            var clone = new ToolbarBuilder(this);
            if (!rules.Any()) return clone;
            foreach (var rule in rules)
            {
                if (rule is ToolbarRuleBase realRule)
                    clone.Rules.Add(realRule);
                else if (rule is string stringRule)
                    clone.Rules.Add(new ToolbarRuleGeneric(stringRule));
            }
            return clone;
        }

        /// <inheritdoc />
        public IToolbarBuilder Metadata(object target, string contentTypes) => Add(new ToolbarRuleMetadata(target, contentTypes));

        public IToolbarBuilder Settings(string noParamOrder = Eav.Parameters.Protector, string show = null,
            string hover = null, string follow = null, string classes = null, string autoAddMore = null, string ui = "", string parameters = "")
            => Add(new ToolbarRuleSettings(show: show, hover: hover, follow: follow, classes: classes, autoAddMore: autoAddMore,
                ui: ui, parameters: parameters));

        public override string ToString()
        {
            var rules = Rules.Select(r => r.ToString()).ToArray();
            return JsonConvert.SerializeObject(rules);
        }

        #region Enumerators

        [PrivateApi]
        public IEnumerator<string> GetEnumerator() => Rules.Select(r => r.ToString()).GetEnumerator();
        [PrivateApi]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

    }
}
