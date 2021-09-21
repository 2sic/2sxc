using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.Toolbar;
using IEntity = ToSic.Eav.Data.IEntity;
#if NET451
using HtmlString = System.Web.HtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
#endif


namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public partial class InPageEditingHelper
    {
        private readonly string innerContentAttribute = "data-list-context";

        #region Toolbar

        /// <inheritdoc />
        public HtmlString Toolbar(object target = null, 
            string noParamOrder = Eav.Parameters.Protector, 
            string actions = null, 
            string contentType = null, 
            object prefill = null, 
            object toolbar = null, 
            object settings = null) 
            => ToolbarInternal(false, target, noParamOrder, actions, contentType, prefill, toolbar,
            settings);

        /// <inheritdoc/>
        public HtmlString TagToolbar(object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string actions = null,
            string contentType = null,
            object prefill = null,
            object toolbar = null,
            object settings = null) 
            => ToolbarInternal(true, target, noParamOrder, actions, contentType, prefill, toolbar,
            settings);

        private HtmlString ToolbarInternal(bool inTag, object target,
            string noParamOrder,
            string actions,
            string contentType,
            object prefill,
            object toolbar,
            object settings,
            bool? condition = true)
        {
            Log.Add($"context toolbar - enabled:{Enabled}; inline{inTag}");
            if (!Enabled) return null;
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Toolbar", $"{nameof(actions)},{nameof(contentType)},{nameof(prefill)},{nameof(toolbar)},{nameof(settings)}");

            // ensure that internally we always process it as an entity
            var eTarget = target as IEntity ?? (target as IDynamicEntity)?.Entity;
            if (target != null && eTarget == null)
                Log.Warn("Creating toolbar - it seems the object provided was neither null, IEntity nor DynamicEntity");
            var itmToolbar = new ItemToolbar(eTarget, actions, contentType, prefill, toolbar, settings);

            return inTag 
                ? Attribute("sxc-toolbar", itmToolbar.ToolbarAttribute()) 
                : new HtmlString(itmToolbar.Toolbar);
        }



        #endregion Toolbar



    }
}