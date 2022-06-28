using ToSic.Eav.Documentation;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Special helper to generate edit toolbars in the front-end.
    /// It is used in combination with `@Edit.Toolbar(...)`.
    /// It's especially useful for complex rules like Metadata-buttons which are more complex to create. 
    /// </summary>
    /// <remarks>
    /// History
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
        /// <param name="target">
        ///     The optional content-item this toolbar is for. Can be null. <br/>
        ///     Usually a [](xref:NetCode.DynamicData.DynamicEntity) or a [](xref:NetCode.DynamicData.Entity)
        /// 
        ///     _Added in v14.03_
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">
        /// Parameters for the UI, like `color=red` - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// </remarks>
        IToolbarBuilder Default(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null
        );

        /// <summary>
        /// Build a Toolbar configuration using the `empty` toolbar to use with `@Edit.Toolbar`
        /// It's a fluid API, so the returned object can be extended with further `Add(...)` or special helpers to quickly create complex configurations.
        /// For guidance what to give it, also check out the [toolbar docs](xref:JsCode.Toolbars.Simple).
        /// </summary>
        /// <param name="target">
        ///     The optional content-item this toolbar is for. Can be null. <br/>
        ///     Usually a [](xref:NetCode.DynamicData.DynamicEntity) or a [](xref:NetCode.DynamicData.Entity)
        /// 
        ///     _Added in v14.03_
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">
        /// Parameters for the UI, like `color=red` - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// </remarks>
        IToolbarBuilder Empty(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null
        );

        /// <summary>
        /// Build an **empty** Toolbar with a Metadata button.
        /// 
        /// This is the same as .Empty().Metadata(target, contentType);
        /// </summary>
        /// <param name="target">The target object which should receive metadata. Must support <see cref="ToSic.Eav.Metadata.IHasMetadata"/> </param>
        /// <param name="contentTypes">Name of one or more content-types for which to generate the button(s). For many, use comma `,` to separate. If not specified, will try to lookup config (v14)</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">
        /// Parameters for the UI, like `color=red` - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <param name="parameters">
        /// Parameters for the command.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <param name="context">EXPERIMENTAL - not final</param>
        /// <returns>An toolbar builder with empty configuration and just this button on it</returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// * contentTypes changed from one to many in v14
        /// * contentTypes can also have `*` or `SomeType,*` in v14
        /// * contentTypes can also be optional, in which case it behaves as if it was `*` in v14 - if no config is found, it will not add a metadata-button
        /// * parameter context added in 2sxc 14 - still WIP/experimental
        /// </remarks>
        IToolbarBuilder Metadata(
            object target,
            string contentTypes = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string context = null
        );
        
    }
}
