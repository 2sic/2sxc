using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Search;
using ToSic.Sxc.Web;
using IApp = ToSic.Sxc.Apps.IApp;
using IDataSource = ToSic.Eav.DataSources.IDataSource;

namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The foundation for engines - must be inherited by other engines
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public abstract class EngineBase : HasLog, IEngine
    {
        protected readonly EngineBaseDependencies Helpers;
        [PrivateApi] protected IView Template;
        [PrivateApi] protected string TemplatePath;
        [PrivateApi] protected IApp App;
        [PrivateApi] protected IDataSource DataSource;
        [PrivateApi] protected Purpose Purpose;
        [PrivateApi] protected IBlock Block;

        [PrivateApi]
        public RenderStatusType PreRenderStatus { get; internal set; }

        [PrivateApi] public bool CompatibilityAutoLoadJQueryAndRVT { get; protected set; } = true;

        #region Constructor and DI

        /// <summary>
        /// Empty constructor, so it can be used in dependency injection
        /// </summary>
        protected EngineBase(EngineBaseDependencies helpers) : base("Sxc.EngBas")
        {
            Helpers = helpers;
            helpers.BlockResourceExtractor.Init(Log);
        }
        
        #endregion

        
        /// <inheritdoc />
        public void Init(IBlock block, Purpose purpose, ILog parentLog)
        {
            Block = block;
            var view = Block.View;
            Log.LinkTo(parentLog);

            var appPathRootInInstallation = Helpers.TemplateHelpers.Init(Block.App, Log).AppPathRoot(view.IsShared, PathTypes.PhysRelative);
            var subPath = view.Path;
            var polymorphInfo = TryToFindPolymorphPath(appPathRootInInstallation, view, subPath);
            var templatePath = polymorphInfo ?? Path.Combine(appPathRootInInstallation, subPath).ToAbsolutePathForwardSlash();

            // Throw Exception if Template does not exist
            if (!File.Exists(Helpers.ServerPaths.FullAppPath(templatePath)))
                // todo: change to some kind of "rendering exception"
                throw new SexyContentException("The template file '" + templatePath + "' does not exist.");

            Template = view;
            TemplatePath = templatePath;
            App = Block.App;
            DataSource = Block.Data;
            Purpose = purpose;

            // check common errors
            CheckExpectedTemplateErrors();

            // check access permissions - before initializing or running data-code in the template
            CheckTemplatePermissions(Block.Context.User);

            // Run engine-internal init stuff
            Init();
        }

        private string TryToFindPolymorphPath(string root, IView view, string subPath)
        {
            var wrapLog = Log.Call<string>($"{root}, {subPath}");
            view.EditionPath = subPath.ToAbsolutePathForwardSlash();
            var polymorph = Helpers.Polymorphism.Init(Block.App.Data.List, Log);
            var edition = polymorph.Edition();
            if (edition == null) return wrapLog("no edition detected", null);
            Log.Add($"edition {edition} detected");

            var testPath = Path.Combine(root, edition, subPath).ToAbsolutePathForwardSlash();
            if (File.Exists(Helpers.ServerPaths.FullAppPath(testPath)))
            {
                view.Edition = edition;
                view.EditionPath = Path.Combine(edition, subPath).ToAbsolutePathForwardSlash();
                return wrapLog($"edition {edition}", testPath);
            }

            Log.Add("tried inserting path, will check if sub-path");
            var firstSlash = subPath.IndexOf('/');
            if (firstSlash == -1) return wrapLog($"edition {edition} not found", null);

            subPath = subPath.Substring(firstSlash + 1);
            testPath = Path.Combine(root, edition, subPath).ToAbsolutePathForwardSlash();
            if (File.Exists(Helpers.ServerPaths.FullAppPath(testPath)))
            {
                view.Edition = edition;
                view.EditionPath = Path.Combine(edition, subPath).ToAbsolutePathForwardSlash();
                return wrapLog($"edition {edition} up one path", testPath);
            }

            return wrapLog($"edition {edition} not found", null);
        }

        [PrivateApi]
        protected abstract string RenderTemplate();

        [PrivateApi]
        protected virtual void Init() {}

        /// <inheritdoc />
        public virtual void CustomizeData() {}

        /// <inheritdoc />
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo,
            DateTime beginDate)
        {
        }

        /// <inheritdoc />
        public string Render()
        {
            // call engine internal feature to optionally change what data is actually used or prepared for search...
            CustomizeData();

            // check if rendering is possible, or throw exceptions...
            CheckExpectedNoRenderConditions();

            if(PreRenderStatus == RenderStatusType.Unknown)
                PreRenderStatus = RenderStatusType.Ok;


            if (PreRenderStatus != RenderStatusType.Ok)
                return AlternateRendering;

            var renderedTemplate = RenderTemplate();
            var depMan = Helpers.BlockResourceExtractor;
            var result = depMan.Process(renderedTemplate);
            ActivateJsApi = result.Item2;
            return result.Item1;
        }

        [PrivateApi] public bool ActivateJsApi { get; private set; }

        /// <inheritdoc/>
        [PrivateApi] public List<ClientAssetInfo> Assets => Helpers.BlockResourceExtractor.Assets;


        private void CheckExpectedTemplateErrors()
        {
            if (Template == null)
                throw new RenderingException("Template Configuration Missing");

            if (Template.ContentType != "" && Helpers.AppStatesLazy.Value.Get(App).GetContentType(Template.ContentType) == null)
                throw new RenderingException("The contents of this module cannot be displayed because I couldn't find the assigned content-type.");
        }

        [PrivateApi]
        internal string AlternateRendering;

        private void CheckExpectedNoRenderConditions()
        {
            if (Template.ContentType != "" && Template.ContentItem == null &&
                Block.Configuration.Content.All(e => e == null))
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

        private void CheckTemplatePermissions(IUser user)
        {
            // do security check IF security exists
            // should probably happen somewhere else - so it doesn't throw errors when not even rendering...
            var templatePermissions = Helpers.AppPermCheckLazy.Value
                .ForItem(Block.Context, App, Template.Entity, Log);

            // Views only use permissions to prevent access, so only check if there are any configured permissions
            if (user.IsAdmin || !templatePermissions.HasPermissions)
                return;

            if (!templatePermissions.UserMay(GrantSets.ReadSomething))
                throw new RenderingException(new UnauthorizedAccessException(
                    "This view is not accessible for the current user. To give access, change permissions in the view settings. See http://2sxc.org/help?tag=view-permissions"));
        }

    }
}