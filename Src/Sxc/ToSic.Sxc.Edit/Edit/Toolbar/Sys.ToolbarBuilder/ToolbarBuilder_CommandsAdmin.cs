using System.Runtime.CompilerServices;
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    private IToolbarBuilder AddAdminAction(
        string verb,
        NoParamOrder npo,
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

        var uiTweaked = PrepareUi(ui, tweaks: tweaks?.UiMerge);
        TargetCheck(target);
        return this.AddInternal([
                new ToolbarRuleCustom(
                    verb,
                    operation: ToolbarRuleOperation.Pick(operation, ToolbarRuleOps.OprAuto, tweaks?.ConditionValue),
                    ui: uiTweaked,
                    parameters: paramsTweaked,
                    operationCode: operation.HasValue() ? null : target as string)
            ],
            methodName: methodName);
    }
        
        
    public IToolbarBuilder App(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("app", npo, ui, parameters, operation, target, tweak);

    public IToolbarBuilder AppImport(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("app-import", npo, ui, parameters, operation, target, tweak);
        
    public IToolbarBuilder AppResources(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("app-resources", npo, ui, parameters, operation, target, tweak);

    public IToolbarBuilder AppSettings(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("app-settings", npo, ui, parameters, operation, target, tweak);

    public IToolbarBuilder Apps(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("apps", npo, ui, parameters, operation, target, tweak);

    public IToolbarBuilder System(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("system", npo, ui, parameters, operation, target, tweak);


    public IToolbarBuilder Insights(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("insights", npo, ui, parameters, operation, target, tweak);

}