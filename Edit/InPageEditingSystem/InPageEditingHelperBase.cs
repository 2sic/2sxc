using System;
using System.Web;
using ToSic.SexyContent.Edit.Toolbar;

namespace ToSic.SexyContent.Edit.InPageEditingSystem
{
    public class InPageEditingHelperBase : IInPageEditingSystem
    {
        private readonly string _jsonTemplate =
            "data-list-context='{{ `parent`: {0}, `field`: `{1}`, `type`: `{2}`, `guid`: `{3}`}}'".Replace("`", "\"");

        private readonly SxcInstance _sxcInstance;

        internal InPageEditingHelperBase(SxcInstance sxc)
        {
            _sxcInstance = sxc;
        }

        // ReSharper disable once UnusedParameter.Local
        private void ProtectAgainstMissingParameterNames(string criticalParameter)
        {
            if (criticalParameter == null || criticalParameter != Constants.RandomProtectionParameter)
                throw new Exception(
                    "when using the toolbar command, please use named parameters - otherwise you are relying on the parameter order staying the same.");
        }

        public bool Enabled => _sxcInstance?.Environment?.Permissions?.UserMayEditContent ?? false;

        public HtmlString Toolbar(DynamicEntity target = null,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string actions = null,
            string contentType = null, 
            object prefill = null,
            object toolbar = null,
            object settings = null)
        {
            if (!Enabled) return null;
            ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder);

            var itmToolbar = new ItemToolbar(target, actions, contentType, prefill, toolbar, settings);

            return new HtmlString(itmToolbar.Toolbar);
        }


        public HtmlString ContextAttributes(DynamicEntity target,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string field = null,
            string contentType = null, 
            Guid? newGuid = null)
        {
            if (!Enabled) return null;
            ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder);

            if(field == null) throw new Exception("need parameter field");

            return new HtmlString(string.Format(
                _jsonTemplate, 
                target.EntityId, 
                field,
                contentType ?? Settings.AttributeSetStaticNameContentBlockTypeName,
                newGuid));
        }
    }
}