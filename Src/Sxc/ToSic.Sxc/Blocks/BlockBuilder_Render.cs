using System;
using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {
        [PrivateApi]
        public bool WrapInDiv { get; set; } = true;

        private IRenderingHelper RenderingHelper =>
            _rendHelp ?? (_rendHelp = Block.Context.ServiceProvider.Build<IRenderingHelper>().Init(Block, Log));
        private IRenderingHelper _rendHelp;


        public string Render()
        {
          var wrapLog = Log.Call<string>();

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
                        body = Block.Context.UserMayEdit
                            ? "" // stop further processing
                            // end users should see server error as no js-side processing will happen
                            : RenderingHelper.DesignErrorMessage(
                                new Exception("Data is missing - usually when a site is copied " +
                                              "but the content / apps have not been imported yet" +
                                              " - check 2sxc.org/help?tag=export-import"),
                                true, "Error - needs admin to fix", false, true);
                    }
                }
                #endregion

                #region try to render the block or generate the error message
                if (body == null)
                    try
                    {
                        if (Block.View != null) // when a content block is still new, there is no definition yet
                        {
                            Log.Add("standard case, found template, will render");
                            var engine = GetEngine();
                            body = engine.Render();
                            // Activate-js-api is true, if the html has some <script> tags which tell it to load the 2sxc
                            // only set if true, because otherwise we may accidentally overwrite the previous setting
                            if (engine.ActivateJsApi)
                            {
                                Log.Add("template referenced 2sxc.api JS in script-tag: will enable");
                                if (RootBuilder is BlockBuilder parentBlock) parentBlock.UiAddJsApi = engine.ActivateJsApi;
                            }

                            TransferEngineAssets(engine);
                        }
                        else body = "";
                    }
                    catch (Exception ex)
                    {
                        body = RenderingHelper.DesignErrorMessage(ex, true, "Error rendering template", false, true);
                    }
                #endregion

                #region Wrap it all up into a nice wrapper tag
                var result = WrapInDiv
                    ? RenderingHelper.WrapInContext(body,
                        instanceId: Block.ParentId,
                        contentBlockId: Block.ContentBlockId,
                        editContext: UiAddEditContext,
                        autoToolbar: UiAutoToolbar)
                    : body;
                #endregion

                return wrapLog(null, result);
            }
            catch (Exception ex)
            {
                return wrapLog("error", RenderingHelper.DesignErrorMessage(ex, true,
                    null, true, true));
            }
        }

        /// <summary>
        /// Cache the installation ok state, because once it's ok, we don't need to re-check
        /// </summary>
        internal static bool InstallationOk;

        private string GenerateErrorMsgIfInstallationNotOk()
        {
            if (InstallationOk) return null;

            var installer = Block.Context.ServiceProvider.Build<IEnvironmentInstaller>();
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

        /// <summary>
        /// Get the rendering engine, but avoid double execution.
        /// In some cases, the engine is needed early on to be sure if we need to do some overrides, but execution should then be later on Render()
        /// </summary>
        /// <param name="renderingPurpose"></param>
        /// <returns></returns>
        public IEngine GetEngine(Purpose renderingPurpose = Purpose.WebView)
        {
            if (_engine != null) return _engine;
            // edge case: view hasn't been built/configured yet, so no engine to find/attach
            if (Block.View == null) return null;
            _engine = EngineFactory.CreateEngine(Block.Context.ServiceProvider, Block.View);
            _engine.Init(Block, renderingPurpose, Log);
            return _engine;
        }
        private IEngine _engine;

    }
}
