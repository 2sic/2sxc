using System;
using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Web;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Edit
{
    /// <summary>
    /// The toolbar builder helps you create Toolbar configurations for the UI.
    /// Note that it has a fluid API, and each method/use returns a fresh object with the updated configuration.
    /// </summary>
    /// <remarks>
    /// Your code cannot construct this object by itself, as it usually needs additional information.
    /// To get a `ToolbarBuilder`, use the <see cref="Services.IToolbarService"/>.
    ///
    /// History
    /// * Added in 2sxc 13, just minimal API
    /// * massively enhanced in v14.04
    /// </remarks>
    [PublicApi]
    public interface IToolbarBuilder: IHybridHtmlString, IHasLog, INeedsDynamicCodeRoot
    {
        /// <summary>
        /// Get a toolbar builder which would render to HTML as a standalone tag.
        /// </summary>
        /// <returns></returns>
        [WorkInProgressApi("wip")]
        IToolbarBuilder AsTag();

        /// <summary>
        /// Get a toolbar builder which would render to HTML as attributes on an existing tag.
        /// Note that this is the default, so you will usually not need this. 
        /// </summary>
        /// <returns></returns>
        [WorkInProgressApi("wip")]
        IToolbarBuilder AsAttributes();

        /// <summary>
        /// Test code
        /// </summary>
        /// <returns></returns>
        [PrivateApi("wip")]
        IToolbarBuilder AsJson();


        IToolbarBuilder Add(
            object target = null,
            string noParamOrder = Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null
        );

        IToolbarBuilder Edit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            //string contentType = null,
            object ui = null,
            object parameters = null, 
            object prefill = null
        );

        /// <summary>
        /// Add one or more rules (as strings or ToolbarRule objects) according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple)
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// </remarks>
        [PrivateApi("Would confuse people, since they cannot create ToolbarRule objects")]
        IToolbarBuilder AddInternal(params object[] rules);

        #region Button Commands

        /// <summary>
        /// Add one or more rules according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple).
        ///
        /// Note that you can actually add many buttons but the name is still Button not Buttons, for API consistency.
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// - new in 14.04 WIP
        /// </remarks>
        IToolbarBuilder ButtonAdd(params string[] rules);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">
        /// The command name.
        /// See [](xref:Api.Js.SxcJs.CommandNames)
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">
        /// Parameters for the UI, like `color=red` - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <param name="parameters">
        /// Parameters for the command.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// - new in 14.04 WIP
        /// </remarks>
        [WorkInProgressApi("still WIP")]
        IToolbarBuilder ButtonModify(
            string name,
            string noParamOrder = Parameters.Protector,
            //object target = null,
            object ui = null,
            object parameters = null);

        /// <summary>
        /// Remove buttons from the toolbar.
        /// Usually in combination with the `Default` toolbar which already has many buttons, for eg to remove the `layout` button.
        ///
        /// Note that you can actually remove many buttons but the name is still Button not Buttons, for API consistency.
        /// </summary>
        /// <param name="names">
        /// One or more command names.
        /// See [](xref:Api.Js.SxcJs.CommandNames)
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// - new in 14.04 WIP
        /// </remarks>
        [WorkInProgressApi("still WIP")]
        IToolbarBuilder ButtonRemove(params string[] names);

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

        #endregion


        [PrivateApi("WIP 13.11 - not sure if we actually make it public, as it's basically metadata with automatic content-type - not published yet")]
        IToolbarBuilder Image(
            object target,
            string noParamOrder = Parameters.Protector,
            string ui = null,
            string parameters = null
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


        /// <summary>
        /// Add a `settings` rule to configure what the toolbar should look like. See [](xref:JsCode.Toolbars.Settings)
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="show"></param>
        /// <param name="hover"></param>
        /// <param name="follow"></param>
        /// <param name="classes"></param>
        /// <param name="autoAddMore"></param>
        /// <param name="ui">
        /// Parameters for the UI, like `color=red` - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <param name="parameters">Parameters for the command - doesn't really have an effect on Settings, but included for consistency</param>
        /// <returns>A new toolbar builder which has been extended with this settings-rule</returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// </remarks>
        IToolbarBuilder Settings(
            string noParamOrder = Parameters.Protector,
            string show = null,
            string hover = null,
            string follow = null,
            string classes = null,
            string autoAddMore = null,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Converts the configuration to a json-string according to the JS-Toolbar specs.
        /// </summary>
        /// <returns></returns>
        string ToString();


        [PrivateApi]
        ToolbarContext Context();

        [PrivateApi]
        IToolbarBuilder With(
            string noParamOrder = Parameters.Protector,
            string mode = null,
            object target = null
        );

        /// <summary>
        /// Set the main target of this toolbar.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <remarks>
        /// New in v14.04
        /// </remarks>
        IToolbarBuilder Target(object target);

        /// <summary>
        /// Condition to apply if the toolbar would show, but maybe shouldn't.
        /// For example, you can prevent the toolbar from appearing if it's the Demo-Item.
        ///
        /// For expensive conditions, use the overload which accepts a function. 
        /// </summary>
        /// <param name="condition">true/false</param>
        /// <returns></returns>
        /// <remarks>
        /// New in v14.04
        /// </remarks>
        IToolbarBuilder Condition(bool condition);

        /// <summary>
        /// Condition to apply if the toolbar would show, but maybe shouldn't.
        /// For example, you can prevent the toolbar from appearing if it's the Demo-Item.
        ///
        /// This accepts a function to check the condition.
        /// It will only run if the toolbar would already show. 
        /// </summary>
        /// <param name="condition">function such as `() => true`</param>
        /// <returns></returns>
        /// <remarks>
        /// New in v14.04
        /// </remarks>
        IToolbarBuilder Condition(Func<bool> condition);

        [PrivateApi("Internal use only, can change at any time")]
        string ObjToString(object uiOrParams, string prefix = null);
    }
}