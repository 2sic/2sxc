using ToSic.Eav.Documentation;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        /// <inheritdoc />
        [PrivateApi]
        public string ObjToString(object uiOrParams, string prefix = null) 
            => new ObjectToUrl().SerializeIfNotString(uiOrParams, prefix);
    }
}
