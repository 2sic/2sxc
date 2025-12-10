using ToSic.Razor.Markup;
using ToSic.Sxc.Edit.Toolbar.Sys;


namespace ToSic.Sxc.Edit.EditService;

partial class EditService
{
    private const string InnerContentAttribute = "data-list-context";

    /// <inheritdoc />
    public IRawHtmlString? Toolbar(
        object? target = null,
        NoParamOrder npo = default,
        string? actions = null,
        string? contentType = null,
        object? condition = null,
        object? prefill = null,
        object? settings = null,
        object? toolbar = null)
        => ToolbarInternal(false, target, npo, actions, contentType, condition, prefill, settings, toolbar)
            ?.Render(false);

    /// <inheritdoc/>
    public IRawHtmlString? TagToolbar(
        object? target = null,
        NoParamOrder npo = default,
        string? actions = null,
        string? contentType = null,
        object? condition = null,
        object? prefill = null,
        object? settings = null,
        object? toolbar = null)
        => ToolbarInternal(true, target, npo, actions, contentType, condition, prefill, settings, toolbar)
            ?.Render(true);

    private ItemToolbarBase? ToolbarInternal(
        bool inTag,
        object? target,
        // ReSharper disable once UnusedParameter.Local
        NoParamOrder npo,
        string? actions,
        string? contentType,
        object? condition,
        object? prefill,
        object? settings,
        object? toolbar)
    {
        var l = Log.Fn<ItemToolbarBase?>($"enabled:{Enabled}; inline{inTag}");

        // #DropRoutingToolbarBuilderThroughEditService v20
        //// new v17.08 - force-show for everyone
        //var tlbConfig = (target as ToolbarBuilder)?.Configuration;
        //var forceShow = tlbConfig?.ShowForce == true;

        // #DropRoutingToolbarBuilderThroughEditService v20
        //if (!Enabled && !forceShow)
        if (!Enabled)
            return l.ReturnNull("not enabled");

        if (!IsConditionOk(condition))
            return l.ReturnNull("condition false");

        // #DropRoutingToolbarBuilderThroughEditService v20
        //// New in v13: The first parameter can also be a ToolbarBuilder, in which case all other params are ignored
        //if (target is IToolbarBuilder tlbBuilder)
        //    return l.Return(new ItemToolbarV14(tlbBuilder),
        //        "Using new modern Item-Toolbar, will ignore all other parameters.");
        
        // ensure that internally we always process it as an entity
        var eTarget = target as IEntity
                      ?? (target as ICanBeEntity)?.Entity;
        
        if (target != null && eTarget == null)
            l.W("Creating toolbar - it seems the object provided was neither null, IEntity nor DynamicEntity");

        // #DropRoutingToolbarBuilderThroughEditService v20
        //if (toolbar is IToolbarBuilder tlbBuilder2)
        //    return l.Return(new ItemToolbarV14(tlbBuilder2, eTarget),
        //        "Using new modern Item-Toolbar with an entity, will ignore all other parameters.");
        
        return l.Return(ItemToolbarPicker.ItemToolbar(eTarget, actions, contentType,
            prefill: prefill, settings: settings, toolbar: toolbar), "Using classic mode, with all parameters.");
    }

    private bool IsConditionOk(object? condition)
    {
        var l = Log.Fn<bool>();
        // Null = no condition and certainly not false, say ok
        if (condition == null)
            return l.ReturnTrue("null,true");

        // Bool (non-null) and nullable
        if (condition is false)
            return l.ReturnFalse("bool false");
        if (condition as bool? == false)
            return l.ReturnFalse("null false");

        // Int are only false if exactly 0
        if (condition is 0)
            return l.ReturnFalse("int 0");
        if (condition as int? == 0)
            return l.ReturnFalse("int nullable 0");

        // String
        if (condition is string s &&
            string.Equals(s, false.ToString(), StringComparison.InvariantCultureIgnoreCase))
            return l.ReturnFalse("string false");

        // Anything else: true
        return l.ReturnTrue("default,true");
    }

}