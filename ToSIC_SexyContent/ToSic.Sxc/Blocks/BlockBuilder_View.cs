using System.Linq;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {

        public IView View
        {
            get => Block.View;
            set => Block.View = value;
        }

        internal void SetTemplateOrOverrideFromUrl(IView defaultView)
        {
            View = defaultView;
            // skip if not relevant or not yet initialized
            if (Block.IsContentApp || App == null) return;

            // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
            var viewFromUrlParam = TryToGetTemplateBasedOnUrlParams();
            if (viewFromUrlParam != null)
                View = viewFromUrlParam;
        }

        private IView TryToGetTemplateBasedOnUrlParams()
        {
            Log.Add("template override - check");
            if (Context.Page.Parameters == null) return null;

            var urlParameterDict = Context.Page.Parameters.ToDictionary(pair => pair.Key?.ToLower() ?? "", pair =>
                $"{pair.Key}/{pair.Value}".ToLower());

            var allTemplates = new CmsRuntime(App, Log, UserMayEdit, false).Views.GetAll();


            foreach (var template in allTemplates.Where(t => !string.IsNullOrEmpty(t.UrlIdentifier)))
            {
                var desiredFullViewName = template.UrlIdentifier.ToLower();
                if (desiredFullViewName.EndsWith("/.*"))   // match details/.* --> e.g. details/12
                {
                    var keyName = desiredFullViewName.Substring(0, desiredFullViewName.Length - 3);
                    if (urlParameterDict.ContainsKey(keyName))
                    {
                        Log.Add("template override - found:" + template.Name);
                        return template;
                    }
                }
                else if (urlParameterDict.ContainsValue(desiredFullViewName)) // match view/details
                {
                    Log.Add("template override - found:" + template.Name);
                    return template;
                }
            }

            Log.Add("template override - none");
            return null;
        }


    }
}
