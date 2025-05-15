namespace ToSic.Sxc.Edit.Toolbar;

public partial interface IToolbarBuilder
{
    /// <summary>
    /// Create (or reconfigure) the button to **delete an item**.
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
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="operation">_optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder Delete(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    );

    /// <summary>
    /// Create button to **edit an item**.
    /// Can also be used to remove the same button on a toolbar which would have it by default. 
    /// </summary>
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="prefill">_optional_ prefill for the edit-UI, see [prefill guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Prefill)</param>
    /// <param name="operation">_optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder Edit(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = default,
        object parameters = default, 
        object prefill = default,
        string operation = default
    );

    /// <summary>
    /// Create button to **create a new item**.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// </summary>
    /// <param name="target">
    /// X Options
    /// * an entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
    /// * a string with the content-type name
    /// </param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="prefill">_optional_ prefill for the edit-UI, see [prefill guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Prefill)</param>
    /// <param name="operation">_optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder New(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        object prefill = null,
        string operation = null
    );

    /// <summary>
    /// Create button to **show a data-admin dialog** with all the data-items / entities of a specific content type.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// </summary>
    /// <param name="target">
    /// 3 Options: 
    /// * an entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
    /// * a `string` containing the type name
    /// * a modifier keyword such as `remove` or `-` to remove the button
    /// </param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="filter">object or string with the filters for the data view see [filter](xref:ToSic.Sxc.Services.ToolbarBuilder.DataFilter)</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="operation">_optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder Data(
        object target = null, // entity-like or content-type name
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object filter = null,
        object ui = null,
        object parameters = null,
        string operation = null
    );


    /// <summary>
    /// Create button to **add or edit metadata** to the specified object and using the content-type specified here.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// </summary>
    /// <param name="target">
    /// The target object which should receive metadata.
    /// Must support <see cref="ToSic.Eav.Metadata.IHasMetadata"/>.
    /// Often an entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target),
    /// but can also other metadata supporting objects, like an Asset, Page, Site, etc.
    /// </param>
    /// <param name="contentTypes">Name of one or more content-types for which to generate the button(s). For many, use comma `,` to separate. If not specified, will try to lookup config (v14)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="operation">_optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation)</param>
    /// <param name="context">EXPERIMENTAL - not final</param>
    /// <param name="prefill">_optional_ prefill for the edit-UI, see [prefill guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Prefill)</param>
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
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        object prefill = null,
        string operation = null,
        string context = null
    );

    /// <summary>
    /// Create button to publish the current item.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// By default it will only appear if the current item is draft/unpublished.
    /// You can change this (but probably shouldn't) by setting an `operation`. 
    /// </summary>
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="operation">_optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder Publish(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    );


    /// <summary>
    /// Create button to **copy an item**.
    /// Can also be used to remove the same button on a toolbar which would have it by default.
    /// It needs the item which it will copy as a parameter.
    /// </summary>
    /// <param name="target">
    /// * an entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)
    /// * can also be a int (number) entityId. If you only supply the entity ID, you must also supply the `contentType`.
    /// </param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="contentType"></param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="operation">_optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation)</param>
    /// <param name="context">EXPERIMENTAL - not final</param>
    /// <param name="prefill">_optional_ prefill for the edit-UI, see [prefill guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Prefill)</param>
    /// <returns>A new toolbar builder which has been extended with this button</returns>
    /// <remarks>
    /// Added in v14.02
    /// </remarks>
    IToolbarBuilder Copy(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        string contentType = null,
        object ui = null,
        object parameters = null,
        object prefill = null,
        string operation = null,
        string context = null
    );

        

}