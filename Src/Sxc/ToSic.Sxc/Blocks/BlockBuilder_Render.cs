using System;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helper;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web.PageFeatures;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {
        [PrivateApi]
        public bool WrapInDiv { get; set; } = true;

        [PrivateApi]
        public IRenderingHelper RenderingHelper => _rendHelp.Get(() => Deps.RenderHelpGen.New().Init(Block));
        private readonly GetOnce<IRenderingHelper> _rendHelp = new GetOnce<IRenderingHelper>();

        public IRenderResult Run(bool topLevel)
        {
            // Cache Result on multiple runs
            if (_result != null) return _result;
            var wrapLog = Log.Fn<IRenderResult>(timer: true);
            try
            {
                var (html, err) = RenderInternal();
                var result = new RenderResult
                {
                    Html = html,
                    IsError = err,
                    ModuleId = Block.ParentId,
                    CanCache = !err && (Block.ContentGroupExists || Block.Configuration?.PreviewTemplateId.HasValue == true),
                };

                // case when we do not have an app
                if (DependentApps.Any())
                    result.DependentApps.AddRange(DependentApps);

                // The remaining stuff should only happen at top-level
                // Because once these properties are picked up, they are flushed
                // So only the top-level should get them
                if (topLevel)
                {
                    var allChanges = Deps.PageChangeSummary.Value
                        .FinalizeAndGetAllChanges(Block.Context.PageServiceShared, Block.Context.UserMayEdit);

                    // Head & Page Changes
                    result.HeadChanges = allChanges.HeadChanges;
                    result.PageChanges = allChanges.PageChanges;
                    result.Assets = allChanges.Assets;
                    result.Features = allChanges.Features;
                    result.FeaturesFromSettings = allChanges.FeaturesFromSettings;

                    result.HttpStatusCode = allChanges.HttpStatusCode;
                    result.HttpStatusMessage = allChanges.HttpStatusMessage;
                    result.HttpHeaders = allChanges.HttpHeaders;

                    // CSP settings
                    result.CspEnabled = allChanges.CspEnabled;
                    result.CspEnforced = allChanges.CspEnforced;
                    result.CspParameters = allChanges.CspParameters;
                    result.Errors = allChanges.Errors;
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

                            // Put all assets into the global page service for final processing later on
                            Block.Context.PageServiceShared.AddAssets(renderEngineResult);
                        }
                        else body = "";
                    }
                    catch (Exception ex)
                    {
                        body = RenderingHelper.DesignErrorMessage(ex, true);
                        err = true;
                    }
                #endregion


                var licenseNotOk = GenerateWarningMsgIfLicenseNotOk();
                if (licenseNotOk != null) body = licenseNotOk + body;

                #region Wrap it all up into a nice wrapper tag

                // Figure out some if we should add the edit context
                // by default the editors will get it
                // in special cases the razor requests it to added as well
                var addEditCtx = Block.Context.UserMayEdit;
                if (!addEditCtx && Block.BlockFeatureKeys.Any())
                {
                    var features = Block.Context.PageServiceShared.PageFeatures.GetWithDependents(Block.BlockFeatureKeys, Log);
                    addEditCtx = features.Contains(BuiltInFeatures.ContextModule);
                }

                // Wrap
                var result = WrapInDiv
                    ? RenderingHelper.WrapInContext(body,
                        instanceId: Block.ParentId,
                        contentBlockId: Block.ContentBlockId,
                        editContext: addEditCtx)
                    : body;
                #endregion

                #region Add Custom Tags to the end if provided by the ModuleService

                var additionalTags = Deps.ModuleService.MoreTags;
                if (additionalTags.Any()) 
                    result += "\n" + string.Join("\n", additionalTags.Select(t => t?.ToString()));

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

            var installer = Deps.EnvInstGen.New();
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
        /// license ok state
        /// </summary>
        protected bool LicenseOk => _licenseOk.Get(() => Deps.LicenseService.Value.HaveValidLicense);
        private readonly GetOnce<bool> _licenseOk = new GetOnce<bool>();

        private string GenerateWarningMsgIfLicenseNotOk()
        {
            if (LicenseOk) return null;
            
            Log.A("none of the licenses are valid");
            var result = RenderingHelper.DesignWarningForSuperUserOnly("Registration is Invalid. Some features may be disabled because of this. Please reactivate the registration in Apps Management.", false, encodeMessage: false); // don't encode, as it contains special links
            return result;
        }

        /// <summary>
        /// Get the rendering engine, but avoid double execution.
        /// In some cases, the engine is needed early on to be sure if we need to do some overrides, but execution should then be later on Render()
        /// </summary>
        /// <returns></returns>
        public IEngine GetEngine()
        {
            var wrapLog = Log.Fn<IEngine>(timer: true);
            if (_engine != null) return wrapLog.Return(_engine, "cached");
            // edge case: view hasn't been built/configured yet, so no engine to find/attach
            if (Block.View == null) return wrapLog.ReturnNull("no view");
            _engine = Deps.EngineFactory.CreateEngine(Block.View);
            _engine.Init(Log).Init(Block);
            return wrapLog.Return(_engine, "created");
        }
        private IEngine _engine;

    }
}
