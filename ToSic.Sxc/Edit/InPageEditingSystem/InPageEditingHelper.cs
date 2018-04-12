using System;
using System.Web;
using Newtonsoft.Json;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent;
using ToSic.SexyContent.Edit.Toolbar;

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public class InPageEditingHelper : HasLog, IInPageEditingSystem
    {
        private readonly string _jsonTemplate =
            "data-list-context='{{ `parent`: {0}, `field`: `{1}`, `type`: `{2}`, `guid`: `{3}`}}'".Replace("`", "\"");

        internal InPageEditingHelper(SxcInstance sxcInstance, Log parentLog) : base("Edt", parentLog)
        {
            Enabled = sxcInstance.UserMayEdit;
            SxcInstance = sxcInstance;
        }

        public bool Enabled { get; }
        protected SxcInstance SxcInstance;

        #region Toolbar

        public HtmlString Toolbar(DynamicEntity target = null,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string actions = null,
            string contentType = null, 
            object prefill = null,
            object toolbar = null,
            object settings = null)
        {
            Log.Add("ctx toolbar - enabled:{Enabled}");
            if (!Enabled) return null;
            Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Toolbar");

            var itmToolbar = new ItemToolbar(target, actions, contentType, prefill, toolbar, settings);

            return new HtmlString(itmToolbar.Toolbar);
        }

        #endregion Toolbar

        #region Context Attributes

        /// <summary>
        /// Get html-attributes to mark the current context
        /// these will be added to a wrapper tag (usually a div)
        /// so that in-page editing knows what the context is
        /// </summary>
        /// <param name="target">the object for is part of the context</param>
        /// <param name="dontRelyOnParameterOrder">this is just to ensure you will use named params for any other param</param>
        /// <param name="field">the field name - in case of list-contexts</param>
        /// <param name="contentType">type name for new items - usually for inner-content and list-contexts</param>
        /// <param name="newGuid">the guid of a new item - use null for auto-generate</param>
        /// <returns></returns>
        public HtmlString ContextAttributes(DynamicEntity target,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string field = null,
            string contentType = null, 
            Guid? newGuid = null)
        {
            Log.Add("ctx attribs - enabled:{Enabled}");
            if (!Enabled) return null;
            Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "ContextAttributes");

            if (field == null) throw new Exception("need parameter 'field'");

            return new HtmlString(string.Format(
                _jsonTemplate,
                target.EntityId,
                field,
                contentType ?? Settings.AttributeSetStaticNameContentBlockTypeName,
                newGuid));
        }


        public HtmlString WrapInContext(object content,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            string tag = Constants.DefaultContextTag,
            bool full = false,
            bool? enableEdit = null,
            int instanceId = 0,
            int contentBlockId = 0
        )
        {
            return new HtmlString(
                SxcInstance.RenderingHelper.WrapInContext(content.ToString(),
                    instanceId: instanceId > 0
                        ? instanceId
                        : SxcInstance?.ContentBlock?.ParentId ?? 0,
                    contentBlockId: contentBlockId > 0
                        ? contentBlockId
                        : SxcInstance?.ContentBlock?.ContentBlockId ?? 0,
                    editContext: enableEdit ?? Enabled)
            );
        }

        #endregion Context Attributes

        #region Attribute-helper

        /// <summary>
        /// Generate an HTML attribute 
        /// - but only if in edit mode
        /// </summary>
        public HtmlString Attribute(string name, string value)
            => !Enabled ? null : SexyContent.Html.Build.Attribute(name, value);

        /// <summary>
        /// Generate an HTML attribute by converting the value to JSON 
        /// - but only in edit mode
        /// </summary>
        public HtmlString Attribute(string name, object value)
            => !Enabled ? null : SexyContent.Html.Build.Attribute(name, JsonConvert.SerializeObject(value));

        #endregion Attribute Helper

        #region Scripts and CSS includes

        public string EnableUi(string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            bool js = true, 
            bool context = false, 
            bool styles = false)
        {
            // only update the values if true, otherwise leave untouched
            if (js)
                SxcInstance.UiAddEditApi = true;

            if (styles)
                SxcInstance.UiAddEditUi = true;

            if (context)
                SxcInstance.UiAddEditContext = true;

            return null;
        }

        #endregion Scripts and CSS includes
    }
}