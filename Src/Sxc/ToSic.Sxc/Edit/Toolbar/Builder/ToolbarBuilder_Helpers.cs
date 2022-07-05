using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        [PrivateApi]
        private string ObjToString(object uiOrParams) => O2U.SerializeIfNotString(uiOrParams);


        private ObjectToUrl O2U => _o2u.Get(() => new ObjectToUrl());
        private readonly GetOnce<ObjectToUrl> _o2u = new GetOnce<ObjectToUrl>();
    }
}
