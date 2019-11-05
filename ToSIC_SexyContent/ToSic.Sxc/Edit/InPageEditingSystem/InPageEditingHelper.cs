using System;
using System.Web;
using Newtonsoft.Json;
using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging;
using ToSic.SexyContent;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Interfaces;
using Feats = ToSic.Eav.Configuration.Features;

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public class InPageEditingHelper : HasLog, IInPageEditingSystem
    {
        private readonly string _jsonTemplate =
            "data-list-context='{{ `parent`: {0}, `field`: `{1}`, `type`: `{2}`, `guid`: `{3}`}}'".Replace("`", "\"");

        internal InPageEditingHelper(SxcInstance sxcInstance, ILog parentLog) : base("Edt", parentLog)
        {
            Enabled = sxcInstance.UserMayEdit;
            SxcInstance = sxcInstance;
        }

        /// <inheritdoc />
        public bool Enabled { get; }
        [PrivateApi]
        protected SxcInstance SxcInstance;

        #region Toolbar

        /// <inheritdoc />
        public HtmlString Toolbar(object target = null, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string actions = null, 
            string contentType = null, 
            object prefill = null, 
            object toolbar = null, 
            object settings = null) 
            => ToolbarInternal(false, target, dontRelyOnParameterOrder, actions, contentType, prefill, toolbar,
            settings);

        /// <inheritdoc/>
        public HtmlString TagToolbar(object target = null,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
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
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Toolbar", $"{nameof(actions)},{nameof(contentType)},{nameof(prefill)},{nameof(toolbar)},{nameof(settings)}");

            // ensure that internally we always process it as an entity
            var eTarget = target as IEntity ?? (target as IDynamicEntity)?.Entity;
            if (target != null && eTarget == null)
                Log.Warn("Creating toolbar - it seems the object provided was neither null, IEntity nor DynamicEntity");
            var itmToolbar = new ItemToolbar(eTarget, actions, contentType, prefill, toolbar, settings);

            return inline ? Attribute("sxc-toolbar", itmToolbar.ToolbarAttribute) : new HtmlString(itmToolbar.Toolbar);
        }



        #endregion Toolbar

        #region Context Attributes

        /// <inheritdoc/>
        public HtmlString ContextAttributes(IDynamicEntity target,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string field = null,
            string contentType = null, 
            Guid? newGuid = null)
        {
            Log.Add("ctx attribs - enabled:{Enabled}");
            if (!Enabled) return null;
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "ContextAttributes", $"{nameof(field)},{nameof(contentType)},{nameof(newGuid)}");

            if (field == null) throw new Exception("need parameter 'field'");

            return new HtmlString(string.Format(
                _jsonTemplate,
                target.EntityId,
                field,
                contentType ?? Settings.AttributeSetStaticNameContentBlockTypeName,
                newGuid));
        }

        /// <inheritdoc/>
        [PrivateApi]
        public HtmlString WrapInContext(object content,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string tag = Constants.DefaultContextTag,
            bool full = false,
            bool? enableEdit = null,
            int instanceId = 0,
            int contentBlockId = 0
        )
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "WrapInContext", $"{nameof(tag)},{nameof(full)},{nameof(enableEdit)},{nameof(instanceId)},{nameof(contentBlockId)}");

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

        /// <inheritdoc/>
        public HtmlString Attribute(string name, string value)
            => !Enabled ? null : SexyContent.Html.Build.Attribute(name, value);

        /// <inheritdoc/>
        public HtmlString Attribute(string name, object value)
            => !Enabled ? null : SexyContent.Html.Build.Attribute(name, JsonConvert.SerializeObject(value));

        #endregion Attribute Helper

        #region Scripts and CSS includes

        /// <inheritdoc/>
        public string Enable(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, bool? api = null, bool? forms = null, bool? context = null, bool? autoToolbar = null, bool? styles = null)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Enable", $"{nameof(api)},{nameof(forms)},{nameof(context)},{nameof(autoToolbar)},{nameof(autoToolbar)},{nameof(styles)}");

            // check if feature enabled - if more than the api is needed
            // extend this list if new parameters are added
            if (forms.HasValue || styles.HasValue || context.HasValue || autoToolbar.HasValue)
            {
                var feats = new[] {FeatureIds.PublicForms};
                if (!Feats.EnabledOrException(feats, "public forms not available", out var exp))
                    throw exp;
            }

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