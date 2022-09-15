﻿using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    /// <summary>
    /// Special utilities for the Toolbar Builder,
    /// which should only be created once per use case to optimize performance.
    ///
    /// Note that previously this was part of the ToolbarBuilder,
    /// but the effect was since the toolbar builder is re-created constantly, the
    /// helpers were also recreated constantly. 
    /// </summary>
    internal class ToolbarBuilderUtilities
    {
        /// <summary>
        /// Helper to process 'parameters' to url, ensuring lower-case etc. 
        /// </summary>
        public ObjectToUrl Par2Url => _par2U.Get(() => new ObjectToUrl(null, new[] { new UrlValueCamelCase() }));
        private readonly GetOnce<ObjectToUrl> _par2U = new GetOnce<ObjectToUrl>();


        /// <summary>
        /// Helper to process 'filter' to url - should not change the case of the properties and auto-fix some special scenarios
        /// </summary>
        public ObjectToUrl Filter2Url => _f2U.Get(() => new ObjectToUrl(null, new[] { new FilterValueProcessor() })
        {
            ArrayBoxStart = "[",
            ArrayBoxEnd = "]"
        });
        private readonly GetOnce<ObjectToUrl> _f2U = new GetOnce<ObjectToUrl>();


        /// <summary>
        /// Helper to process 'prefill' - should not change the case of the properties
        /// </summary>
        public ObjectToUrl Prefill2Url => _pref2U.Get(() => new ObjectToUrl());
        private readonly GetOnce<ObjectToUrl> _pref2U = new GetOnce<ObjectToUrl>();

        internal static ObjectToUrl GetUi2Url() => new ObjectToUrl(null, new UrlValueProcess[]
        {
            new UrlValueCamelCase(),
            new UiValueProcessor()
        });


        public string PrepareUi(
            object ui,
            object uiMerge = null,
            string uiMergePrefix = null,
            string group = null // current button-group name which must be merged into the Ui parameter
        )
        {
            var uiString = Ui2Url.SerializeWithChild(ui, uiMerge, uiMergePrefix);
            if (group.HasValue()) uiString = Ui2Url.SerializeWithChild(uiString, $"group={group}");
            return uiString;
        }
        private ObjectToUrl Ui2Url => _ui2Url.Get(GetUi2Url);
        private readonly GetOnce<ObjectToUrl> _ui2Url = new GetOnce<ObjectToUrl>();

    }
}