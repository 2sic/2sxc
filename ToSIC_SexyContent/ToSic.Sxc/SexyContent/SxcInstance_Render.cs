using System;
using System.Web;
using ToSic.Eav;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Interfaces;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent
{
    public partial class CmsInstance
    {


        internal bool RenderWithDiv = true;
        public bool UserMayEdit => _userMayEdit
            ?? (_userMayEdit = EnvFac.InstancePermissions(Log, Container, App).UserMay(GrantSets.WriteSomething)).Value;
        private bool? _userMayEdit;


        internal IRenderingHelpers RenderingHelper =>
            _rendHelp ?? (_rendHelp = Factory.Resolve<IRenderingHelpers>().Init(this, Log));
        private IRenderingHelpers _rendHelp;


        public HtmlString Render()
        {
            Log.Add("render");

            try
            {
                // do pre-check to see if system is stable & ready
                var body = GenerateErrorMsgIfInstallationNotOk();

                #region check if the content-group exists (sometimes it's missing if a site is being imported and the data isn't in yet
                if (body == null)
                {
                    Log.Add("pre-init innerContent content is empty so no errors, will build");
                    if (Block.DataIsMissing)
                    {
                        Log.Add("content-block is missing data - will show error or just stop if not-admin-user");
                        if (UserMayEdit)
                            body = ""; // stop further processing
                        else // end users should see server error as no js-side processing will happen
                        {
                            var ex =
                                new Exception(
                                    "Data is missing - usually when a site is copied but the content / apps have not been imported yet - check 2sxc.org/help?tag=export-import");
                            body = RenderingHelper.DesignErrorMessage(ex, true,
                                "Error - needs admin to fix", false, true);
                        }
                    }
                }
                #endregion

                #region try to render the block or generate the error message
                if (body == null)
                    try
                    {
                        if (View != null) // when a content block is still new, there is no definition yet
                        {
                            Log.Add("standard case, found template, will render");
                            var engine = GetRenderingEngine(Purpose.WebView);
                            body = engine.Render();
                        }
                        else body = "";
                    }
                    catch (Exception ex)
                    {
                        body = RenderingHelper.DesignErrorMessage(ex, true, "Error rendering template", false, true);
                    }
                #endregion

                #region Wrap it all up into a nice wrapper tag
                var result = RenderWithDiv
                    ? RenderingHelper.WrapInContext(body,
                        instanceId: Block.ParentId,
                        contentBlockId: Block.ContentBlockId,
                        editContext: UiAddEditContext,
                        autoToolbar: UiAutoToolbar)
                    : body;
                #endregion

                return new HtmlString(result);
            }
            catch (Exception ex)
            {
                return new HtmlString(RenderingHelper.DesignErrorMessage(ex, true, null, true, true));
            }
        }


        /// <summary>
        /// Cache the installation ok state, because once it's ok, we don't need to re-check
        /// </summary>
        internal static bool InstallationOk;

        private string GenerateErrorMsgIfInstallationNotOk()
        {
            if (InstallationOk) return null;

            var installer = Factory.Resolve<IEnvironmentInstaller>();
            var notReady = installer.UpgradeMessages();
            if (!string.IsNullOrEmpty(notReady))
            {
                Log.Add("system isn't ready,show upgrade message");
                return RenderingHelper.DesignErrorMessage(new Exception(notReady), true,
                    "Error - needs admin to fix this", false, false);
            }

            InstallationOk = true;
            Log.Add("system is ready, no upgrade-message to show");
            return null;
        }

        public IEngine GetRenderingEngine(Purpose renderingPurpose)
        {
            var engine = EngineFactory.CreateEngine(View);
            engine.Init(View, App, Container, Block.Data, renderingPurpose, this, Log);
            return engine;
        }

    }
}
