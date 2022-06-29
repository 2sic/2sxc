using System.Linq;
using Newtonsoft.Json;
using ToSic.Razor.Markup;
using static ToSic.Sxc.Edit.Toolbar.ItemToolbarBase;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private const string ErrRenderMessage = "error: can't render toolbar to html, missing context";

        public IToolbarBuilder AsTag(object target = null) => With(mode: ToolbarHtmlModes.Standalone, target: target);

        public IToolbarBuilder AsAttributes(object target = null) => With(mode: ToolbarHtmlModes.OnTag, target: target);

        public IToolbarBuilder AsJson(object target = null) => With(mode: ToolbarHtmlModes.Json, target: target);

        public override string ToString()
        {
            var mode = _configuration?.Mode;
            mode = (mode ?? ToolbarHtmlModes.OnTag).ToLowerInvariant();

            var edit = _codeRoot?.Edit;

            // TODO:
            // - force

            // Only test conditions if the toolbar would show - otherwise ignore
            if (edit?.Enabled == true)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                if (_configuration?.Condition == false) return null;
                if (_configuration?.ConditionFunc != null && _configuration.ConditionFunc() == false) return null;
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
