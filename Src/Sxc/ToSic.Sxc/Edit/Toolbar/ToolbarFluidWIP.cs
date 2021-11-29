using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarFluidWIP: HybridHtmlString
    {
        public ToolbarFluidWIP() : base(string.Empty)
        {
        }

        public ToolbarFluidWIP(ToolbarFluidWIP original): this()
        {
            Rules = original.Rules.Select(r => r).ToList();
        }

        public List<ToolbarRuleBase> Rules { get; private set; } = new List<ToolbarRuleBase>();

        public override string ToString()
        {
            var rules = Rules.Select(r => r.Rule).ToArray();
            var rulesJson = JsonConvert.SerializeObject(rules);
            return '{' + rulesJson.TrimStart('[').TrimEnd(']') + "}";
        }
    }
}
