using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using Newtonsoft.Json;
using ToSic.SexyContent.Search;
using ToSic.SexyContent.Security;
using IDataSource = ToSic.Eav.DataSources.IDataSource;

namespace ToSic.SexyContent.Engines
{
    public abstract class EngineBase : IEngine
    {
        protected Template Template;
        protected string TemplatePath;
        protected App App;
        protected ModuleInfo ModuleInfo;
        protected IDataSource DataSource;
        protected InstancePurposes InstancePurposes;
        protected SxcInstance Sexy;

        public RenderStatusType PreRenderStatus { get; internal set; }


        public void Init(Template template, App app, ModuleInfo hostingModule, IDataSource dataSource, InstancePurposes instancePurposes, SxcInstance sexy)
        {
            var templatePath = VirtualPathUtility.Combine(Internal.TemplateManager.GetTemplatePathRoot(template.Location, app) + "/", template.Path);

            // Throw Exception if Template does not exist
            if (!File.Exists(HostingEnvironment.MapPath(templatePath)))
                // todo: rendering exception
                throw new SexyContentException("The template file '" + templatePath + "' does not exist.");

            Template = template;
            TemplatePath = templatePath;
            App = app;
            ModuleInfo = hostingModule;
            DataSource = dataSource;
            InstancePurposes = instancePurposes;
            Sexy = sexy;

            // check common errors
            CheckExpectedTemplateErrors();

            // check access permissions - before initializing or running data-code in the template
            CheckTemplatePermissions(sexy.AppPortalSettings);

            // Run engine-internal init stuff
            Init();

            // call engine internal feature to optionally change what data is actually used or prepared for search...
            CustomizeData();

            // check if rendering is possible, or throw exceptions...
            CheckExpectedNoRenderConditions();

            if(PreRenderStatus == RenderStatusType.Unknown)
                PreRenderStatus = RenderStatusType.Ok;
        }

        protected abstract string RenderTemplate();

        protected virtual void Init() {}

        public virtual void CustomizeData() {}

        public virtual void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo,
            DateTime beginDate)
        {
        }

        /// <summary>
        /// Renders the given elements with Razor or TokenReplace and returns the string representation.
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            if (PreRenderStatus != RenderStatusType.Ok)
                return AlternateRendering;

