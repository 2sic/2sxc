using System;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {
        [PrivateApi]
        public bool WrapInDiv { get; set; } = true;

        private IRenderingHelper RenderingHelper =>
            _rendHelp ?? (_rendHelp = _renderHelpGen.New.Init(Block, Log));
        private IRenderingHelper _rendHelp;

        public string Render() => Run(true).Html;

        public IRenderResult Run(bool topLevel = true)
        {
            if (_result != null) return _result;
            var wrapLog = Log.Call<IRenderResult>();
            try
            {
                var result = new RenderResult
                {
                    Html = RenderInternal(),
                    ModuleId = Block.ParentId
                };

                result.DependentApps.Add(Block.AppId);

                // TODO: this may fail on a sub-tmplate, must research
                result.Assets = Assets;

                // The remaining stuff should only happen at top-level
                // Because once these properties are picked up, they are flushed
                // So only the top-level should get them
                if (topLevel)
                {
                    var pss = Block.Context.PageServiceShared;
                    // Page Features
                    if (Block.Context.UserMayEdit)
                    {
                        pss.Activate(BuiltInFeatures.Toolbars.Key);
                        pss.Activate(BuiltInFeatures.ToolbarsAuto.Key);
                    }

                    result.Features = pss.Features.GetFeaturesWithDependentsAndFlush(Log);

                    // Head & Page Changes
                    result.HeadChanges = pss.GetHeadChangesAndFlush(Log);
                    result.PageChanges = pss.GetPropertyChangesAndFlush(Log);
                    result.FeaturesFromSettings = pss.Features.FeaturesFromSettingsGetNew(Log);

                    result.HttpStatusCode = pss.HttpStatusCode;
                    result.HttpStatusMessage = pss.HttpStatusMessage;
                }

                _result = result;
            }
            catch (Exception ex)
            {
                Log.Add("Error!");
                Log.Exception(ex);
            }

            return wrapLog(null, _result);
        }

        private IRenderResult _result;

        private string RenderInternal()
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
                                // 2021-09-01 before: if (RootBuilder is BlockBuilder parentBlock) parentBlock.UiAddJsApi = engine.ActivateJsApi;
                                // todo: should change this, so the param isn't in ActivateJsApi but clearer
                                Block.Context.PageServiceShared.Features.Activate(BuiltInFeatures.JsCore.Key);
                            }

                            // TODO: this should use the same pattern as features, waiting to be picked up
                            TransferEngineAssetsToParent(engine);
                        }
                        else body = "";
                    }
                    catch (Exception ex)
                    {
                        body = RenderingHelper.DesignErrorMessage(ex, true, "Error rendering template", false, true);
                    }
                #endregion

                #region Wrap it all up into a nice wrapper tag

                // Figure out some if we should add the edit context
                // by default the editors will get it
                // in special cases the razor requests it to added as well
                var addEditCtx = Block.Context.UserMayEdit;
                if (!addEditCtx && Block.BlockFeatureKeys.Any())
                {
                    var features = Block.Context.PageServiceShared.Features.GetWithDependents(Block.BlockFeatureKeys, Log);
                    addEditCtx = features.Contains(BuiltInFeatures.ModuleContext);
                }

                // 2022-03-03 2dm - moving special properties to page-activate features #pageActivate
                // WIP, if all is good, remove these comments end of March
                //addEditCtx = UiAddEditContext;

                // Wrap
                var result = WrapInDiv
                    ? RenderingHelper.WrapInContext(body,
                        instanceId: Block.ParentId,
                        contentBlockId: Block.ContentBlockId,
                        editContext: addEditCtx)
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

            var installer = _envInstGen.New;
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
        /// <returns></returns>
        public IEngine GetEngine()
        {
            var wrapLog = Log.Call<IEngine>();
            if (_engine != null) return wrapLog("cached", _engine);
            // edge case: view hasn't been built/configured yet, so no engine to find/attach
            if (Block.View == null) return wrapLog("no view", null);
            _engine = EngineFactory.CreateEngine(Block.View, _razorEngineGen, _tokenEngineGen);
            _engine.Init(Block, Purpose.WebView, Log);
            return wrapLog("created", _engine);
        }
        private IEngine _engine;

    }
}
