using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.Serialization;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using IApp = ToSic.Sxc.Apps.IApp;
using IDataSource = ToSic.Eav.DataSource.IDataSource;

namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The foundation for engines - must be inherited by other engines
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public abstract partial class EngineBase : ServiceBase<EngineBase.MyServices>, IEngine
    {
        [PrivateApi] protected IView Template;
        [PrivateApi] protected string TemplatePath;
        [PrivateApi] protected IApp App;
        [PrivateApi] protected IDataSource DataSource;

        [PrivateApi] protected IBlock Block;

        [PrivateApi] public bool CompatibilityAutoLoadJQueryAndRvt { get; protected set; } = true;

        #region Constructor and DI

        public class MyServices : MyServicesBase
        {

            public MyServices(IServerPaths serverPaths,
                IBlockResourceExtractor blockResourceExtractor,
                LazySvc<AppPermissionCheck> appPermCheckLazy,
                Polymorphism.Polymorphism polymorphism,
                LazySvc<IAppStates> appStatesLazy
            )
            {
                ConnectServices(
                    Polymorphism = polymorphism,
                    AppStatesLazy = appStatesLazy,
                    ServerPaths = serverPaths,
                    BlockResourceExtractor = blockResourceExtractor,
                    AppPermCheckLazy = appPermCheckLazy
                );
            }

            internal readonly IServerPaths ServerPaths;
            internal readonly IBlockResourceExtractor BlockResourceExtractor;
            internal readonly LazySvc<AppPermissionCheck> AppPermCheckLazy;
            internal Polymorphism.Polymorphism Polymorphism { get; }
            internal LazySvc<IAppStates> AppStatesLazy { get; }
        }
        /// <summary>
        /// Empty constructor, so it can be used in dependency injection
        /// </summary>
        protected EngineBase(MyServices services) : base(services, $"{Constants.SxcLogName}.EngBas") { }

        #endregion

        public void Init(IBlock block) => Log.Do(() =>
        {
            Block = block;
            var view = Block.View;

            var appPathRootInInstallation = Block.App.PathSwitch(view.IsShared, PathTypes.PhysRelative);
            var subPath = view.Path;
            var polymorphPathOrNull = PolymorphTryToSwitchPath(appPathRootInInstallation, view, subPath);
            var templatePath = polymorphPathOrNull ??
                               Path.Combine(appPathRootInInstallation, subPath).ToAbsolutePathForwardSlash();

            // Throw Exception if Template does not exist
            if (!File.Exists(Services.ServerPaths.FullAppPath(templatePath)))
                // todo: change to some kind of "rendering exception"
                throw new SexyContentException("The template file '" + templatePath + "' does not exist.");

            Template = view;
            TemplatePath = templatePath;
            App = Block.App;
            DataSource = Block.Data;

            // check common errors
            CheckExpectedTemplateErrors();

            // check access permissions - before initializing or running data-code in the template
            CheckTemplatePermissions(Block.Context.User);

            // Run engine-internal init stuff
            Init();
        });

        private string PolymorphTryToSwitchPath(string root, IView view, string subPath) => Log.Func($"{root}, {subPath}", l =>
        {
            // Get initial path - here the file is already reliably stored
            view.EditionPath = subPath.ToAbsolutePathForwardSlash();
            subPath = view.EditionPath.TrimPrefixSlash();

            // Figure out the current edition - if none, stop here
            var polymorph = Services.Polymorphism.Init(Block.App.Data.List);
            // New 2023-03-20 - if the view comes with a preset edition, it's an ajax-preview which should be respected
            var edition = view.Edition.NullIfNoValue() ?? polymorph.Edition();
            if (edition == null)
                return (null, "no edition detected");
            l.A($"edition '{edition}' detected");

            // Case #1 where edition is between root and path
            // eg. subPath = "View.cshtml" and there is a "bs5/View.cshtml"
            var newPath = PolymorphTestPathAndSetIfFound(view, root, edition, subPath);
            if (newPath != null)
                return (newPath, $"found edition {edition}");

            // Case #2 where edition _replaces_ an edition in the current path
            // eg. subPath ="bs5/View.cshtml" and there is a "bs4/View.cshtml"
            l.A("tried inserting path, will check if sub-path");
            var pathWithoutFirstFolder = subPath.After("/");
            if (string.IsNullOrEmpty(pathWithoutFirstFolder))
                return (null, "view is not in subfolder, so no edition to replace, stopping now");
            newPath = PolymorphTestPathAndSetIfFound(view, root, edition, pathWithoutFirstFolder);
            if (newPath != null)
                return (newPath, $"edition {edition} up one path");

            return (null, $"edition {edition} not found");
        });

        private string PolymorphTestPathAndSetIfFound(IView view, string root, string edition, string subPath
        ) => Log.Func($"root: {root}; edition: {edition}; subPath: {subPath}", () =>
        {
            var fullPathForTest = Path.Combine(root, edition, subPath).ToAbsolutePathForwardSlash();
            if (!File.Exists(Services.ServerPaths.FullAppPath(fullPathForTest)))
                return (null, "not found");
            view.Edition = edition;
            view.EditionPath = Path.Combine(edition, subPath).ToAbsolutePathForwardSlash();
            return (fullPathForTest, $"edition {edition}");
        });

        [PrivateApi]
        protected abstract string RenderTemplate(object data);

        [PrivateApi]
        protected virtual void Init() {}

        /// <inheritdoc />
        public RenderEngineResult Render(object data)
        {
            var l = Log.Fn<RenderEngineResult>(timer: true);
            // call engine internal feature to optionally change what data is actually used or prepared for search...
#if NETFRAMEWORK
#pragma warning disable CS0618
            CustomizeData();
#pragma warning restore CS0618
#endif
            // check if rendering is possible, or throw exceptions...
            var (renderStatus, message, errorCode) = CheckExpectedNoRenderConditions();

            if (renderStatus != RenderStatusType.Ok)
                return l.Return(new RenderEngineResult(message, false, null, errorCode), $"{nameof(renderStatus)} not OK");

            var renderedTemplate = RenderTemplate(data);
            var depMan = Services.BlockResourceExtractor;
            var result = depMan.Process(renderedTemplate);
            return l.ReturnAsOk(result);
        }

        private void CheckExpectedTemplateErrors()
        {
            if (Template == null)
                throw new RenderingException("Template Configuration Missing");

            if (Template.ContentType != "" && Services.AppStatesLazy.Value.Get(App).GetContentType(Template.ContentType) == null)
                throw new RenderingException("The contents of this module cannot be displayed because I couldn't find the assigned content-type.");
        }

        private (RenderStatusType RenderStatus, string Message, string ErrorCode) CheckExpectedNoRenderConditions()
        {
            if (Template.ContentType != "" && Template.ContentItem == null &&
                Block.Configuration.Content.All(e => e == null))
                return (RenderStatusType.MissingData, ToolbarForEmptyTemplate, BlockBuildingConstants.ErrorDataIsMissing);

            return (RenderStatusType.Ok, null, null);
        }

        // todo: refactor - this should go somewhere, I just don't know where :)
        [PrivateApi]
        internal static string ToolbarForEmptyTemplate
        {
            get
            {
                var toolbar = "<ul class='sc-menu' data-toolbar='" +
                              JsonSerializer.Serialize(new {sortOrder = 0, useModuleList = true, action = "edit"}, JsonOptions.SafeJsonForHtmlAttributes) +
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
            var templatePermissions = Services.AppPermCheckLazy.Value
                .ForItem(Block.Context, App, Template.Entity);

            // Views only use permissions to prevent access, so only check if there are any configured permissions
            if (user.IsSiteAdmin || !templatePermissions.HasPermissions)
                return;

            if (!templatePermissions.UserMay(GrantSets.ReadSomething))
                throw new RenderingException(new UnauthorizedAccessException(
                    "This view is not accessible for the current user. To give access, change permissions in the view settings. See http://2sxc.org/help?tag=view-permissions"));
        }

    }
}