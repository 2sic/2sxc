using System;
using System.Web;
using ToSic.SexyContent.Edit.Toolbar;

namespace ToSic.SexyContent.Edit.InPageEditingSystem
{
    public class InPageEditingHelperBase : IInPageEditingSystem
    {
        private readonly string _jsonTemplate =
            "data-list-context='{{ `parent`: {0}, `field`: `{1}`, `type`: `{2}`}}'".Replace("`", "\"");

        private readonly SxcInstance _sxcInstance;

        internal InPageEditingHelperBase(SxcInstance sxc)
        {
            _sxcInstance = sxc;
        }

        private void protectAgainstMissingParameterNames(string criticalParameter)
        {
            if (criticalParameter != Constants.RandomProtectionParameter)
                throw new Exception(
                    "when using this command, please use named parameters - otherwise you are relying on the parameter order staying the same.");
        }

        public bool Enabled => _sxcInstance?.Environment?.Permissions?.UserMayEditContent ?? false;

        public HtmlString Toolbar(DynamicEntity target = null,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string actions = null,
            string contentType = null, 
            object prefill = null)
        {
            if (!Enabled) return null;
            protectAgainstMissingParameterNames(dontRelyOnParameterOrder);

            var toolbar = new ItemToolbar(target, actions, contentType, prefill);

            return new HtmlString(toolbar.Toolbar);
        }


        public HtmlString ContextAttributes(DynamicEntity target,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string field = null,
            string contentType = null)
        {
            if (!Enabled) return null;
            protectAgainstMissingParameterNames(dontRelyOnParameterOrder);

            if(field == null) throw new Exception("need parameter field");

            return new HtmlString(string.Format(
                _jsonTemplate, target.EntityId, field,
                contentType ?? Settings.AttributeSetStaticNameContentBlockTypeName));
        }
    }
}