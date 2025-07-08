using System.Runtime.CompilerServices;
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    private IToolbarBuilder AddAdminAction(
        string verb,
        NoParamOrder noParamOrder,
        object? ui,
        object? parameters,
        string? operation,
        object? target,
        Func<ITweakButton, ITweakButton>? tweak,
        [CallerMemberName] string? methodName = default
    )
    {
        // process tweaks, but skip early to reduce calls if null
        var tweaks = tweak == null
            ? null
            : RunTweaksOrErrorIfCombined(tweak: tweak, ui: ui, parameters: parameters, methodName: methodName);
        var paramsTweaked = tweaks == null && parameters == null
            ? null
            : Utils.PrepareParams(parameters, tweaks);

        var tweaksInt = tweaks as ITweakButtonInternal;
        var uiTweaked = PrepareUi(ui, tweaks: tweaksInt?.UiMerge);
        TargetCheck(target);
        return this.AddInternal([
                new ToolbarRuleCustom(
                    verb,
                    operation: ToolbarRuleOperation.Pick(operation, ToolbarRuleOps.OprAuto, tweaksInt?._condition),
                    ui: uiTweaked,
                    parameters: paramsTweaked,
                    operationCode: operation.HasValue() ? null : target as string)
            ],
            methodName: methodName);
    }
        
        
    public IToolbarBuilder App(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("app", noParamOrder, ui, parameters, operation, target, tweak);

    public IToolbarBuilder AppImport(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("app-import", noParamOrder, ui, parameters, operation, target, tweak);
        
    public IToolbarBuilder AppResources(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("app-resources", noParamOrder, ui, parameters, operation, target, tweak);

    public IToolbarBuilder AppSettings(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("app-settings", noParamOrder, ui, parameters, operation, target, tweak);

    public IToolbarBuilder Apps(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("apps", noParamOrder, ui, parameters, operation, target, tweak);

    public IToolbarBuilder System(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("system", noParamOrder, ui, parameters, operation, target, tweak);


    public IToolbarBuilder Insights(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("insights", noParamOrder, ui, parameters, operation, target, tweak);

}