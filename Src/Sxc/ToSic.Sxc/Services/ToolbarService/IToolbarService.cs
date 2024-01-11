using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services;

/// <summary>
/// Special helper to generate edit toolbars in the front-end.
/// It's especially useful custom and/or complex rules like Metadata-buttons.
/// </summary>
/// <remarks>
/// History
/// * uses the [](xref:NetCode.Conventions.Functional)
/// * Added in 2sxc 13
/// * parameter `target` added to `Default()` and `Empty()` in v14.03
/// </remarks>
[PublicApi]
public interface IToolbarService
{
    /// <summary>
    /// Build a Toolbar configuration using the `default` template/buttons to use with `@Edit.Toolbar`
    /// It's a fluid API, so the returned object can be extended with further `Add(...)` or special helpers to quickly create complex configurations.
    /// For guidance what to give it, also check out the [toolbar docs](xref:JsCode.Toolbars.Simple).
    /// </summary>
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters (new v16.02)</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="prefill">_optional_ prefill for the edit-UI, see [prefill guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Prefill)</param>
    /// <returns></returns>
    /// <remarks>
    /// History
    /// * Added in 2sxc 13
    /// * target, ui, parameters added in v14.04
    /// </remarks>
    IToolbarBuilder Default(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        object prefill = null
    );

    /// <summary>
    /// Build a Toolbar configuration using the `empty` toolbar to use with `@Edit.Toolbar`
    /// It's a fluid API, so the returned object can be extended with further `Add(...)` or special helpers to quickly create complex configurations.
    /// For guidance what to give it, also check out the [toolbar docs](xref:JsCode.Toolbars.Simple).
    /// </summary>
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters (new v16.02)</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="prefill">_optional_ prefill for the edit-UI, see [prefill guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Prefill)</param>
    /// <returns></returns>
    /// <remarks>
    /// History
    /// * Added in 2sxc 13
    /// * target, ui, parameters added in v14.04
    /// </remarks>
    IToolbarBuilder Empty(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        object prefill = null
    );

    /// <summary>
    /// Build an **empty** Toolbar with a Metadata button.
    /// 
    /// This is the same as `.Empty().Metadata(...)`
    /// </summary>
    /// <param name="target">The target object which should receive metadata. Must support <see cref="ToSic.Eav.Metadata.IHasMetadata"/> </param>
    /// <param name="contentTypes">Name of one or more content-types for which to generate the button(s). For many, use comma `,` to separate. If not specified, will try to lookup config (v14)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters (new v16.02)</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="prefill">_optional_ prefill for the edit-UI, see [prefill guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Prefill)</param>
    /// <param name="context">EXPERIMENTAL - not final</param>
    /// <returns>An toolbar builder with empty configuration and just this button on it</returns>
    /// <remarks>
    /// History
    /// * Added in 2sxc 13
    /// * contentTypes changed from one to many in v14
    /// * contentTypes can also have `*` or `YourCustomType,*` in v14
    /// * contentTypes can also be optional, in which case it behaves as if it was `*` in v14 - if no config is found, it will not add a metadata-button
    /// * parameter context added in 2sxc 14 - still WIP/experimental
    /// </remarks>
    IToolbarBuilder Metadata(
        object target,
        string contentTypes = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        object prefill = null,
        string context = null
    );


    /// <summary>
    /// Build an **empty** Toolbar with a Edit button.
    /// 
    /// This is the same as `.Empty().Edit(...)`
    /// </summary>
    /// <param name="target">The target object which should receive metadata. Must support <see cref="ToSic.Eav.Metadata.IHasMetadata"/> </param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters (new v16.02)</param>
    /// <returns>An toolbar builder with empty configuration and just this button on it</returns>
    /// <remarks>
    /// History
    /// * Added in 2sxc 17
    /// </remarks>
    IToolbarBuilder Edit(
        object target,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default
    );
}