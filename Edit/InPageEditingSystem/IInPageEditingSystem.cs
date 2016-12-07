using System;
using System.Web;

namespace ToSic.SexyContent.Edit.InPageEditingSystem
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

        string Attribute(string name, string value);

        string Attribute(string name, object value);
    }
}
