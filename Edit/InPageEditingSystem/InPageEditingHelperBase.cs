using System.Web;

namespace ToSic.SexyContent.Edit.InPageEditingSystem
{
    public class InPageEditingHelperBase: IInPageEditingSystem
    {
        private readonly string _jsonTemplate = "data-list-context='{{ `parent`: {0}, `field`: `{1}`, `type`: `{2}`}}'".Replace("`", "\"");
        private readonly SxcInstance _sxcInstance;

        internal InPageEditingHelperBase(SxcInstance sxc)
        {
            _sxcInstance = sxc;
        }

        public bool Enabled => _sxcInstance.Environment.Permissions.UserMayEditContent;

        public HtmlString Toolbar(DynamicEntity target)//, string actions = null) 
            => Enabled ? target.Toolbar : null;


        public HtmlString ContextAttributes(DynamicEntity target, string fieldName, string typeNameForNew = null)
        {
            return Enabled
                ? new HtmlString(string.Format(
                    _jsonTemplate, target.EntityId, fieldName, typeNameForNew ?? Settings.AttributeSetStaticNameContentBlockTypeName))
                : null;
        }
}
}