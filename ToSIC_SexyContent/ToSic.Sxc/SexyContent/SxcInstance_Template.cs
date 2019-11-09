using System.Linq;
using ToSic.Sxc.Views;

namespace ToSic.SexyContent
{
    public partial class SxcInstance
    {

        public IView View
        {
            get => ContentBlock.View;
            set => ContentBlock.View = value;
        }

        internal void SetTemplateOrOverrideFromUrl(IView defaultView)
        {
            View = defaultView;
            // skif if not relevant or not yet initialized
            if (IsContentApp || App == null) return;

            // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
            var templateFromUrl = TryToGetTemplateBasedOnUrlParams();
            if (templateFromUrl != null)
                View = templateFromUrl;
        }

        private IView TryToGetTemplateBasedOnUrlParams()
        {
            Log.Add("template override - check");
            if (Parameters == null) return null;

            // new 2016-05-01
            var urlParameterDict = Parameters.ToDictionary(pair => pair.Key?.ToLower() ?? "", pair =>
                $"{pair.Key}/{pair.Value}".ToLower());


            foreach (var template in App.ViewManager.GetAllTemplates().Where(t => !string.IsNullOrEmpty(t.UrlIdentifier)))
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
