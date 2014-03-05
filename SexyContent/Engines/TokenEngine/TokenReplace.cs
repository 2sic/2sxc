using System.Collections.Generic;
using System.Web;
using DotNetNuke.Services.Tokens;
using ToSic.SexyContent.DataSources.Tokens;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class TokenReplace : DotNetNuke.Services.Tokens.TokenReplace
    {
        public TokenReplace(DynamicEntity Content, DynamicEntity Presentation, DynamicEntity ListContent, DynamicEntity ListPresentation, Dictionary<string,string> List, App app) : base()
        {
            if (HttpContext.Current != null)
            {
                HttpRequest request = HttpContext.Current.Request;
                PropertySource.Add("querystring", new FilteredNameValueCollectionPropertyAccess(request.QueryString));
                PropertySource.Add("form", new FilteredNameValueCollectionPropertyAccess(request.Form));
                PropertySource.Add("server", new FilteredNameValueCollectionPropertyAccess(request.ServerVariables));

                // Add our expando, requires that we convert it to a string/string table
                if (Content != null)
                {
                    /*Dictionary<string, object> ContentDictionary = new Dictionary<string, object>(Content.Dictionary);*/
                    PropertySource.Add("content", new DynamicEntityPropertyAccess(Content));
                }
                if(Presentation != null)
                {
                    /*Dictionary<string, object> PresentationDictionary = new Dictionary<string, object>(Presentation.Dictionary);*/
                    PropertySource.Add("presentation", new DynamicEntityPropertyAccess(Presentation));
                }
                if (ListContent != null)
                {
                    /*Dictionary<string, object> ContentDictionary = new Dictionary<string, object>(ListContent.Dictionary);*/
                    PropertySource.Add("listcontent", new DynamicEntityPropertyAccess(ListContent));
                }
                if (ListPresentation != null)
                {
                    /*Dictionary<string, object> PresentationDictionary = new Dictionary<string, object>(ListPresentation.Dictionary);*/
                    PropertySource.Add("listpresentation", new DynamicEntityPropertyAccess(ListPresentation));
                }
                
                PropertySource.Add("app", new AppPropertyAccess(app));
                if(app.Settings != null)
                    PropertySource.Add("appsetting", new DynamicEntityPropertyAccess(app.Settings));
                if (app.Resources != null)
                    PropertySource.Add("appresource", new DynamicEntityPropertyAccess(app.Resources));
                PropertySource.Add("list", new DictionaryPropertyAccess(List));
            }
        }

        protected override string replacedTokenValue(string strObjectName, string strPropertyName, string strFormat)
        {
            return base.replacedTokenValue(strObjectName, strPropertyName, strFormat);
        }
    }
}