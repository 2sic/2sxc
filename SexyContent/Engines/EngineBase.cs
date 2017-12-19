using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using Newtonsoft.Json;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Search;
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

        protected Log Log { get; set; }

        public void Init(Template template, App app, ModuleInfo hostingModule, IDataSource dataSource, InstancePurposes instancePurposes, SxcInstance sxcInstance, Log parentLog)
        {
            var templatePath = VirtualPathUtility.Combine(Internal.TemplateHelpers.GetTemplatePathRoot(template.Location, app) + "/", template.Path);

            Log = new Log("Htm.RendEn", parentLog);

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
            Sexy = sxcInstance;

            // check common errors
            CheckExpectedTemplateErrors();

            // check access permissions - before initializing or running data-code in the template
            CheckTemplatePermissions(sxcInstance.AppPortalSettings);

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

        /// <inheritdoc />
        /// <summary>
        /// Renders the given elements with Razor or TokenReplace and returns the string representation.
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            if (PreRenderStatus != RenderStatusType.Ok)
                return AlternateRendering;

            var renderedTemplate = RenderTemplate();
            return new Environment.Dnn7.ClientDependencyManager().Process(renderedTemplate);
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
            var permissionsOnThisTemplate = new DnnPermissionController(/*App.ZoneId, App.AppId,*/ Template.Entity /*.Guid*/, Log, ModuleInfo);

            // Views only use permissions to prevent access, so only check if there are any configured permissions
            if (!portalSettings.UserInfo.IsInRole(portalSettings.AdministratorRoleName) && permissionsOnThisTemplate.PermissionList.Any())
                if (!permissionsOnThisTemplate.UserMay(PermissionGrant.Read))
                    throw new RenderingException(new UnauthorizedAccessException(
                        "This view is not accessible for the current user. To give access, change permissions in the view settings. See http://2sxc.org/help?tag=view-permissions"));
        }

    }
}