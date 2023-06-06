using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ToSic.Eav.Data;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Data;
using static ToSic.Sxc.Edit.Toolbar.ItemToolbarBase;
using Attribute = ToSic.Razor.Markup.Attribute;

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
            var edit = _DynCodeRoot?.Edit;

            // TODO:
            // - force

            // Only test conditions if the toolbar would show - otherwise ignore
            if (edit?.Enabled == true && _configuration != null)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                if (_configuration.Condition == false) return null;
                if (_configuration.ConditionFunc?.Invoke() == false) return null;
                // ReSharper restore AssignNullToNotNullAttribute
            }

            // No auto-demo
            if (!ShouldUseDemoMode()) return Render();

            //var useDemoMode = ShouldUseDemoMode();
            //if (!useDemoMode) return Render();

            // Implement Demo-Mode with info-button only
            var rules = new List<ToolbarRuleBase>();
            void AddRuleIfFound<T>() where T : ToolbarRuleBase
            {
                var possibleParams = FindRule<T>();
                if (possibleParams != null) rules.Add(possibleParams);
            }

            AddRuleIfFound<ToolbarRuleToolbar>();
            AddRuleIfFound<ToolbarRuleForParams>();
            AddRuleIfFound<ToolbarRuleSettings>();
                
            var tlb = new ToolbarBuilder(this, rules) as IToolbarBuilder;
            tlb = tlb.Info(tweak: b => b.Note(_DynCodeRoot.Resources.Get<string>("Toolbar.IsDemoSubItem")));
            return ((ToolbarBuilder)tlb).Render();

        }

        private bool ShouldUseDemoMode()
        {
            // If no root provided, we can't check demo mode as of now, so return
            var root = _configuration?.Root?.Entity;
            if (root == null) return false;

            // If root is not demo, then don't use demo mode
            if (!root.IsDemoItem()) return false;

            // Check if we have a target, if not, then go into demo-mode
            var target = (FindRule<ToolbarRuleForParams>()?.Target as ICanBeEntity)?.Entity;
            if (target == null) return true;

            // If the root and target are the same, then the toolbar should work
            // because it's meant to create a new entry right here
            if (root.EntityId == target.EntityId) return false;

            return true;
        }


        private string Render()
        {
            var edit = _DynCodeRoot?.Edit;
            var mode = _configuration?.Mode;
            mode = (mode ?? ToolbarHtmlModes.OnTag).ToLowerInvariant();
            switch (mode)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                case ToolbarHtmlModes.OnTag:
                    return edit == null
                        ? new Attribute(ToolbarAttributeName, ErrRenderMessage).ToString() // add error
                        : edit.TagToolbar(this)?.ToString();
                case ToolbarHtmlModes.Standalone:
                    return edit == null
                        ? $"<!-- {ErrRenderMessage} -->"              // add error
                        : edit.Toolbar(this)?.ToString();       // Show toolbar
                // ReSharper restore AssignNullToNotNullAttribute
                case ToolbarHtmlModes.Json:
                    var rules = Rules.Select(r => r.ToString()).ToArray();
                    return JsonSerializer.Serialize(rules, JsonOptions.SafeJsonForHtmlAttributes);
                default:
                    return $"error: toolbar ToString mode '{mode}' is not known";
            }

        }
        
    }
}
