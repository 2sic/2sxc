using System.Web;

namespace ToSic.SexyContent.Edit.InPageEditingSystem
{
    public interface IInPageEditingSystem
    {
        bool Enabled { get; }
        HtmlString Toolbar(DynamicEntity target);//, string actions = null);
        
        HtmlString ContextAttributes(DynamicEntity target, string fieldName, string typeNameForNew = null);
    }
}
