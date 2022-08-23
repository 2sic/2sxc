using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        /// <summary>
        /// Helper to process 'parameters' to url, ensuring lower-case etc. 
        /// </summary>
        private ObjectToUrl Par2Url => _o2U.Get(() => new ObjectToUrl(null, new[] { new UrlValueCamelCase() }));
        private readonly GetOnce<ObjectToUrl> _o2U = new GetOnce<ObjectToUrl>();

        /// <summary>
        /// Helper to process 'filter' to url - should not change the case of the properties and auto-fix some special scenarios
        /// </summary>
        private ObjectToUrl Filter2Url => _f2U.Get(() => new ObjectToUrl(null, new[] { new FilterValueProcessor() })
        {
            ArrayBoxStart = "[",
            ArrayBoxEnd = "]"
        });
        private readonly GetOnce<ObjectToUrl> _f2U = new GetOnce<ObjectToUrl>();

        /// <summary>
        /// Helper to process 'prefill' - should not change the case of the properties
        /// </summary>
        private ObjectToUrl Prefill2Url => _p2U.Get(() => new ObjectToUrl());
        private readonly GetOnce<ObjectToUrl> _p2U = new GetOnce<ObjectToUrl>();

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

        internal static ObjectToUrl GetUi2Url() => new ObjectToUrl(null, new UrlValueProcess[]
        {
            new UrlValueCamelCase(),
            new UiValueProcessor()
        });

    }
}
