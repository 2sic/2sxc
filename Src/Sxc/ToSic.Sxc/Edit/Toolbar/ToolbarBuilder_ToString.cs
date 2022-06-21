using System.Linq;
using Newtonsoft.Json;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private const string ErrRenderMessage = "error: can't render toolbar to html, missing context";

        public override string ToString()
        {
            var mode = _params?.Mode;
            mode = (mode ?? ToolbarHtmlModes.OnTag).ToLowerInvariant();

            var target = _params?.Target;

            var edit = _codeRoot?.Edit;

            switch (mode)
            {
                case ToolbarHtmlModes.OnTag:
                    return edit == null
                        ? new Attribute(ItemToolbar.ToolbarAttributeName, ErrRenderMessage).ToString()
                        : edit.TagToolbar(target, toolbar: this).ToString();
                case ToolbarHtmlModes.Standalone:
                    return edit == null
                        ? $"<!-- {ErrRenderMessage} -->"
                        : edit.Toolbar(target, toolbar: this).ToString();
                case ToolbarHtmlModes.Json:
                    var rules = Rules.Select(r => r.ToString()).ToArray();
                    return JsonConvert.SerializeObject(rules);
                default:
                    return $"error: toolbar ToString mode '{mode}' is not known";
            }

        }
        
    }
}
