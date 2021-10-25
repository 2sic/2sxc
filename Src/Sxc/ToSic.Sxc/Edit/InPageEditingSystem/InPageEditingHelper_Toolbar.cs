using System;
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

        /// <inheritdoc />
        public HtmlString Toolbar(object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string actions = null,
            string contentType = null,
            object prefill = null,
            object toolbar = null,
            object settings = null, 
            object condition = null) 
            => ToolbarInternal(false, target, noParamOrder, actions, contentType, prefill, toolbar,
            settings, condition);

        /// <inheritdoc/>
        public HtmlString TagToolbar(object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string actions = null,
            string contentType = null,
            object prefill = null,
            object toolbar = null,
            object settings = null,
            object condition = null) 
            => ToolbarInternal(true, target, noParamOrder, actions, contentType, prefill, toolbar,
            settings, condition);

        private HtmlString ToolbarInternal(bool inTag, object target,
            string noParamOrder,
            string actions,
            string contentType,
            object prefill,
            object toolbar,
            object settings,
            object condition)
        {
            var wrapLog = Log.Call<HtmlString>($"enabled:{Enabled}; inline{inTag}");
            if (!Enabled) return wrapLog("not enabled", null);
            if (!IsConditionOk(condition)) return wrapLog("condition false", null);

            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Toolbar", $"{nameof(actions)},{nameof(contentType)},{nameof(prefill)},{nameof(toolbar)},{nameof(settings)}");

            // ensure that internally we always process it as an entity
            var eTarget = target as IEntity ?? (target as IDynamicEntity)?.Entity;
            if (target != null && eTarget == null)
                Log.Warn("Creating toolbar - it seems the object provided was neither null, IEntity nor DynamicEntity");
            var itmToolbar = new ItemToolbar(eTarget, actions, contentType, prefill, toolbar, settings);

            var result = inTag
                ? Attribute("sxc-toolbar", itmToolbar.ToolbarAttribute())
                : new HtmlString(itmToolbar.Toolbar);
            return wrapLog("ok", result);
        }

        private bool IsConditionOk(object condition)
        {
            var wrapLog = Log.Call<bool>();

            // Null = no condition and certainly not false, say ok
            if (condition == null) return wrapLog("null,true", true);

            // Bool (non-null) and nullable
            if (condition is bool b && b == false) return wrapLog($"{false}", false);
            if (condition as bool? == false) return wrapLog("null false", false);

            // Int are only false if exactly 0
            if (condition is int i && i == 0) return wrapLog("int 0", false);
            if (condition as int? == 0) return wrapLog("int nullable 0", false);

            // String
            if (condition is string s &&
                string.Equals(s, false.ToString(), StringComparison.InvariantCultureIgnoreCase))
                return wrapLog("string false", false);
            
            // Anything else: true
            return wrapLog("default,true", true);
        }

    }
}