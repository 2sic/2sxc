using System;
using System.Web;
using ToSic.SexyContent;

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public interface IInPageEditingSystem
    {
        /// <summary>
        /// If editing is enabled or not
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Generate a toolbar tag - must be used in normal html, not as an attribute
        /// </summary>
        /// <param name="target">The item this toolbar is for, can be null</param>
        /// <param name="dontRelyOnParameterOrder">Special parameter to force named-parameters for any other setting</param>
        /// <param name="actions">list of actions on this toolbar, null means default actions for this item</param>
        /// <param name="contentType">content-type of this toolbar, used when it offers new/add buttons</param>
        /// <param name="prefill">prefill information, for new items</param>
        /// <param name="toolbar">complex manual toolbar configuration if needed - providing this will cause actions to be ignored</param>
        /// <param name="settings">toolbar settings controlling hover etc.</param>
        /// <returns>If the user is an editor, it returns HTML UL tag containing all the toolbar configuration</returns>
        HtmlString Toolbar(DynamicEntity target = null, 
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string actions = null, 
            string contentType = null, 
            object prefill = null,
            object toolbar = null,
            object settings = null);

        /// <summary>
        /// Generate a toolbar attribute inside an html-tag
        /// </summary>
        /// <param name="target">The item this toolbar is for, can be null</param>
        /// <param name="dontRelyOnParameterOrder">Special parameter to force named-parameters for any other setting</param>
        /// <param name="actions">list of actions on this toolbar, null means default actions for this item</param>
        /// <param name="contentType">content-type of this toolbar, used when it offers new/add buttons</param>
        /// <param name="prefill">prefill information, for new items</param>
        /// <param name="toolbar">complex manual toolbar configuration if needed - providing this will cause actions to be ignored</param>
        /// <param name="settings">toolbar settings controlling hover etc.</param>
        /// <returns>If the user is an editor, it returns the attribute containing all the toolbar configuration</returns>
        HtmlString ToolbarAttribute(DynamicEntity target = null,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            string actions = null,
            string contentType = null,
            object prefill = null,
            object toolbar = null,
            object settings = null);


        HtmlString ContextAttributes(DynamicEntity target, 
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string field = null, 
            string contentType = null,
            Guid? newGuid = null);

        HtmlString WrapInContext(object content,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            string tag = Constants.DefaultContextTag,
            bool full = false,
            bool? enableEdit = null,
            int instanceId = 0,
            int contentBlockId = 0
        );

        /// <summary>
        /// Ensure that the UI will load the correct assets to enable editing
        /// </summary>
        /// <param name="dontRelyOnParameterOrder"></param>
        /// <param name="api">if JS etc. should be included to enable editing API</param>
        /// <param name="forms"></param>
        /// <param name="context">If context should be added, to ensure in-instance data editing</param>
        /// <param name="autoToolbar"></param>
        /// <param name="styles"></param>
        /// <returns>null - but we wanted to make sure it returns something, so you can use it in razor like @Edit.EnableUi()</returns>
        string Enable(string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            bool? api = null, 
            bool? forms = null,
            bool? context = null,
            bool? autoToolbar = null,
            bool? styles = null);

        HtmlString Attribute(string name, string value);

        HtmlString Attribute(string name, object value);
    }
}
