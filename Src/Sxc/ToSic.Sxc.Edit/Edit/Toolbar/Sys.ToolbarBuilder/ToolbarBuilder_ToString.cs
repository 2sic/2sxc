using System.Text.Json;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Sys.Render.PageFeatures;
using Attribute = ToSic.Razor.Markup.Attribute;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    private const string ErrRenderMessage = "error: can't render toolbar to html, missing context";

    /// <summary>
    /// Also overwrite ToString() to keep functionality similar to before switch from class to record.
    /// Probably not relevant though.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => ToHtmlString();

    protected override string ToHtmlString()
    {
        // Get edit, but don't exit if null, as the Render (later on) will add comments if Edit is null
        var editSvc = ExCtx.GetService<IEditService>(reuse: true);

        // As the config can change before render, put `this` into a variable first
        var finalToolbar = this;
        var (enabled, showNonAdmin) = CheckShowConditions(editSvc);

        // If we show to everyone, then we must make sure that the toolbars API is loaded in JS
        // since in this case it may not be activated
        if (showNonAdmin)
        {
            ExCtx.GetService<IPageService>(reuse: true)
                .Activate(SxcPageFeatures.ToolbarsInternal.NameId);

            finalToolbar = this with { Configuration = Configuration with { ShowForce = true } };
        }

        // Show toolbar or a Demo-Informative-Toolbar
        if (ShouldSwitchToItemDemoMode())
            finalToolbar = CreateStandaloneItemDemoToolbar();       // Implement Demo-Mode with info-button only

        // don't show toolbar if not enabled (or not for everyone)
        if (!enabled)
            return "";
        return finalToolbar.Render(editSvc);
    }

    private (bool enabled, bool showNonAdmin) CheckShowConditions(IEditService? editSvc)
    {
        // Get initial enabled from the Edit Service
        var enabled = editSvc?.Enabled == true;

        // Check force show for Everyone, because
        // 1. the configuration says so
        // 2. because the page-level Edit object says we're in demo mode
        var showNonAdmins = Configuration.ShowForEveryone == true;
        enabled = enabled || showNonAdmins;

        // Check if enabled for certain groups
        if (ExCtx != null! /* paranoid */)
        {
            var user = ExCtx.GetCmsContext().User;
            var overrideShow = new ToolbarConfigurationShowHelper()
                .OverrideShowBecauseOfRoles(Configuration, user);
            enabled = overrideShow ?? enabled;
            showNonAdmins = overrideShow ?? showNonAdmins;
        }

        // If enabled, then still check if conditions would deny again
        // Check if conditions don't allow - in which case we return nothing.
        // Only test conditions if the toolbar would show - otherwise ignore
        if (!enabled)
            return (enabled, showNonAdmins);

        if (Configuration.Condition == false || Configuration.ConditionFunc?.Invoke() == false)
            return (false, false);

        return (enabled, showNonAdmins);
    }




    private string Render(IEditService? edit)
    {
        var mode = (Configuration.HtmlMode ?? ToolbarHtmlModes.OnTag).ToLowerInvariant();
        switch (mode)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            case ToolbarHtmlModes.OnTag:
                return edit == null
                    ? new Attribute(ToolbarConstants.ToolbarAttributeName, ErrRenderMessage).ToString() // add error
                    //: edit.TagToolbar(this)?.ToString();
                    : new ItemToolbarV14(this).Render(true).ToString();
            case ToolbarHtmlModes.Standalone:
                return edit == null
                    ? $"<!-- {ErrRenderMessage} -->" // add error
                    //: edit.Toolbar(this)?.ToString();       // Show toolbar
                    : new ItemToolbarV14(this).Render(false).ToString();
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