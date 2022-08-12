using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        [PrivateApi]
        private string ObjToString(object uiOrParams) => O2U.SerializeIfNotString(uiOrParams);


        private ObjectToUrl O2U => _o2U.Get(() => new ObjectToUrl());
        private readonly GetOnce<ObjectToUrl> _o2U = new GetOnce<ObjectToUrl>();

        private ObjectToUrl Ui2Url => _ui2Url.Get(() => new ObjectToUrl(null, new[] { new UrlValueCamelCase() }));
        private readonly GetOnce<ObjectToUrl> _ui2Url = new GetOnce<ObjectToUrl>();
    }
}
