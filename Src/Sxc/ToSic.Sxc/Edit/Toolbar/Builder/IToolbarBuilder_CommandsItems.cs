using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Add / change the delete button on a toolbar.
        /// 
        /// This has a special behavior.
        /// The `default` toolbar already includes a delete-button in the third group.
        /// So if the toolbar is is a `default` this will just modify it to force-show.
        /// But it will still be in the third group of buttons.
        /// 
        /// For the `empty` toolbar it will just add the button in the normal way.
        ///
        /// To change this automatic behavior, use a `operation` = `modify` or `add`
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder Delete(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );

        /// <summary>
        /// Button to edit an item. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="prefill"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder Edit(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            //string contentType = null,
            object ui = null,
            object parameters = null, 
            object prefill = null,
            string operation = null
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">
        /// X Options
        /// * A entity, in which case it uses it to detect content type etc.
        /// * a string with the content-type name
        /// </param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="prefill"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder New(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null
        );

        /// <summary>
        /// Button to show a admin dialog with all the data-items / entities of a specific content type.
        /// </summary>
        /// <param name="target">
        /// 3 Options: 
        /// * An entity which will determine the type automatically
        /// * a `string` containing the type name
        /// * a modifier keyword such as `remove` or `-` to remove the button
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="filter">object or string with the filters for the data vew</param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder Data(
            object target = null, // entity-like or content-type name
            string noParamOrder = Eav.Parameters.Protector,
            // string contentType = null,
            object filter = null,
            object ui = null,
            object parameters = null,
            string operation = null
        );


        /// <summary>
        /// Create an add `metadata` rule to add or edit metadata to the specified object and using the content-type specified here. 
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
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <param name="context">EXPERIMENTAL - not final</param>
        /// <param name="prefill"></param>
        /// <returns>A new toolbar builder which has been extended with this button</returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// * contentTypes changed from one to many in v14
        /// * contentTypes can also have `*` or `SomeType,*` in v14
        /// * contentTypes can also be optional, in which case it behaves as if it was `*` in v14 - if no config is found, it will not add a metadata-button
        /// * parameter context added in 2sxc 14 - still WIP/experimental
        /// * changed ui and parameters to support object in v14.04
        /// </remarks>
        IToolbarBuilder Metadata(
            object target,
            string contentTypes = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null,
            string context = null
        );

        /// <summary>
        /// Button to publish the current item.
        /// By default it will only appear if the current item is draft/unpublished.
        /// You can change this (but probably shouldn't) by setting an `operation`. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <returns></returns>
        IToolbarBuilder Publish(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        );


        /// <summary>
        /// Create a toolbar rule to copy an item. It needs the item which it will copy as a parameter.
        /// </summary>
        /// <param name="target">
        ///     The target object which is either an <see cref="Eav.Data.IEntity"/> or an <see cref="Sxc.Data.IDynamicEntity"/>.
        ///     Can also be a int (number) entityId.
        ///     If you only supply the entity ID, you must also supply the `contentType`.
        ///     If it's `null` it will only work if the toolbar context already determines the primary item. 
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="contentType"></param>
        /// <param name="ui">
        ///     Parameters for the UI, like `color=red` - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options.
        ///     Can be a string, and can also be an object since v14.04
        /// </param>
        /// <param name="parameters">
        ///     Parameters for the command.
        ///     Can be a string, and can also be an object since v14.04
        /// </param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <param name="context">EXPERIMENTAL - not final</param>
        /// <param name="prefill"></param>
        /// <returns>A new toolbar builder which has been extended with this button</returns>
        /// <remarks>
        /// Added in v14.02
        /// </remarks>
        IToolbarBuilder Copy(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string operation = null,
            string context = null
        );

        

    }
}