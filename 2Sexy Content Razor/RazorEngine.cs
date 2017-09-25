using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Compilation;
using System.Web.WebPages;
using DotNetNuke.Entities.Modules;
using ToSic.SexyContent.Razor;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.Search;

namespace ToSic.SexyContent.Engines
{
    public class RazorEngine : EngineBase
    {

        protected SexyContentWebPage Webpage { get; set; }

        protected override void Init()
        {
            try
            {
                InitWebpage();
            }
            // Catch web.config Error on DNNs upgraded to 7
            catch (ConfigurationErrorsException exc)
            {
                var e = new Exception("Configuration Error: Please follow this checklist to solve the problem: http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7", exc);
                throw e;
            }
        }

        protected HttpContextBase HttpContext 
            => System.Web.HttpContext.Current == null ? null : new HttpContextWrapper(System.Web.HttpContext.Current);

        public Type RequestedModelType()
        {
            if (Webpage != null)
            {
                var webpageType = Webpage.GetType();
                if (webpageType.BaseType.IsGenericType)
                    return webpageType.BaseType.GetGenericArguments()[0];
            }
            return null;
        }


        public void Render(TextWriter writer)
        {
            Log.Add("will render into textwriter");
            Webpage.ExecutePageHierarchy(new WebPageContext(HttpContext, Webpage, null), writer, Webpage);
        }

        /// <summary>
        /// Renders the template
        /// </summary>
        /// <returns></returns>
        protected override string RenderTemplate()
        {
            Log.Add("will render razor template");
            var writer = new StringWriter();
            Render(writer);
            return writer.ToString();
        }

        private object CreateWebPageInstance()
        {
            var compiledType = BuildManager.GetCompiledType(TemplatePath);
            object objectValue = null;
            if (compiledType != null)
            {
                objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
            }
            return objectValue;
        }

        private void InitHelpers(SexyContentWebPage webPage)
        {
            webPage.Html = new HtmlHelper();
            webPage.Url = new UrlHelper(ModuleInfo);
            webPage.Sexy = Sexy;
            webPage.AppAndDataHelpers = new AppAndDataHelpers(Sexy);

        }

        private void InitWebpage()
        {
            if (string.IsNullOrEmpty(TemplatePath)) return;

            var objectValue = RuntimeHelpers.GetObjectValue(CreateWebPageInstance());
            if (objectValue == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The webpage found at '{0}' was not created.", new object[] { TemplatePath }));

            Webpage = objectValue as SexyContentWebPage;

            if ((Webpage == null))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The webpage at '{0}' must derive from SexyContentWebPage.", new object[] { TemplatePath }));

            Webpage.Context = HttpContext;
            Webpage.VirtualPath = TemplatePath;
            Webpage.InstancePurpose = InstancePurposes;
            InitHelpers(Webpage);
        }

        public override void CustomizeData() 
            => Webpage?.CustomizeData();

        public override void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo, DateTime beginDate) 
            => Webpage?.CustomizeSearch(searchInfos, moduleInfo, beginDate);
    }
}