            var renderedTemplate = RenderTemplate();
            return HandleClientDependencyInjection(renderedTemplate);
        }

        // pre-compile all reg-ex, to improve performance
        private const string ClientDependencyRegex =
            "\\sdata-enableoptimizations=('|\")(?<Priority>true|[0-9]+)?(?::)?(?<Position>bottom|head|body)?('|\")(>|\\s)";

        private const string ScriptRegExFormula = "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))";
        private const string StyleRegExFormula = "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/>)";
        private const string StyleRelFormula = "('|\"|\\s)rel=('|\")stylesheet('|\")";

        private static readonly Regex ScriptDetection = new Regex(ScriptRegExFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex StyleDetection = new Regex(StyleRegExFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly  Regex StyleRelDetect = new Regex(StyleRelFormula, RegexOptions.IgnoreCase);
        private static readonly Regex OptimizeDetection = new Regex(ClientDependencyRegex, RegexOptions.IgnoreCase);

        private string HandleClientDependencyInjection(string renderedTemplate)
        {
            if (HttpContext.Current == null || HttpContext.Current.CurrentHandler == null || !(HttpContext.Current.CurrentHandler is Page))
                return renderedTemplate;

            var page = (HttpContext.Current.CurrentHandler as Page);

            #region  Handle Client Dependency injection

            #region Scripts

            var scriptMatches = ScriptDetection.Matches(renderedTemplate);// Regex.Matches(renderedTemplate, scriptRegExFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var scriptMatchesToRemove = new List<Match>();

            foreach (Match match in scriptMatches)
            {
                var optMatch = OptimizeDetection.Match(match.Value);// Regex.Match(match.Value, ClientDependencyRegex, RegexOptions.IgnoreCase);
                if (!optMatch.Success)
                    continue;

                //var defPos = "body";
                //var position = (optMatch.Groups["Position"]?.Value ?? defPos).ToLower(); // default JS provider, see https://github.com/dnnsoftware/Dnn.Platform/blob/a142594a0c18a589cb5fb913a022eebe34549a8f/DNN%20Platform/DotNetNuke.Web.Client/ClientResourceManager.cs#L54
                var providerName = GetProviderName(optMatch, "body");

                var prio = GetPriority(optMatch, (int) FileOrder.Js.DefaultPriority);

                if (prio <= 0) continue;    // don't register/remove if not within specs

                ClientResourceManager.RegisterScript(page, match.Groups["Src"].Value, prio, providerName);

                // Remove the script tag from the Rendered Template
                scriptMatchesToRemove.Add(match);
            }

            scriptMatchesToRemove.Reverse();
            scriptMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));
            #endregion

            #region Styles

            var styleMatches = StyleDetection.Matches(renderedTemplate);// Regex.Matches(renderedTemplate, "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var styleMatchesToRemove = new List<Match>();

            foreach (Match match in styleMatches)
            {
                var optMatch = OptimizeDetection.Match(match.Value);// Regex.Match(match.Value, ClientDependencyRegex, RegexOptions.IgnoreCase);
                if (!optMatch.Success)
                    continue;

                // Break If the Rel attribute is not stylesheet
                if (!StyleRelDetect.IsMatch(match.Value))// !Regex.IsMatch(match.Value, "('|\"|\\s)rel=('|\")stylesheet('|\")", RegexOptions.IgnoreCase))
                    continue;

                // var position = (optMatch.Groups["Position"]?.Value ?? "head").ToLower(); // default CSS provider, see https://github.com/dnnsoftware/Dnn.Platform/blob/a142594a0c18a589cb5fb913a022eebe34549a8f/DNN%20Platform/DotNetNuke.Web.Client/ClientResourceManager.cs#L54
                var providerName = GetProviderName(optMatch, "head");

                var prio = GetPriority(optMatch, (int) FileOrder.Css.DefaultPriority);

                //var priority = (clientDependencyMatch.Groups["Priority"]?.Value ?? "true").ToLower();
                //var prio = (priority == "true" || priority == "")
                //    ? (int) FileOrder.Css.DefaultPriority
                //    : int.Parse(priority);

                if (prio <= 0) continue;    // don't register/remove if not within specs

                ClientResourceManager.RegisterStyleSheet(page, match.Groups["Src"].Value, prio, providerName);

                // Remove the script tag from the Rendered Template
                styleMatchesToRemove.Add(match);
            }

            styleMatchesToRemove.Reverse();
            styleMatchesToRemove.ForEach(p => renderedTemplate = renderedTemplate.Remove(p.Index, p.Length));

            #endregion
            #endregion

            return renderedTemplate;
        }

        private int GetPriority(Match optMatch, int defValue)
        {
            var priority = (optMatch.Groups["Priority"]?.Value ?? "true").ToLower();
            var prio = (priority == "true" || priority == "")
                ? defValue
                : int.Parse(priority);
            return prio;
        }

        private string GetProviderName(Match optMatch, string defaultPosition)
        {
            var position = (optMatch.Groups["Position"]?.Value ?? defaultPosition).ToLower();

            switch (position)
            {
                case "body":
                    return DnnBodyProvider.DefaultName;
                case "head":
                    return DnnPageHeaderProvider.DefaultName;
                case "bottom":
                    return DnnFormBottomProvider.DefaultName;
            }
            return "";
        }

        // todo: i18n
        private void CheckExpectedTemplateErrors()
        {
            //#region Check if everything has values and return if not

            if (Template == null)
                //{
                throw new RenderingException("Template Configuration Missing");

            if (Template.ContentTypeStaticName != "" &&
                Eav.DataSource.GetCache(App.ZoneId, App.AppId).GetContentType(Template.ContentTypeStaticName) == null)
                throw new RenderingException("The contents of this module cannot be displayed because I couldn't find the assigned content-type.");

        }

        internal string AlternateRendering;

        private void CheckExpectedNoRenderConditions()
        {
            if (Template.ContentTypeStaticName != "" && Template.ContentDemoEntity == null &&
                Sexy.ContentGroup.Content.All(e => e == null))
            {
                PreRenderStatus = RenderStatusType.MissingData;

                AlternateRendering = ToolbarForEmptyTemplate;
                //var toolbar = ToolbarForEmptyTemplate;
                //throw new RenderingException(RenderStatusType.MissingData, "No demo item found ");// /*LocalizeString("NoDemoItem.Text")*/+ " " + toolbar);
            }
        }

        // todo: refactor - this should go somewhere, I just don't know where :)
        internal static string ToolbarForEmptyTemplate
        {
            get
            {
                var toolbar = "<ul class='sc-menu' data-toolbar='" +
                              JsonConvert.SerializeObject(new {sortOrder = 0, useModuleList = true, action = "edit"}) +
                              "'></ul>";
                var wrapped =
                    "<div class='dnnFormMessage dnnFormInfo'>No demo item exists for the selected template. " +
                    toolbar + "</div>";
                return wrapped;
            }
        }

        private void CheckTemplatePermissions(PortalSettings portalSettings)
        {
            // 2015-05-19 2dm: new: do security check if security exists
            // should probably happen somewhere else - so it doesn't throw errors when not even rendering...
            var permissionsOnThisTemplate = new PermissionController(App.ZoneId, App.AppId, Template.Guid, ModuleInfo);

            // Views only use permissions to prevent access, so only check if there are any configured permissions
            if (!portalSettings.UserInfo.IsInRole(portalSettings.AdministratorRoleName) && permissionsOnThisTemplate.PermissionList.Any())
                if (!permissionsOnThisTemplate.UserMay(PermissionGrant.Read))
                    throw new RenderingException(new UnauthorizedAccessException(
                        "This view is not accessible for the current user. To give access, change permissions in the view settings. See http://2sxc.org/help?tag=view-permissions"));
        }

    }
}