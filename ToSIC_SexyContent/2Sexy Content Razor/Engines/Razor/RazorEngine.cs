using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Compilation;
using System.Web.WebPages;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.SexyContent.Search;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Search;
using ToSic.Sxc.Web;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The razor engine, which compiles / runs engine templates
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    [EngineDefinition(Name = "Razor")]
    // ReSharper disable once UnusedMember.Global
    public class RazorEngine : EngineBase
    {
        [PrivateApi]
        protected RazorComponentBase  Webpage { get; set; }

        /// <inheritdoc />
        [PrivateApi]
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

        [PrivateApi]
        protected HttpContextBase HttpContext 
            => System.Web.HttpContext.Current == null ? null : new HttpContextWrapper(System.Web.HttpContext.Current);

        // 2019-11-28 2dm disabled, don't think it's used
        //[PrivateApi("not sure if this is actually used anywhere?")]
        //public Type RequestedModelType()
        //{
        //    if (Webpage != null)
        //    {
        //        var webpageType = Webpage.GetType();
        //        if (webpageType.BaseType.IsGenericType)
        //            return webpageType.BaseType.GetGenericArguments()[0];
        //    }
        //    return null;
        //}

        [PrivateApi]
        public void Render(TextWriter writer)
        {
            Log.Add("will render into TextWriter");
            try
            {
                Webpage.ExecutePageHierarchy(new WebPageContext(HttpContext, Webpage, null), writer, Webpage);
            }
            catch (Exception maybeIEntityCast)
            {
                Code.ErrorHelp.AddHelpIfKnownError(maybeIEntityCast);
                throw;
            }
        }

        /// <inheritdoc/>
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
                Code.ErrorHelp.AddHelpIfKnownError(ex);
                throw;
            }
        }

        private void InitWebpage()
        {
            if (string.IsNullOrEmpty(TemplatePath)) return;

            var objectValue = RuntimeHelpers.GetObjectValue(CreateWebPageInstance());
            // ReSharper disable once JoinNullCheckWithUsage
            if (objectValue == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The webpage found at '{0}' was not created.", TemplatePath));

            Webpage = objectValue as RazorComponentBase;

            if (Webpage == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The webpage at '{0}' must derive from SexyContentWebPage.", TemplatePath));

            Webpage.Context = HttpContext;
            Webpage.VirtualPath = TemplatePath;
            var compatibility = 9;
            if (Webpage is RazorComponent rzrPage)
            {
                rzrPage.Purpose = Purpose;
                compatibility = 10;
            }
#pragma warning disable 618
            if(Webpage is SexyContentWebPage oldPage)
                oldPage.InstancePurpose = (InstancePurposes) Purpose;
#pragma warning restore 618
            InitHelpers(Webpage, compatibility);
        }

        private void InitHelpers(RazorComponentBase webPage, int compatibility)
        {
            webPage.Html = new Razor.HtmlHelper();
            webPage.DynCode = new DnnDynamicCode(BlockBuilder, compatibility, Log);

            #region New in 10.25 - ensure jquery is not included by default

            if (compatibility == 10) CompatibilityAutoLoadJQueryAndRVT = false;

            #endregion

        }


        /// <inheritdoc />
        public override void CustomizeData() 
            => (Webpage as IRazorComponent)?.CustomizeData();

        /// <inheritdoc />
        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo, DateTime beginDate)
        {
            if (Webpage == null || searchInfos == null || searchInfos.Count <= 0) return;

            // call new signature
            (Webpage as RazorComponent)?.CustomizeSearch(searchInfos, moduleInfo, beginDate);

            // also call old signature
            var oldSignature = searchInfos.ToDictionary(si => si.Key, si => si.Value.Cast<ISearchInfo>().ToList());
            (Webpage as SexyContentWebPage)?.CustomizeSearch(oldSignature, ((Container<ModuleInfo>) moduleInfo).UnwrappedContents, beginDate);
        }
    }
}