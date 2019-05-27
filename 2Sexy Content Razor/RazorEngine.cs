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
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Razor;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.Search;

// ReSharper disable once CheckNamespace
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
            try
            {
                Webpage.ExecutePageHierarchy(new WebPageContext(HttpContext, Webpage, null), writer, Webpage);
            }
            catch (Exception maybeIEntityCast)
            {
                ProvideSpecialErrorOnIEntityIssues(maybeIEntityCast);
                throw;
            }
        }

        private const string IentityErrDetection = "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav'";
        private const string IentityErrorMessage =
            "Error in your razor template. " +
            "You are seeing this because 2sxc 9.3 has a breaking change on ToSic.Eav.IEntity. " +
            "It's easy to fix - please read " +
            "https://2sxc.org/en/blog/post/fixing-the-breaking-change-on-tosic-eav-ientity-in-2sxc-9-3 " +
            ". What follows is the internal error: ";

        private static void ProvideSpecialErrorOnIEntityIssues(Exception maybeIEntityCast)
        {
            if (maybeIEntityCast is HttpCompileException || maybeIEntityCast is InvalidCastException)
                if (maybeIEntityCast.Message.IndexOf(IentityErrDetection, StringComparison.Ordinal) > 0)
                    throw new Exception(IentityErrorMessage, maybeIEntityCast);
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
            try
            {
                var compiledType = BuildManager.GetCompiledType(TemplatePath);
                object objectValue = null;
                if (compiledType != null)
                    objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
                return objectValue;
            }
            catch (Exception ex)
            {
                ProvideSpecialErrorOnIEntityIssues(ex);
                throw;
            }
        }

        private void InitHelpers(SexyContentWebPage webPage)
        {
            webPage.Html = new HtmlHelper();
            // Deprecated 2019-05-27 2dm - I'm very sure this isn't used anywhere or by anyone.
            // reactivate if it turns out to be used, otherwise delete ca. EOY 2019
            //webPage.Url = new UrlHelper(InstInfo);
            webPage.Sexy = Sexy;
            webPage.DnnAppAndDataHelpers = new DnnAppAndDataHelpers(Sexy);

        }

        private void InitWebpage()
        {
            if (string.IsNullOrEmpty(TemplatePath)) return;

            var objectValue = RuntimeHelpers.GetObjectValue(CreateWebPageInstance());
            // ReSharper disable once JoinNullCheckWithUsage
            if (objectValue == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The webpage found at '{0}' was not created.", TemplatePath));

            Webpage = objectValue as SexyContentWebPage;

            if ((Webpage == null))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The webpage at '{0}' must derive from SexyContentWebPage.", TemplatePath));

            Webpage.Context = HttpContext;
            Webpage.VirtualPath = TemplatePath;
            Webpage.InstancePurpose = InstancePurposes;
            InitHelpers(Webpage);
        }

        public override void CustomizeData() 
            => Webpage?.CustomizeData();

        public override void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, IInstanceInfo moduleInfo, DateTime beginDate) 
            => Webpage?.CustomizeSearch(searchInfos, ((EnvironmentInstance<ModuleInfo>)moduleInfo).Original /*moduleInfo*/, beginDate);
    }
}