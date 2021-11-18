using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;


namespace ToSic.Sxc.Apps.Assets
{
    public class TemplateKey
    {
        // TODO: STV pls move all these to the definition objects
        //public const string CsHtml = "cshtml-hybrid";
        public const string CsHtmlCode = "cshtml-code-hybrid";
        public const string CsCode = "cs-code-hybrid";
        public const string Api = "cs-api-hybrid";
        public const string Token = "html-token";
        public const string CustomSearchCsCode = "cs-code-custom-search-dnn";
    }

    public class Extension
    {
        // TODO: STV pls move all these to the definition objects
        public const string Html = ".html";
        public const string Cshtml = ".cshtml";
        public const string CodeCshtml = ".code.cshtml";
        public const string Cs = ".cs";
    }

    public class Purpose
    {
        // TODO: Once the UIs are updated, we'll kill the 'auto' case
        public const string Auto = "auto";
        //public const string Razor = "razor";
        //public const string Token = "token";
        //public const string Api = "api";
        //public const string Search = "search";
    }

    [PrivateApi]
    public partial class AssetTemplates : HasLog<AssetTemplates> // , IAssetTemplates
    {
        #region Constants

        private const string ForTemplate = "Template";
        private const string ForApi = "Api";
        private const string ForDocs = "Documentation";
        public const string ForCode = "Code";
        public const string ForSearch = "Search";


        #endregion


        public AssetTemplates() : base("SxcAss.Templt")
        {
        }

        public List<TemplateInfo> GetTemplates()
        {
            var callLog = Log.Call<List<TemplateInfo>>(nameof(GetTemplates));

            if (_templates == null) _templates = new List<TemplateInfo>
            {
                RazorHybrid,
                DnnCsCode,
                CsHybrid,
                ApiHybrid,
                Token,
                DnnSearch,
                Markdown,
                EmptyTextFile,
            };

            return callLog(null, _templates);
        }

        private static List<TemplateInfo> _templates;

        // TODO: @STV This should probably become obsolete once you change the objects above
        public string GetTemplate(string key) => GetTemplateInfo(key).Body;

        // TODO: @STV This should become obsolete once you change the objects above
        private TemplateInfo GetTemplateInfo(string key) => GetTemplates()
            .FirstOrDefault(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));




        public const string CsApiTemplateControllerName = "PleaseRenameController";

        // copied from the razor tutorial

        //internal static string DefaultWebApiBody =
//            @"#if NETCOREAPP // Oqtane
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//#else // DNN
//using System.Web.Http;
//using DotNetNuke.Web.Api;
//#endif

//[AllowAnonymous]      // define that all commands can be accessed without a login
//// Inherit from Custom.Hybrid.Api12 to get features like App, CmsContext, Data etc.
//// see https://docs.2sxc.org/web-api/custom/index.html
//public class " + CsApiTemplateControllerName + @" : Custom.Hybrid.Api12
//{
//    [HttpGet]        // [HttpGet] says we're listening to GET requests
//    public string Hello()
//    {
//        return ""Hello from the controller with ValidateAntiForgeryToken in /api"";
//    }

//    [HttpPost]        // [HttpPost] says we're listening to POST requests
//    [ValidateAntiForgeryToken] // protects from the users not on your site (CSRF protection)
//    public int Sum([FromBody] dynamic bodyJson) // post body { ""a"": 2, ""b"": 3 }
//    {
//        int a = bodyJson.a;
//        int b = bodyJson.b;
//        return a + b;
//    }
//}
//";



//        internal static string Readme = @"# Readme

//A standard README file.
//";

//        internal static string Txt = @"Simple text file.
//";
    }
}
