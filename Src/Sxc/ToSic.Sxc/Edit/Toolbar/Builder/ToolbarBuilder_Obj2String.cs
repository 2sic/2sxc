using ToSic.Eav.Documentation;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        /// <inheritdoc />
        [PrivateApi]
        public string ObjToString(object uiOrParams)
        {
            if (uiOrParams == null) return null;
            if (uiOrParams is string str) return str;

            return new ObjectToUrl().Serialize(uiOrParams);
        }
    }
}
