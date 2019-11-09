using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent;
using ToSic.SexyContent.Search;
using ToSic.Sxc.Interfaces;
using ToSic.Sxc.Views;
using App = ToSic.SexyContent.App;
using IDataSource = ToSic.Eav.DataSources.IDataSource;

namespace ToSic.Sxc.Engines
{
    public abstract class EngineBase : HasLog, IEngine
    {
        protected IView Template;
        protected string TemplatePath;
        protected App App;
        protected IInstanceInfo InstInfo;
        protected IDataSource DataSource;
        protected InstancePurposes InstancePurposes;
        protected SxcInstance Sexy;

        public RenderStatusType PreRenderStatus { get; internal set; }

        //protected ILog Log { get; set; }

        protected EngineBase() : base("Sxc.EngBas")
        { }

        public void Init(IView template, App app, IInstanceInfo hostingModule, IDataSource dataSource, InstancePurposes instancePurposes, SxcInstance sxcInstance, ILog parentLog)
        {
            var templatePath = VirtualPathUtility.Combine(SexyContent.Internal.TemplateHelpers.GetTemplatePathRoot(template.Location, app) + "/", template.Path);

            Log.LinkTo(parentLog);
            //Log = new Log("Htm.RendEn", parentLog);

            // Throw Exception if Template does not exist
            if (!File.Exists(HostingEnvironment.MapPath(templatePath)))
                // todo: change to some kind of "rendering exception"
                throw new SexyContentException("The template file '" + templatePath + "' does not exist.");

            Template = template;
            TemplatePath = templatePath;
            App = app;
            InstInfo = hostingModule;
            DataSource = dataSource;
            InstancePurposes = instancePurposes;
            Sexy = sxcInstance;

            // check common errors
            CheckExpectedTemplateErrors();

            // check access permissions - before initializing or running data-code in the template
            CheckTemplatePermissions(sxcInstance.Tenant);

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

        public virtual void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, IInstanceInfo moduleInfo,
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
            var depMan = Factory.Resolve<IClientDependencyManager>();
            return depMan.Process(renderedTemplate);
        }


        private void CheckExpectedTemplateErrors()
        {
            if (Template == null)
                throw new RenderingException("Template Configuration Missing");

            if (Template.ContentType != "" &&
                Eav.DataSource.GetCache(App.ZoneId, App.AppId).GetContentType(Template.ContentType) == null)
                throw new RenderingException("The contents of this module cannot be displayed because I couldn't find the assigned content-type.");

        }

        internal string AlternateRendering;

        private void CheckExpectedNoRenderConditions()
        {
            if (Template.ContentType != "" && Template.ContentItem == null &&
                Sexy.ContentGroup.Content.All(e => e == null))
            {
                PreRenderStatus = RenderStatusType.MissingData;

                AlternateRendering = ToolbarForEmptyTemplate;
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

        private void CheckTemplatePermissions(ITenant tenant)
        {
            // do security check IF security exists
            // should probably happen somewhere else - so it doesn't throw errors when not even rendering...
            var templatePermissions = Factory.Resolve<IEnvironmentFactory>()
                .ItemPermissions(App, Template.Entity, Log, InstInfo);

            // Views only use permissions to prevent access, so only check if there are any configured permissions
            if (tenant.RefactorUserIsAdmin || !templatePermissions.HasPermissions)
                return;

            if (!templatePermissions.UserMay(GrantSets.ReadSomething))
                throw new RenderingException(new UnauthorizedAccessException(
                    "This view is not accessible for the current user. To give access, change permissions in the view settings. See http://2sxc.org/help?tag=view-permissions"));
        }

    }
}