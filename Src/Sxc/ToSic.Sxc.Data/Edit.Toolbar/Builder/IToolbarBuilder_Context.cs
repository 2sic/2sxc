﻿namespace ToSic.Sxc.Edit.Toolbar;

internal interface IToolbarBuilderInternal
{
    [PrivateApi("WIP / Debugging")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    ToolbarContext? GetContext();

    [PrivateApi("WIP 14.07.04")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    IToolbarBuilder Context(
        object target
    );
}

public partial interface IToolbarBuilder
{
    /// <summary>
    /// Set the main target of this toolbar.
    /// </summary>
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    /// <remarks>
    /// New in v14.04
    /// </remarks>
    IToolbarBuilder For(object target);

    /// <summary>
    /// Detect if the toolbar should go into demo-mode.
    /// 
    /// </summary>
    /// <param name="root"></param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="message">Optional message or a resources key such as `Resources.ToolbarShowingDemo`</param>
    /// <returns></returns>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP 16.02")]
    IToolbarBuilder DetectDemo(
        ICanBeEntity root,
        NoParamOrder noParamOrder = default,
        string? message = default);

    /// <summary>
    /// Condition to apply if the toolbar would show, but maybe shouldn't.
    /// For example, you can prevent the toolbar from appearing if it's the Demo-Item.
    ///
    /// For expensive conditions, use the overload which accepts a function. 
    /// </summary>
    /// <param name="condition">true/false</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
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
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    /// <remarks>
    /// New in v14.04
    /// </remarks>
    IToolbarBuilder Condition(Func<bool> condition);

    /// <summary>
    /// Specify an audience for the toolbar - to activate even if the user wouldn't normally see it.
    /// </summary>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="everyone">default is `null`, set to true to make everybody see this.</param>
    /// <param name="roleNames">list/array of role names which should _also_ see the toolbar (new v20)</param>
    /// <param name="denyRoleNames">list/array of role names which should _never_ see the toolbar - has precedence over allow (new v20)</param>
    /// <returns></returns>
    /// <remarks>
    /// Normally only people with admin permissions would see a toolbar.
    /// Specifying the audience will make it appear even if you are not an admin.
    /// 
    /// Reasons for this would be to have some special buttons for a certain group of users.
    ///
    /// * New in v17.08, experimental
    /// * released v19
    /// * updated with roleNames/denyRoleNames v20
    /// </remarks>
    IToolbarBuilder Audience(
        NoParamOrder protector = default,
        bool? everyone = default,
        IEnumerable<string>? roleNames = default,
        IEnumerable<string>? denyRoleNames = default);

    /// <summary>
    /// Adds a button group to the toolbar.
    /// All following buttons will be in this group automatically.
    ///
    /// Can also be used to remove a group of buttons on the `default` toolbar, such as the group `view`.
    /// See [list of groups on default](xref:Basics.Browser.EditUx.Toolbars.ButtonGroups)
    /// </summary>
    /// <param name="name">_optional_ - name of new group or `-name` to remove an existing group.</param>
    /// <returns></returns>
    /// <remarks>
    /// New in v14.08
    /// </remarks>
    IToolbarBuilder Group(string? name = null);

}