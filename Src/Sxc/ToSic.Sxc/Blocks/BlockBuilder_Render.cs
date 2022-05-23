using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using ToSic.Sxc.Web.PageFeatures;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {
        [PrivateApi]
        public bool WrapInDiv { get; set; } = true;

        [PrivateApi]
        public IRenderingHelper RenderingHelper => _rendHelp2.Get(() => _renderHelpGen.New.Init(Block, Log));
        private readonly ValueGetOnce<IRenderingHelper> _rendHelp2 = new ValueGetOnce<IRenderingHelper>();

        public string Render() => Run(true).Html;

        public IRenderResult Run(bool topLevel = true)
        {
            if (_result != null) return _result;
            var wrapLog = Log.Fn<IRenderResult>();
            try
            {
                var (html, err) = RenderInternal();
                var result = new RenderResult
                {
                    Html = html,
                    IsError = err,
                    ModuleId = Block.ParentId,

                    Assets = Assets, // TODO: this may fail on a sub-sub-template, must research
                    CanCache = !err && (Block.ContentGroupExists || Block.Configuration?.PreviewTemplateId.HasValue == true),
                };

                // case when we do not have an app
                if (DependentApps.Any())
                    result.DependentApps.AddRange(DependentApps);

                //if (Block.AppId != 0 && Block.App?.AppState != null)
                //    result.DependentApps.Add(new DependentApp { AppId = Block.AppId, CacheTimestamp = Block.App.AppState.CacheTimestamp });

                // The remaining stuff should only happen at top-level
                // Because once these properties are picked up, they are flushed
                // So only the top-level should get them
                if (topLevel)
                {
                    var pss = Block.Context.PageServiceShared;
                    // Page Features
                    if (Block.Context.UserMayEdit)
                    {
                        pss.Activate(BuiltInFeatures.Toolbars.NameId);
                        pss.Activate(BuiltInFeatures.ToolbarsAuto.NameId);
                    }

                    result.Features = pss.PageFeatures.GetFeaturesWithDependentsAndFlush(Log);

                    // Head & Page Changes
                    result.HeadChanges = pss.GetHeadChangesAndFlush(Log);
                    result.PageChanges = pss.GetPropertyChangesAndFlush(Log);
                    var (newAssets, rest) = ConvertSettingsAssetsIntoReal(pss.PageFeatures.FeaturesFromSettingsGetNew(Log));

                    Assets.AddRange(newAssets);
                    result.Assets = Assets;

                    result.FeaturesFromSettings = rest;

                    result.HttpStatusCode = pss.HttpStatusCode;
                    result.HttpStatusMessage = pss.HttpStatusMessage;
                    result.HttpHeaders = pss.HttpHeaders;

                    // CSP settings
                    result.CspEnabled = pss.Csp.IsEnabled;
                    result.CspEnforced = pss.Csp.IsEnforced;
                    result.CspParameters = pss.Csp.CspParameters();
                    // Whitelist any assets which were officially ok, or which were from the settings
                    var additionalCsp = GetCspListFromAssets(Assets);
                    if(additionalCsp != null) result.CspParameters.Add(additionalCsp);
                }

                _result = result;
            }
            catch (Exception ex)
            {
                Log.A("Error!");
                Log.Ex(ex);
            }

            return wrapLog.Return(_result);
        }


        
        private CspParameters GetCspListFromAssets(List<IClientAsset> assets)
        {
            if (assets == null || assets.Count == 0) return null;
            var toWhitelist = assets
                .Where(a => a.WhitelistInCsp)
                .Where(a => !a.Url.NeverNull().StartsWith("/")) // skip local files
                .ToList();
            if (!toWhitelist.Any()) return null;
            var whitelist = new CspParameters();
            foreach (var asset in toWhitelist)
            {
                whitelist.Add((asset.IsJs ? "script" : "style") + "-src", asset.Url);
            }

            return whitelist;
        }

        private (List<IClientAsset> newAssets, List<IPageFeature> rest) ConvertSettingsAssetsIntoReal(List<PageFeatureFromSettings> featuresFromSettings)
        {
            var wrapLog = Log.Fn<(List<IClientAsset> newAssets, List<IPageFeature> rest)>($"{featuresFromSettings.Count}");
            var newAssets = new List<IClientAsset>();
            foreach (var settingFeature in featuresFromSettings)
            {
                var extracted = _resourceExtractor.Ready.Process(settingFeature.Html);
                if (!extracted.Assets.Any()) continue;
                Log.A($"Moved Feature Html {settingFeature.NameId} to assets");

                // All resources from the settings are seen as safe
                extracted.Assets.ForEach(a => a.WhitelistInCsp = true);

                newAssets.AddRange(extracted.Assets);
                // Reset the HTML to what's left after extracting the resources
                settingFeature.Html = extracted.Html;
            }

            var featsLeft = featuresFromSettings
                .Where(f => !string.IsNullOrWhiteSpace(f.Html))
                .Cast<IPageFeature>()
                .ToList();

            return wrapLog.Return((newAssets, featsLeft), $"New: {newAssets.Count}; Rest: {featsLeft.Count}");
        }

        private IRenderResult _result;

        private (string Html, bool Error) RenderInternal()
        {
            var wrapLog = Log.Fn<(string, bool)>();

            try
            {
                // New 13.11 - must set appid etc. for dependencies before we start
                // So that in a stack of renders, the top-most was set first
                PreSetAppDependenciesToRoot();

                // do pre-check to see if system is stable & ready
                var (body, err) = GenerateErrorMsgIfInstallationNotOk();

                #region check if the content-group exists (sometimes it's missing if a site is being imported and the data isn't in yet
                if (body == null)
                {
                    Log.A("pre-init innerContent content is empty so no errors, will build");
                    if (Block.DataIsMissing)
                    {
                        Log.A("content-block is missing data - will show error or just stop if not-admin-user");
                        var blockId = Block.Configuration?.BlockIdentifierOrNull;
                        var msg = "Data is missing. This is common when a site is copied " +
                                  "but the content / apps have not been imported yet" +
                                  " - check 2sxc.org/help?tag=export-import - " +
                                  $" Zone/App: {Block.ZoneId}/{Block.AppId}; App NameId: {blockId?.AppNameId}; ContentBlock GUID: {blockId?.Guid}";
                            body = RenderingHelper.DesignErrorMessage(new Exception(msg),
                                true);
                        err = true;
                    }
                }
                #endregion

                #region try to render the block or generate the error message
                if (body == null)
                    try
                    {
                        if (Block.View != null) // when a content block is still new, there is no definition yet
                        {
                            Log.A("standard case, found template, will render");
                            var engine = GetEngine();
                            var renderEngineResult = engine.Render();
                            body = renderEngineResult.Html;
                            // Activate-js-api is true, if the html has some <script> tags which tell it to load the 2sxc
                            // only set if true, because otherwise we may accidentally overwrite the previous setting
                            if (renderEngineResult.ActivateJsApi)
                            {
                                Log.A("template referenced 2sxc.api JS in script-tag: will enable");
                                Block.Context.PageServiceShared.PageFeatures.Activate(BuiltInFeatures.JsCore.NameId);
                            }

                            // TODO: this should use the same pattern as features, waiting to be picked up
                            TransferCurrentAssetsAndAppDependenciesToRoot(renderEngineResult);
                        }
                        else body = "";
                    }
                    catch (Exception ex)
                    {
                        body = RenderingHelper.DesignErrorMessage(ex, true);
                        err = true;
                    }
                #endregion

                #region Wrap it all up into a nice wrapper tag

                // Figure out some if we should add the edit context
                // by default the editors will get it
                // in special cases the razor requests it to added as well
                var addEditCtx = Block.Context.UserMayEdit;
                if (!addEditCtx && Block.BlockFeatureKeys.Any())
                {
                    var features = Block.Context.PageServiceShared.PageFeatures.GetWithDependents(Block.BlockFeatureKeys, Log);
                    addEditCtx = features.Contains(BuiltInFeatures.ModuleContext);
                }

                // Wrap
                var result = WrapInDiv
                    ? RenderingHelper.WrapInContext(body,
                        instanceId: Block.ParentId,
                        contentBlockId: Block.ContentBlockId,
                        editContext: addEditCtx)
                    : body;
                #endregion

                return wrapLog.Return((result, err));
            }
            catch (Exception ex)
            {
                return wrapLog.Return((RenderingHelper.DesignErrorMessage(ex, true, addContextWrapper: true), true), "error");
            }
        }

        /// <summary>
        /// Cache the installation ok state, because once it's ok, we don't need to re-check
        /// </summary>
        internal static bool InstallationOk;

        private (string Html, bool Error) GenerateErrorMsgIfInstallationNotOk()
        {
            if (InstallationOk) return (null, false);

            var installer = _envInstGen.New;
            var notReady = installer.UpgradeMessages();
            if (!string.IsNullOrEmpty(notReady))
            {
                Log.A("system isn't ready,show upgrade message");
                var result = RenderingHelper.DesignErrorMessage(new Exception(notReady), true, encodeMessage: false); // don't encode, as it contains special links
                return (result, true);
            }

            InstallationOk = true;
            Log.A("system is ready, no upgrade-message to show");
            return (null, false);
        }

        /// <summary>
        /// Get the rendering engine, but avoid double execution.
        /// In some cases, the engine is needed early on to be sure if we need to do some overrides, but execution should then be later on Render()
        /// </summary>
        /// <returns></returns>
        public IEngine GetEngine()
        {
            var wrapLog = Log.Fn<IEngine>();
            if (_engine != null) return wrapLog.Return(_engine, "cached");
            // edge case: view hasn't been built/configured yet, so no engine to find/attach
            if (Block.View == null) return wrapLog.ReturnNull("no view");
            _engine = EngineFactory.CreateEngine(Block.View, _razorEngineGen, _tokenEngineGen);
            _engine.Init(Log).Init(Block);
            return wrapLog.Return(_engine, "created");
        }
        private IEngine _engine;

    }
}
