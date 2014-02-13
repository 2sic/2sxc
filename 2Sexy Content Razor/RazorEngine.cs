using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Compilation;
using System.Web.WebPages;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Modules;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.Razor;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.Engines
{
    public class RazorEngine : IEngine
    {
        public RazorEngine()
        {
        }

        protected string RazorScriptFile { get; set; }
        protected ModuleInstanceContext ModuleContext { get; set; }
        protected string LocalResourceFile { get; set; }
        // <2sic>
        protected SexyContentWebPage Webpage { get; set; }
        protected dynamic Content { get; set; }
        protected dynamic Presentation { get; set; }
        protected dynamic ListContent { get; set; }
        protected dynamic ListPresentation { get; set; }
        protected App App { get; set; }
        protected List<Element> List { get; set; }
        protected IDataSource DataSource { get; set; }
        // </2sic>

        protected HttpContextBase HttpContext
        {
            get { return new HttpContextWrapper(System.Web.HttpContext.Current); }
        }

        public Type RequestedModelType()
        {
            if (Webpage != null)
            {
                var webpageType = Webpage.GetType();
                if (webpageType.BaseType.IsGenericType)
                {
                    return webpageType.BaseType.GetGenericArguments()[0];
                }
            }
            return null;
        }

        // <2sic removed: Render<T>(TextWriter writer, T model)>
        // </2sic>

        public void Render(TextWriter writer)
        {
            Webpage.ExecutePageHierarchy(new WebPageContext(HttpContext, Webpage, null), writer, Webpage);
        }

        /// <summary>
        /// Renders the template
        /// </summary>
        /// <param name="TemplatePath"></param>
        /// <param name="TemplateText"></param>
        /// <param name="Elements"></param>
        /// <param name="ListElement"></param>
        /// <param name="HostingModule"></param>
        /// <param name="LocalResourcesPath"></param>
        /// <returns></returns>
        public string Render(Template template, App app, List<Element> Elements, Element ListElement, ModuleInstanceContext HostingModule, string LocalResourcesPath, IDataSource DataSource) 
        {
            SexyContent Sexy = new SexyContent();
            ModuleContext = HostingModule;
            LocalResourceFile = LocalResourcesPath;
            RazorScriptFile = template.GetTemplatePath();

            Content = Elements.First().Content;
            Presentation = Elements.First().Presentation;
            if (ListElement != null)
            {
                ListContent = ListElement.Content;
                ListPresentation = ListElement.Presentation;
            }
            List = Elements;
            App = app;
            this.DataSource = DataSource;
            // </2sic>

            // Error if it's DNN 7 and WebPages v1 is used
            if (typeof(DotNetNuke.Common.Globals).Assembly.GetName().Version.Major >= 7)
            {
                if(typeof(System.Web.WebPages.WebPageBase).Assembly.GetName().Version.Major == 1)
                {
                    Exception e = new Exception("Error - wrong version of WebPages used, please follow this checklist to solve the problem: http://swisschecklist.com/en/titu2ili/2Sexy-Content-Create-WebPages-redirect-in-web.config-for-DNN-7");
                    throw e;
                }
            }

            try
            {
                InitWebpage();
            }
            // Catch web.config Error on DNNs upgraded to 7
            catch (System.Configuration.ConfigurationErrorsException Exc)
            {
                Exception e = new Exception("Configuration Error: Please follow this checklist to solve the problem: http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7", Exc);
                //Exceptions.LogException(e);
                throw e;
            }

            var writer = new System.IO.StringWriter();
            Render(writer);
            return writer.ToString();
        }

        private object CreateWebPageInstance()
        {
            var compiledType = BuildManager.GetCompiledType(RazorScriptFile);
            object objectValue = null;
            if (compiledType != null)
            {
                objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
            }
            return objectValue;
        }

        private void InitHelpers(SexyContentWebPage webPage)
        {
            webPage.Dnn = new DnnHelper(ModuleContext);
            webPage.Html = new HtmlHelper(ModuleContext, LocalResourceFile);
            webPage.Url = new UrlHelper(ModuleContext);
            // <2sic>
            webPage.Content = Content;
            webPage.Presentation = Presentation;
            webPage.ListContent = ListContent;
            webPage.ListPresentation = ListPresentation;
            webPage.List = List;
            webPage.App = App;
            webPage.Data = (IDataTarget)DataSource;
            // </2sic>
        }

        private void InitWebpage()
        {
            if (!string.IsNullOrEmpty(RazorScriptFile))
            {
                var objectValue = RuntimeHelpers.GetObjectValue(CreateWebPageInstance());
                if ((objectValue == null))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The webpage found at '{0}' was not created.", new object[] { RazorScriptFile }));
                }
                // <2sic>
                Webpage = objectValue as SexyContentWebPage;
                // </2sic>
                if ((Webpage == null))
                {
                    // <2sic> changed Exception Message (DotNetNukeWebPage to SexyContentWebPage)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The webpage at '{0}' must derive from SexyContentWebPage.", new object[] { RazorScriptFile }));
                    // </2sic>
                }
                Webpage.Context = HttpContext;
                Webpage.VirtualPath = VirtualPathUtility.GetDirectory(RazorScriptFile);
                InitHelpers(Webpage);
            }
        }
    }
}