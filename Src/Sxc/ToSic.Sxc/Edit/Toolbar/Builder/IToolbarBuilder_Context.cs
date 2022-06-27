using System;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Edit.Toolbar
{

    public partial interface IToolbarBuilder
    {

        [PrivateApi]
        ToolbarContext Context();
        

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

    }
}