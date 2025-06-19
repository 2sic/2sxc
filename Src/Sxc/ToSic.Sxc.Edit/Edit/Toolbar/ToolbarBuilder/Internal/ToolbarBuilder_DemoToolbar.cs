using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Sys.ExecutionContext;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleToolbar;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial record ToolbarBuilder
{
    /// <summary>
    /// Detect if switch to demo is needed.
    /// </summary>
    /// <returns></returns>
    private bool ShouldSwitchToItemDemoMode()
    {
        // If no root provided, we can't check demo mode as of now, so return
        var root = Configuration?.DemoCheckItem?.Entity;
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

    /// <summary>
    /// Create a fresh Toolbar which only shows infos about item being in demo-mode
    /// </summary>
    /// <returns></returns>
    private ToolbarBuilder CreateStandaloneItemDemoToolbar()
    {
        var rules = new List<ToolbarRuleBase>();

        if (!AddRuleOfOriginal<ToolbarRuleToolbar>(t => new(Empty, t.Ui)))
            rules.Add(new ToolbarRuleToolbar(Empty));
        AddRuleOfOriginal<ToolbarRuleForParams>();
        AddRuleOfOriginal<ToolbarRuleSettings>();

        var tlb = this with { Rules = rules };
        var keyOrMessage = Configuration?.DemoMessage;
        var allResources = ExCtx.GetState<ITypedStack>(ExecutionContextStateNames.AllResources);
        var message = keyOrMessage == null
            ? allResources.Get<string>($"{AppStackConstants.RootNameResources}.Toolbar.IsDemoSubItem")
            : keyOrMessage.StartsWith($"{AppStackConstants.RootNameResources}.")
                ? allResources.Get<string>(keyOrMessage)
                : keyOrMessage;
        tlb = (ToolbarBuilder)tlb.Info(tweak: b => b.Note(message));
        return tlb;

        // Helper to check if a rule exists on the original
        // and add it to the new toolbar if it does
        // optionally tweak it if necessary
        // return true/false to indicate if it was added
        bool AddRuleOfOriginal<T>(Func<T, T> tweak = null) where T : ToolbarRuleBase
        {
            var possibleParams = FindRule<T>();
            if (possibleParams == null)
                return false;
            var tweaked = tweak?.Invoke(possibleParams);
            rules.Add(tweaked ?? possibleParams);
            return true;
        }
    }



        
}