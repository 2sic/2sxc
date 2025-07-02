using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Internal;
using ToSic.Sxc.Services.Internal;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services;

[PrivateApi("Hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ToolbarService(Generator<IToolbarBuilder> toolbarGenerator)
    : ServiceWithContext($"{SxcLogName}.TlbSvc", connect: [toolbarGenerator]), IToolbarService
{

    /// <inheritdoc />
    public IToolbarBuilder Default(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        object? prefill = null
    ) => ToolbarBuilder(noParamOrder: noParamOrder, tweak: tweak, toolbarTemplate: ToolbarRuleToolbar.Default, ui: ui, parameters: parameters, prefill: prefill, context: null, target: target);


    /// <inheritdoc />
    public IToolbarBuilder Empty(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        object? prefill = null
    ) => ToolbarBuilder(noParamOrder: noParamOrder, tweak: tweak, toolbarTemplate: ToolbarRuleToolbar.Empty, ui: ui, parameters: parameters, prefill: prefill, context: null, target: target);


    /// <inheritdoc />
    public IToolbarBuilder Metadata(
        object target,
        string? contentTypes = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        object? prefill = null,
        string? context = null
    ) => Empty().Metadata(target: target, contentTypes: contentTypes, noParamOrder: noParamOrder, tweak: tweak, ui: ui, parameters: parameters, prefill: prefill, context: context);

    public IToolbarBuilder Edit(object target, NoParamOrder noParamOrder = default, Func<ITweakButton, ITweakButton>? tweak = default)
        => Empty().Edit(target: target, noParamOrder: noParamOrder, tweak: tweak);



    private IToolbarBuilder ToolbarBuilder(
        NoParamOrder noParamOrder,
        string toolbarTemplate,
        Func<ITweakButton, ITweakButton>? tweak,
        object? ui,
        object? parameters,
        object? prefill,
        string? context,
        object? target)
    {
        var l = Log.Fn<IToolbarBuilder>($"{nameof(toolbarTemplate)}:{toolbarTemplate}");
        
        // The following lines must be just as this, because it's a functional object, where each call may return a new copy
        var tlb = (ToolbarBuilder)toolbarGenerator.New();
        tlb.ConnectToRoot(ExCtxOrNull);

        tlb = (ToolbarBuilder)tlb
            .Toolbar(toolbarTemplate: toolbarTemplate, target: target, tweak: tweak, ui: ui, parameters: parameters, prefill: prefill);

        if (_defaultUi.HasValue())
            tlb = (ToolbarBuilder)tlb.Settings(ui: _defaultUi);

        if (context.HasValue())
            tlb = tlb.AddInternal([new ToolbarRuleGeneric($"context?{context}")]);

        return l.Return(tlb);
    }


    internal void _setDemoDefaults(string? defaultUi)
        => _defaultUi = defaultUi;
    private string? _defaultUi;
}