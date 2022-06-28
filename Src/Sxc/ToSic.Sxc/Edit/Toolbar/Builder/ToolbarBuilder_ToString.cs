using System.Linq;
using Newtonsoft.Json;
using ToSic.Razor.Markup;
using static ToSic.Sxc.Edit.Toolbar.ItemToolbarBase;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private const string ErrRenderMessage = "error: can't render toolbar to html, missing context";

        public IToolbarBuilder AsTag() => WithInternal(mode: ToolbarHtmlModes.Standalone);

        public IToolbarBuilder AsAttributes() => WithInternal(mode: ToolbarHtmlModes.OnTag);

        public IToolbarBuilder AsJson() => WithInternal(mode: ToolbarHtmlModes.Json);

        public override string ToString()
        {
            var mode = _params?.Mode;
            mode = (mode ?? ToolbarHtmlModes.OnTag).ToLowerInvariant();

            var target = _params?.Target;

            var edit = _codeRoot?.Edit;

            // TODO:
            // - force

            // Only test conditions if the toolbar would show - otherwise ignore
            if (edit?.Enabled == true)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                if (_params?.Condition == false) return null;
                if (_params?.ConditionFunc != null && _params.ConditionFunc() == false) return null;
                // ReSharper restore AssignNullToNotNullAttribute
            }

            switch (mode)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                case ToolbarHtmlModes.OnTag:
                    return edit == null
                        ? new Attribute(ToolbarAttributeName, ErrRenderMessage).ToString()
                        : edit.TagToolbar(this)?.ToString();
                case ToolbarHtmlModes.Standalone:
                    return edit == null
                        ? $"<!-- {ErrRenderMessage} -->"
                        : edit.Toolbar(this)?.ToString();
                // ReSharper restore AssignNullToNotNullAttribute
                case ToolbarHtmlModes.Json:
                    var rules = Rules.Select(r => r.ToString()).ToArray();
                    return JsonConvert.SerializeObject(rules);
                default:
                    return $"error: toolbar ToString mode '{mode}' is not known";
            }

        }
        
    }
}
