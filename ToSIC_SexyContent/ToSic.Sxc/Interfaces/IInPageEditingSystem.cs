using System;
using System.Web;
using ToSic.Eav.PublicApi;
using ToSic.SexyContent;

namespace ToSic.Sxc.Interfaces
{
    [PublicApi]
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
        HtmlString Toolbar(object target = null,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
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
        HtmlString TagToolbar(object target = null,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string actions = null,
            string contentType = null,
            object prefill = null,
            object toolbar = null,
            object settings = null);

        /// <summary>
        /// Get html-attributes to mark the current context
        /// these will be added to a wrapper tag (usually a div)
        /// so that in-page editing knows what the context is
        /// </summary>
        /// <param name="target">the object for is part of the context</param>
        /// <param name="dontRelyOnParameterOrder">this is just to ensure you will use named params for any other param</param>
        /// <param name="field">the field name - in case of list-contexts</param>
        /// <param name="contentType">type name for new items - usually for inner-content and list-contexts</param>
        /// <param name="newGuid">the guid of a new item - use null for auto-generate</param>
        /// <returns></returns>

        HtmlString ContextAttributes(DynamicEntity target, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string field = null, 
            string contentType = null,
            Guid? newGuid = null);

        /// <summary>
        /// Wrap something in a context wrapper-tag
        /// This is mainly meant for internal use
        /// </summary>
        /// <param name="content">the string / tags to wrap</param>
        /// <param name="dontRelyOnParameterOrder">protection parameter to ensure rest of stuff is named</param>
        /// <param name="tag">optional tag to use for the wrapper, default is div</param>
        /// <param name="full">include full context (default is partial context only)</param>
        /// <param name="enableEdit">include information needed for editing</param>
        /// <param name="instanceId">id to include in context - important for API calls</param>
        /// <param name="contentBlockId">content block this is for - important for API calls</param>
        /// <returns></returns>
        [PrivateApi]
        HtmlString WrapInContext(object content,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
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
        string Enable(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            bool? api = null, 
            bool? forms = null,
            bool? context = null,
            bool? autoToolbar = null,
            bool? styles = null);

        /// <summary>
        /// Generate an HTML attribute by converting the value to JSON 
        /// - but only in edit mode
        /// </summary>
        HtmlString Attribute(string name, string value);

        /// <summary>
        /// Generate an HTML attribute by converting the value to JSON 
        /// - but only in edit mode
        /// </summary>
        HtmlString Attribute(string name, object value);
    }
}
