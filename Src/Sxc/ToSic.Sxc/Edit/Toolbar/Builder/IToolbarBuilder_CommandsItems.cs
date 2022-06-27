using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {


        IToolbarBuilder Edit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            //string contentType = null,
            object ui = null,
            object parameters = null, 
            object prefill = null
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
        /// <param name="context">EXPERIMENTAL - not final</param>
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
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string context = null
        );


        IToolbarBuilder Publish(
            object target = null,
            string noParamOrder = Parameters.Protector,
            bool? show = null,
            object ui = null,
            object parameters = null
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
        /// <param name="context">EXPERIMENTAL - not final</param>
        /// <returns>A new toolbar builder which has been extended with this button</returns>
        /// <remarks>
        /// Added in v14.02
        /// </remarks>
        IToolbarBuilder Copy(
            object target = null,
            string noParamOrder = Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null,
            string context = null
        );

        

    }
}