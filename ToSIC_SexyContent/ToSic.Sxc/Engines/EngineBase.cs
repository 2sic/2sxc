using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent;
using ToSic.SexyContent.Search;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Interfaces;
using ToSic.Sxc.Search;
using IApp = ToSic.Sxc.Apps.IApp;
using IDataSource = ToSic.Eav.DataSources.IDataSource;

namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The foundation for engines - must be inherited by other engines
    /// </summary>
    [PublicApi]
    public abstract class EngineBase : HasLog, IEngine
    {
        [PrivateApi] protected IView Template;
        [PrivateApi] protected string TemplatePath;
        [PrivateApi] protected IApp App;
        [PrivateApi] protected IContainer InstInfo;
        [PrivateApi] protected IDataSource DataSource;
        [PrivateApi] protected Purpose Purpose;
        [PrivateApi] protected Blocks.ICmsBlock Sexy;

        [PrivateApi]
        public RenderStatusType PreRenderStatus { get; internal set; }

        /// <summary>
        /// Empty constructor, so it can be used in dependency injection
        /// </summary>
        protected EngineBase() : base("Sxc.EngBas")
        { }

        /// <inheritdoc />
        public void Init(IView view, IApp app, IContainer envInstance, IDataSource dataSource, Purpose purpose, Blocks.ICmsBlock cmsBlock, ILog parentLog)
        {
            var templatePath = VirtualPathUtility.Combine(SexyContent.Internal.TemplateHelpers.GetTemplatePathRoot(view.Location, app) + "/", view.Path);

            Log.LinkTo(parentLog);

            // Throw Exception if Template does not exist
            if (!File.Exists(HostingEnvironment.MapPath(templatePath)))
                // todo: change to some kind of "rendering exception"
                throw new SexyContentException("The template file '" + templatePath + "' does not exist.");

            Template = view;
            TemplatePath = templatePath;
            App = app;
            InstInfo = envInstance;
            DataSource = dataSource;
            Purpose = purpose;
            Sexy = cmsBlock;

            // check common errors
            CheckExpectedTemplateErrors();

            // check access permissions - before initializing or running data-code in the template
            CheckTemplatePermissions(cmsBlock.Block.Tenant);

            // Run engine-internal init stuff
            Init();

            // call engine internal feature to optionally change what data is actually used or prepared for search...
            CustomizeData();

            // check if rendering is possible, or throw exceptions...
            CheckExpectedNoRenderConditions();

            if(PreRenderStatus == RenderStatusType.Unknown)
                PreRenderStatus = RenderStatusType.Ok;
        }

        [PrivateApi]
        protected abstract string RenderTemplate();

        [PrivateApi]
        protected virtual void Init() {}

        /// <inheritdoc />
        public virtual void CustomizeData() {}

        /// <inheritdoc />
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
            DateTime beginDate)
        {
        }

        /// <inheritdoc />
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

        [PrivateApi]
        internal string AlternateRendering;

        private void CheckExpectedNoRenderConditions()
        {
            if (Template.ContentType != "" && Template.ContentItem == null &&
                Sexy.Block.Configuration.Content.All(e => e == null))
            {
                PreRenderStatus = RenderStatusType.MissingData;

                AlternateRendering = ToolbarForEmptyTemplate;
            }
        }

        // todo: refactor - this should go somewhere, I just don't know where :)
        [PrivateApi]
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