using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Get a toolbar builder which would render to HTML as a standalone tag.
        /// </summary>
        /// <param name="target">_Optional_ target / entity for which the toolbar should be.</param>
        /// <returns></returns>
        [WorkInProgressApi("wip")]
        IToolbarBuilder AsTag(
            object target = null
        );

        /// <summary>
        /// Get a toolbar builder which would render to HTML as attributes on an existing tag.
        /// Note that this is the default, so you will usually not need this. 
        /// </summary>
        /// <param name="target">_Optional_ target / entity for which the toolbar should be.</param>
        /// <returns></returns>
        [WorkInProgressApi("wip")]
        IToolbarBuilder AsAttributes(
            object target = null
        );


        /// <summary>
        /// Test code
        /// </summary>
        /// <returns></returns>
        /// <param name="target">_Optional_ target / entity for which the toolbar should be.</param>
        [PrivateApi("wip")]
        IToolbarBuilder AsJson(
            object target = null
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

        

        /// <summary>
        /// Converts the configuration to a json-string according to the JS-Toolbar specs.
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
        string ToString();


        //[PrivateApi("Internal use only, can change at any time")]
        //string ObjToString(object uiOrParams, string prefix = null);
    }
}