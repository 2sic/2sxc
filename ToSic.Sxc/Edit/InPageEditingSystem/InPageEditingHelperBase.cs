using System;
using System.Web;
using Newtonsoft.Json;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Edit.Toolbar;

namespace ToSic.SexyContent.Edit.InPageEditingSystem
{
    public abstract class InPageEditingHelperBase : HasLog, IInPageEditingSystem
    {
        private readonly string _jsonTemplate =
            "data-list-context='{{ `parent`: {0}, `field`: `{1}`, `type`: `{2}`, `guid`: `{3}`}}'".Replace("`", "\"");

        internal InPageEditingHelperBase(bool enabled, Log parentLog) : base("Edt", parentLog)
        {
            Enabled = enabled;
        }

        public bool Enabled { get; }


        public HtmlString Toolbar(DynamicEntity target = null,
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter, 
            string actions = null,
            string contentType = null, 
            object prefill = null,
            object toolbar = null,
            object settings = null)
        {
            if (!Enabled) return null;
            Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Toolbar");

            var itmToolbar = new ItemToolbar(target, actions, contentType, prefill, toolbar, settings);

            return new HtmlString(itmToolbar.Toolbar);
        }
        
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

        /// <summary>
        /// Generate an HTML attribute 
        /// - but only if in edit mode
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HtmlString Attribute(string name, string value)
            // special notes: has trailing space to ensure attributes never "stick" together
            // we also don't want to use {HttpUtility.HtmlAttributeEncode(value)...
            // ...because it makes the html hard to work with when debugging
            // so we just manually replace all apos to make sure it doesn't create invalid html
            => !Enabled ? null : new HtmlString($" {name}='{value.Replace("'", "&apos;")}'");

        /// <summary>
        /// Generate an HTML attribute by converting the value to JSON 
        /// - but only in edit mode
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HtmlString Attribute(string name, object value)
            => !Enabled ? null : Attribute(name, JsonConvert.SerializeObject(value));
    }
}