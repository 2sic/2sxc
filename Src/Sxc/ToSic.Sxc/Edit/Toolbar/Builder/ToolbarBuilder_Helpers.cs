using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private string ParToString(object uiOrParams) => Par2Url.Serialize(uiOrParams);


        private ObjectToUrl Par2Url => _o2U.Get(() => new ObjectToUrl(null, new[] { new UrlValueCamelCase() }));
        private readonly GetOnce<ObjectToUrl> _o2U = new GetOnce<ObjectToUrl>();



        [PrivateApi]
        internal static ObjectToUrl GetUi2Url() => new ObjectToUrl(null, new UrlValueProcess[]
        {
            new UrlValueCamelCase(),
            new UiValueProcessor()
        });

        [PrivateApi]
        private string PrepareUi(
            object ui,
            object uiMerge = null,
            string uiMergePrefix = null
        )
        {
            var uiString = Ui2Url.SerializeWithChild(ui, uiMerge, uiMergePrefix);
            var group = _configuration?.Group;
            if (group.HasValue()) uiString = Ui2Url.SerializeWithChild(uiString, $"group={group}");
            return uiString;
        }
        private ObjectToUrl Ui2Url => _ui2Url.Get(GetUi2Url);
        private readonly GetOnce<ObjectToUrl> _ui2Url = new GetOnce<ObjectToUrl>();

    }
}
