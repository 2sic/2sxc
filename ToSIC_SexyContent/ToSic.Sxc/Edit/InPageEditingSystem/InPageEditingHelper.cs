using System;
using System.Web;
using Newtonsoft.Json;
using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.SexyContent;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Web;
using CmsBlock = ToSic.Sxc.Blocks.CmsBlock;
using Feats = ToSic.Eav.Configuration.Features;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public class InPageEditingHelper : HasLog, IInPageEditingSystem
    {
        private readonly string _jsonTemplate =
            "data-list-context='{{ `parent`: {0}, `field`: `{1}`, `type`: `{2}`, `guid`: `{3}`}}'".Replace("`", "\"");

        internal InPageEditingHelper(ICmsBlock cmsInstance, ILog parentLog) : base("Edt", parentLog)
        {
            Enabled = cmsInstance.UserMayEdit;
            CmsInstance = cmsInstance;
        }

        /// <inheritdoc />
        public bool Enabled { get; }
        [PrivateApi]
        protected  ICmsBlock CmsInstance;

        #region Toolbar

        /// <inheritdoc />
        public HtmlString Toolbar(object target = null, 
            string noParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string actions = null, 
            string contentType = null, 
            object prefill = null, 
            object toolbar = null, 
            object settings = null) 
            => ToolbarInternal(false, target, noParameterOrder, actions, contentType, prefill, toolbar,
            settings);

        /// <inheritdoc/>
        public HtmlString TagToolbar(object target = null,
            string noParameterOrder = Eav.Constants.RandomProtectionParameter,
            string actions = null,
            string contentType = null,
            object prefill = null,
            object toolbar = null,
            object settings = null) 
            => ToolbarInternal(true, target, noParameterOrder, actions, contentType, prefill, toolbar,
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
            string noParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string field = null,
            string contentType = null, 
            Guid? newGuid = null)
        {
            Log.Add("ctx attribs - enabled:{Enabled}");
            if (!Enabled) return null;
            Eav.Constants.ProtectAgainstMissingParameterNames(noParameterOrder, "ContextAttributes", $"{nameof(field)},{nameof(contentType)},{nameof(newGuid)}");

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
            string noParameterOrder = Eav.Constants.RandomProtectionParameter,
            string tag = Constants.DefaultContextTag,
            bool full = false,
            bool? enableEdit = null,
            int instanceId = 0,
            int contentBlockId = 0
        )
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(noParameterOrder, "WrapInContext", $"{nameof(tag)},{nameof(full)},{nameof(enableEdit)},{nameof(instanceId)},{nameof(contentBlockId)}");

            return new HtmlString(
                ((CmsBlock)CmsInstance).RenderingHelper.WrapInContext(content.ToString(),
                    instanceId: instanceId > 0
                        ? instanceId
                        : CmsInstance?.Block?.ParentId ?? 0,
                    contentBlockId: contentBlockId > 0
                        ? contentBlockId
                        : CmsInstance?.Block?.ContentBlockId ?? 0,
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
        public string Enable(string noParameterOrder = "random-y023n", bool? js = null, bool? api = null,
            bool? forms = null, bool? context = null, bool? autoToolbar = null, bool? styles = null)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(noParameterOrder, "Enable", $"{nameof(js)},{nameof(api)},{nameof(forms)},{nameof(context)},{nameof(autoToolbar)},{nameof(autoToolbar)},{nameof(styles)}");

            // check if feature enabled - if more than the api is needed
            // extend this list if new parameters are added
            if (forms.HasValue || styles.HasValue || context.HasValue || autoToolbar.HasValue)
            {
                var feats = new[] {FeatureIds.PublicForms};
                if (!Feats.EnabledOrException(feats, "public forms not available", out var exp))
                    throw exp;
            }

            var hostWithInternals = (CmsBlock) CmsInstance;

            if (js.HasValue || api.HasValue || forms.HasValue)
                hostWithInternals.UiAddJsApi = (js ?? false) || (api ?? false) || (forms ?? false);

            // only update the values if true, otherwise leave untouched
            if (api.HasValue || forms.HasValue)
                hostWithInternals.UiAddEditApi = (api ?? false) || (forms ?? false);

            if (styles.HasValue)
                hostWithInternals.UiAddEditUi = styles.Value;

            if (context.HasValue)
                hostWithInternals.UiAddEditContext = context.Value;

            if (autoToolbar.HasValue)
                hostWithInternals.UiAutoToolbar = autoToolbar.Value;

            return null;
        }

        #endregion Scripts and CSS includes
    }
}