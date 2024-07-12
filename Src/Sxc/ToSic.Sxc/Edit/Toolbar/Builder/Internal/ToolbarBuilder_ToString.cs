using System.Text.Json;
using ToSic.Eav.Apps;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Internal.PageFeatures;
using static ToSic.Sxc.Edit.Toolbar.ItemToolbarBase;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleToolbar;
using Attribute = ToSic.Razor.Markup.Attribute;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial class ToolbarBuilder
{
    private const string ErrRenderMessage = "error: can't render toolbar to html, missing context";

    public IToolbarBuilder AsTag(object target = null) => With(mode: ToolbarHtmlModes.Standalone, target: target);

    public IToolbarBuilder AsAttributes(object target = null) => With(mode: ToolbarHtmlModes.OnTag, target: target);

    public IToolbarBuilder AsJson(object target = null) => With(mode: ToolbarHtmlModes.Json, target: target);

    public override string ToString()
    {
        // Get edit, but don't exit if null, as the Render (later on) will add comments if Edit is null
        var edit = _CodeApiSvc?.Edit;

        // TODO:
        // - force
        var forceEnable = Configuration?.ForEveryone == true;
        var enabled = edit?.Enabled == true || forceEnable;

        if (forceEnable) 
            _CodeApiSvc?.GetService<IPageService>(reuse: true)
                .Activate(SxcPageFeatures.ToolbarsInternal.NameId);

        // Check if conditions don't allow. Only test conditions if the toolbar would show - otherwise ignore
        if (enabled && Configuration != null)
        {
            if (Configuration.Condition == false) return "";
            if (Configuration.ConditionFunc?.Invoke() == false) return "";
        }

        // Show toolbar or a Demo-Informative-Toolbar
        var finalToolbar = ShouldSwitchToItemDemoMode()
            ? CreateItemDemoToolbar()       // Implement Demo-Mode with info-button only
            : this;                         // Normal render of the current toolbar
        return finalToolbar.Render(edit, enabled);
    }

    /// <summary>
    /// Create a fresh Toolbar which only shows infos about item being in demo-mode
    /// </summary>
    /// <returns></returns>
    private ToolbarBuilder CreateItemDemoToolbar()
    {
        var rules = new List<ToolbarRuleBase>();

        bool AddRuleIfFound<T>(Func<T, T> tweak = null) where T : ToolbarRuleBase
        {
            var possibleParams = FindRule<T>();
            if (possibleParams == null) return false;
            var tweaked = tweak?.Invoke(possibleParams);
            rules.Add(tweaked ?? possibleParams);
            return true;
        }

        if (!AddRuleIfFound<ToolbarRuleToolbar>(t => new(Empty, t.Ui)))
            rules.Add(new ToolbarRuleToolbar(Empty));
        AddRuleIfFound<ToolbarRuleForParams>();
        AddRuleIfFound<ToolbarRuleSettings>();

        var tlb = new ToolbarBuilder(this, rules) as IToolbarBuilder;
        var keyOrMessage = Configuration?.DemoMessage;
        var message = keyOrMessage == null
            ? _CodeApiSvc.Resources.Get<string>($"{AppStackConstants.RootNameResources}.Toolbar.IsDemoSubItem")
            : keyOrMessage.StartsWith($"{AppStackConstants.RootNameResources}.")
                ? _CodeApiSvc.Resources.Get<string>(keyOrMessage)
                : keyOrMessage;
        tlb = tlb.Info(tweak: b => b.Note(message));
        return (ToolbarBuilder)tlb;
    }

    private bool ShouldSwitchToItemDemoMode()
    {
        // If no root provided, we can't check demo mode as of now, so return
        var root = Configuration?.Root?.Entity;
        if (root == null) return false;

        // If root is not demo, then don't use demo mode
        if (!root.IsDemoItemSafe()) return false;

        // Check if we have a target, if not, then go into demo-mode
        var target = (FindRule<ToolbarRuleForParams>()?.Target as ICanBeEntity)?.Entity;
        if (target == null) return true;

        // If the root and target are the same, then the toolbar should work
        // because it's meant to create a new entry right here
        if (root.EntityId == target.EntityId) return false;

        return true;
    }


    private string Render(IEditService edit, bool enabled)
    {
        // don't show toolbar if not enabled (or not for everyone)
        if (!enabled)
            return ""; 

        var mode = (Configuration?.Mode ?? ToolbarHtmlModes.OnTag).ToLowerInvariant();
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
                var rules = Rules
                    .Select(r => r.ToString())
                    .Where(r => r != "")
                    .ToArray();
                return JsonSerializer.Serialize(rules, JsonOptions.SafeJsonForHtmlAttributes);
            default:
                return $"error: toolbar ToString mode '{mode}' is not known";
        }

    }
        
}