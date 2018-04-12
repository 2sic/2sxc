using System;
using System.Web;
using ToSic.SexyContent;

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public interface IInPageEditingSystem
    {
        bool Enabled { get; }
        HtmlString Toolbar(DynamicEntity target = null, 
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
        /// <param name="context">If context should be added, to ensure in-instance data editing</param>
        /// <param name="styles"></param>
        /// <returns>null - but we wanted to make sure it returns something, so you can use it in razor like @Edit.EnableUi()</returns>
        string Enable(string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            bool api = true, 
            bool context = false,
            bool styles = false);

        HtmlString Attribute(string name, string value);

        HtmlString Attribute(string name, object value);
    }
}
