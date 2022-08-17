using System;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Edit.Toolbar
{

    public partial interface IToolbarBuilder
    {

        [PrivateApi]
        ToolbarContext GetContext();

        [PrivateApi("WIP 14.07.04")]
        IToolbarBuilder Context(
            object target
        );


        /// <summary>
        /// Set the main target of this toolbar.
        /// </summary>
        /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
        /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
        /// <remarks>
        /// New in v14.04
        /// </remarks>
        [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP - still choosing name for/target")]
        IToolbarBuilder For(object target);

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
        /// Adds a button group to the toolbar.
        /// All following buttons will be in this group automatically.
        ///
        /// Can also be used to remove a group of buttons on the `default` toolbar, such as the group `view`.
        /// See [list of groups on default](xref:Basics.Browser.EditUx.Toolbars.ButtonGroups)
        /// </summary>
        /// <param name="name">_optional_ - name of new group or `-name` to remove an existing group.</param>
        /// <returns></returns>
        /// <remarks>
        /// New in v14.07.05
        /// </remarks>
        IToolbarBuilder Group(string name = null);

    }
}