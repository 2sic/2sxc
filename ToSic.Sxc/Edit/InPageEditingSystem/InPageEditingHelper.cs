using System;
using System.Web;
using Newtonsoft.Json;
using ToSic.Eav.Configuration;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent;
using ToSic.Sxc.Edit.Toolbar;
using Feats = ToSic.Eav.Configuration.Features;

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

        public HtmlString Toolbar(object target = null, 
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string actions = null, 
            string contentType = null, 
            object prefill = null, 
            object toolbar = null, 
            object settings = null) 
            => ToolbarInternal(false, target, dontRelyOnParameterOrder, actions, contentType, prefill, toolbar,
            settings);

        public HtmlString TagToolbar(object target = null,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            string actions = null,
            string contentType = null,
            object prefill = null,
            object toolbar = null,
            object settings = null) 
            => ToolbarInternal(true, target, dontRelyOnParameterOrder, actions, contentType, prefill, toolbar,
            settings);

        private HtmlString ToolbarInternal(bool inline, object target,
            string dontRelyOnParameterOrder,
            string actions,
            string contentType,
            object prefill,
            object toolbar,
            object settings)
        {
            Log.Add($"context toolbar - enabled:{Enabled}; inline{inline}");
            if (!Enabled) return null;
            Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Toolbar", $"{nameof(actions)},{nameof(contentType)},{nameof(prefill)},{nameof(toolbar)},{nameof(settings)}");

            // ensure that internally we always process it as an entity
            var eTarget = target as IEntity ?? (target as DynamicEntity)?.Entity;
            if (target != null && eTarget == null)
                Log.Warn("Creating toolbar - it seems the object provided was neither null, IEntity nor DynamicEntity");
            var itmToolbar = new ItemToolbar(eTarget, actions, contentType, prefill, toolbar, settings);

            return inline ? EditorAttribute("sxc-toolbar", itmToolbar.ToolbarAttribute) : new HtmlString(itmToolbar.Toolbar);
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
            Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "ContextAttributes", $"{nameof(field)},{nameof(contentType)},{nameof(newGuid)}");

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
            Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "WrapInContext", $"{nameof(tag)},{nameof(full)},{nameof(enableEdit)},{nameof(instanceId)},{nameof(contentBlockId)}");

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
        public HtmlString EditorAttribute(string name, string value)
            => !Enabled ? null : SexyContent.Html.Build.Attribute(name, value);

        /// <summary>
        /// Generate an HTML attribute by converting the value to JSON 
        /// - but only in edit mode
        /// </summary>
        public HtmlString EditorAttribute(string name, object value)
            => !Enabled ? null : SexyContent.Html.Build.Attribute(name, JsonConvert.SerializeObject(value));

        #endregion Attribute Helper

        #region Scripts and CSS includes


        public string Enable(string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, bool? api = null, bool? forms = null, bool? context = null, bool? autoToolbar = null, bool? styles = null)
        {
            Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Enable", $"{nameof(api)},{nameof(forms)},{nameof(context)},{nameof(autoToolbar)},{nameof(autoToolbar)},{nameof(styles)}");

            // check if feature enabled
            var feats = new[] {FeatureIds.PublicForms};
            if (!Feats.EnabledOrException(feats, "public forms not available", out var exp))
                throw exp;
            //if (!Feats.Enabled(feats))
            //    throw new Exception($"public forms not available - {Feats.MsgMissingSome(feats)}");

            // only update the values if true, otherwise leave untouched
            if (api.HasValue || forms.HasValue)
                SxcInstance.UiAddEditApi = api ?? forms.Value;

            if (styles.HasValue)
                SxcInstance.UiAddEditUi = styles.Value;

            if (context.HasValue)
                SxcInstance.UiAddEditContext = context.Value;

            if (autoToolbar.HasValue)
                SxcInstance.UiAutoToolbar = autoToolbar.Value;

            return null;
        }

        #endregion Scripts and CSS includes
    }
}