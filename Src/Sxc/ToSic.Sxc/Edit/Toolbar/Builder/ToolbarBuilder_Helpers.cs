using ToSic.Eav.Documentation;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOperations;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {

        private char OperationShow(bool? show) => show == null ? (char)BtnAddAuto : show.Value ? (char)BtnAdd : (char)BtnRemove;


        [PrivateApi]
        private string ObjToString(object uiOrParams/*, string prefix = null*/) 
            => new ObjectToUrl().SerializeIfNotString(uiOrParams/*, prefix*/);
    }
}
