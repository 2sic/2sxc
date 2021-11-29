using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarFluidWIP: HybridHtmlString, IEnumerable<string>
    {
        #region Constructors
        public ToolbarFluidWIP() : base(string.Empty)
        {
        }

        public ToolbarFluidWIP(ToolbarFluidWIP original): this()
        {
            Rules = original.Rules.Select(r => r).ToList();
        }
        #endregion

        public ToolbarFluidWIP Add(params string[] rules)
        {
            var clone = new ToolbarFluidWIP(this);
            if (!rules.Any()) return clone;
            foreach (var rule in rules)
                clone.Rules.Add(new ToolbarRuleGeneric(rule));
            return clone;
        }

        public ToolbarFluidWIP Add(params ToolbarRuleBase[] rules)
        {
            var clone = new ToolbarFluidWIP(this);
            if (!rules.Any()) return clone;
            foreach (var rule in rules)
                clone.Rules.Add(rule);
            return clone;
        }


        public List<ToolbarRuleBase> Rules { get; } = new List<ToolbarRuleBase>();

        public IEnumerator<string> GetEnumerator() => Rules.Select(r => r.Rule).GetEnumerator();

        public override string ToString()
        {
            var rules = Rules.Select(r => r.Rule).ToArray();
            return JsonConvert.SerializeObject(rules);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();




        public ToolbarFluidWIP AddMeta(object target, string contentType) => Add(new ToolbarRuleMetadata(target, contentType));
    }
}
