using System.Linq;
using Newtonsoft.Json;
using ToSic.Razor.Markup;
using static ToSic.Sxc.Edit.Toolbar.ItemToolbarBase;

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

            // TODO:
            // - force
            // - condition - test
            // - conditionfunc - test

            // Only test conditions if the toolbar would show - otherwise ignore
            if (edit?.Enabled == true)
            {
                if (_params?.Condition == false) return null;
                if (_params?.ConditionFunc != null && _params.ConditionFunc() == false) return null;
            }

            switch (mode)
            {
                case ToolbarHtmlModes.OnTag:
                    // ReSharper disable once AssignNullToNotNullAttribute - the edit.TagToolbar can return a null
                    return edit == null
                        ? new Attribute(ToolbarAttributeName, ErrRenderMessage).ToString()
                        : edit.TagToolbar(target, toolbar: this)?.ToString();
                case ToolbarHtmlModes.Standalone:
                    // ReSharper disable once AssignNullToNotNullAttribute - the edit.Toolbar can return a null
                    return edit == null
                        ? $"<!-- {ErrRenderMessage} -->"
                        : edit.Toolbar(target, toolbar: this)?.ToString();
                case ToolbarHtmlModes.Json:
                    var rules = Rules.Select(r => r.ToString()).ToArray();
                    return JsonConvert.SerializeObject(rules);
                default:
                    return $"error: toolbar ToString mode '{mode}' is not known";
            }

        }
        
    }
}
