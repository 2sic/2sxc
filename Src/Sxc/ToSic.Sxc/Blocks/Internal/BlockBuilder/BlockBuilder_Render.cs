using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web.Internal.PageFeatures;
using static ToSic.Sxc.Blocks.Internal.BlockBuildingConstants;

namespace ToSic.Sxc.Blocks.Internal;

public partial class BlockBuilder
{
    [PrivateApi]
    public bool WrapInDiv { get; set; } = true;

    [PrivateApi]
    public IRenderingHelper RenderingHelper => _rendHelp.Get(() => Services.RenderHelpGen.New().Init(Block));
    private readonly GetOnce<IRenderingHelper> _rendHelp = new();

    public IRenderResult Run(bool topLevel, RenderSpecs specs)
    {
        // Cache Result on multiple runs
        if (_result != null) return _result;
        var l = Log.Fn<IRenderResult>(timer: true);
        try
        {
            var (html, isErr, exsOrNull) = RenderInternal(specs);

            var result = new RenderResult(html)
            {
                IsError = isErr,
                ModuleId = Block.ParentId,
                // Note 2023-10-30 2dm changed the handling of the preview template and checks if it's set. In case caching is too aggressive this can be the problem. Remove early 2024
                //CanCache = !isErr && exsOrNull.SafeNone() && (Block.ContentGroupExists || Block.Configuration?.PreviewTemplateId.HasValue == true),
                CanCache = !isErr && exsOrNull.SafeNone() && (Block.ContentGroupExists || Block.Configuration?.PreviewViewEntity != null),
            };

            // case when we do not have an app
            if (DependentApps.Any())
                result.DependentApps.AddRange(DependentApps);

            // The remaining stuff should only happen at top-level
            // Because once these properties are picked up, they are flushed
            // So only the top-level should get them
            if (topLevel)
            {
                var allChanges = Services.PageChangeSummary.Value
                    .FinalizeAndGetAllChanges(Block.Context.PageServiceShared, Block.Context.Permissions.IsContentAdmin);

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
            l.A("Error!");
            l.Ex(ex);
        }

        // Add information to code changes if relevant
        Services.CodeInfos.AddContext(() => new SpecsForLogHistory().BuildSpecsForLogHistory(Block));


        return l.Return(_result);
    }


    private IRenderResult _result;

    private (string Html, bool IsError, List<Exception> exsOrNull) RenderInternal(RenderSpecs specs)
    {
        var l = Log.Fn<(string, bool, List<Exception>)>(timer: true);

        // any errors from dnn requirements check (like missing c# 8.0)
        if (specs.RenderEngineResult?.ExceptionsOrNull != null)
            return l.Return(new(specs.RenderEngineResult.Html, specs.RenderEngineResult.ExceptionsOrNull != null, specs.RenderEngineResult.ExceptionsOrNull), "dnn requirements (c# 8.0...) not met");

        var exceptions = new List<Exception>();
        try
        {
            // New 13.11 - must set appid etc. for dependencies before we start
            // So that in a stack of renders, the top-most was set first
            PreSetAppDependenciesToRoot();

            // do pre-check to see if system is stable & ready
            var (body, err) = GenerateErrorMsgIfInstallationNotOk();
            var errorCode = err ? ErrorInstallationNotOk : null;

            #region Content-Group Exists
            // Check if the content-group exists - sometimes the Content-Group it's missing if a site is being imported and the data isn't in yet
            if (body == null)
            {
                l.A("pre-init innerContent content is empty so no errors, will build");
                if (Block.DataIsMissing)
                {
                    l.A("content-block is missing data - will show error or just stop if not-admin-user");
                    var blockId = Block.Configuration?.BlockIdentifierOrNull;
                    var msg = "Data is missing. This is common when a site is copied " +
                              "but the content / apps have not been imported yet" +
                              " - check 2sxc.org/help?tag=export-import - " +
                              $" Zone/App: {Block.ZoneId}/{Block.AppId}; App NameId: {blockId?.AppNameId}; ContentBlock GUID: {blockId?.Guid}";
                    var ex = new Exception(msg);
                    exceptions.Add(ex);
                    body = RenderingHelper.DesignErrorMessage(exceptions, true);
                    err = true;
                    errorCode = ErrorDataIsMissing;
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
                        var renderEngineResult = engine.Render(specs);
                        body = renderEngineResult.Html;
                        if (renderEngineResult.ExceptionsOrNull != null)
                            exceptions.AddRange(renderEngineResult.ExceptionsOrNull);
                        errorCode = renderEngineResult.ErrorCode ?? errorCode;
                        if (errorCode == null && body?.Contains(ErrorHtmlMarker) == true) 
                            errorCode = ErrorGeneral;
                        // Activate-js-api is true, if the html has some <script> tags which tell it to load the 2sxc
                        // only set if true, because otherwise we may accidentally overwrite the previous setting
                        if (renderEngineResult.ActivateJsApi)
                        {
                            l.A("template referenced 2sxc.api JS in script-tag: will enable");
                            Block.Context.PageServiceShared.PageFeatures.Activate(SxcPageFeatures.JsCore.NameId);
                        }

                        // Put all assets into the global page service for final processing later on
                        Block.Context.PageServiceShared.AddAssets(renderEngineResult);
                    }
                    else body = "";
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    body = RenderingHelper.DesignErrorMessage(exceptions, true);
                    err = true;
                    errorCode = ErrorRendering;
                }
            #endregion


            var licenseNotOk = GenerateWarningMsgIfLicenseNotOk();
            if (licenseNotOk != null) body = licenseNotOk + body;

            #region Wrap it all up into a nice wrapper tag

            // Figure out some if we should add the edit context
            // by default the editors will get it
            // in special cases the razor requests it to added as well
            var addEditCtx = Block.Context.Permissions.IsContentAdmin;
            var addJsApiOnly = false;
            if (!addEditCtx && Block.BlockFeatureKeys.Any())
            {
                var features = Block.Context.PageServiceShared.PageFeatures.GetWithDependents(Block.BlockFeatureKeys, Log);
                addEditCtx = features.Contains(SxcPageFeatures.ContextModule);
                addJsApiOnly = features.Contains(SxcPageFeatures.JsApiOnModule);
            }

            #region Add Custom Tags to the end if provided by the ModuleService - like TurnOn - not ideal yet

            // This is not ideal, because it actually changes what's in the DIV
            // We would rather add it to the end, but ATM that doesn't trigger turnOn in AJAX reload

            var additionalTags = Services.ModuleService.MoreTags;
            var bodyWithAddOns = additionalTags.Any()
                ? body + "\n" + string.Join("\n", additionalTags.Select(t => t?.ToString()))
                : body;

            #endregion

            var stats = new RenderStatistics { RenderMs = (int)l.Timer.ElapsedMilliseconds, UseLightSpeed = specs.UseLightspeed};

            // Wrap
            var result = WrapInDiv
                ? RenderingHelper.WrapInContext(bodyWithAddOns,
                    instanceId: Block.ParentId,
                    contentBlockId: Block.ContentBlockId,
                    editContext: addEditCtx,
                    jsApiContext: addJsApiOnly,
                    errorCode: errorCode,
                    exsOrNull: exceptions,
                    statistics: stats)
                : bodyWithAddOns;
            #endregion

            return l.ReturnAsOk((result, err, exceptions));
        }
        catch (Exception ex)
        {
            exceptions.Add(ex);
            return l.ReturnAsError((RenderingHelper.DesignErrorMessage(exceptions, true, addContextWrapper: true), true, exceptions));
        }
    }

    /// <summary>
    /// Cache the installation ok state, because once it's ok, we don't need to re-check
    /// </summary>
    internal static bool InstallationOk;

    private (string Html, bool Error) GenerateErrorMsgIfInstallationNotOk()
    {
        if (InstallationOk) return (null, false);

        var installer = Services.EnvInstGen.New();
        var notReady = installer.UpgradeMessages();
        if (!string.IsNullOrEmpty(notReady))
        {
            Log.A("system isn't ready,show upgrade message");
            var result = RenderingHelper.DesignErrorMessage([new(notReady)], true, encodeMessage: false); // don't encode, as it contains special links
            return (result, true);
        }

        InstallationOk = true;
        Log.A("system is ready, no upgrade-message to show");
        return (null, false);
    }

    /// <summary>
    /// license ok state
    /// </summary>
    protected bool AnyLicenseOk => _licenseOk.Get(() => Services.LicenseService.Value.HaveValidLicense);
    private readonly GetOnce<bool> _licenseOk = new();

    private string GenerateWarningMsgIfLicenseNotOk()
    {
        if (AnyLicenseOk) return null;

        Log.A("none of the licenses are valid");
        var warningLink = Tag.A("go.2sxc.org/license-warning").Href("https://go.2sxc.org/license-warning").Target("_blank");
        var appsManagementLink = Tag.A("System-Management").Href("#").On("click", "$2sxc(this).cms.run({ action: 'system', params: { newWindow: true }})");
        var warningMsg = "Registration not valid so some features may be disabled. " +
                         $"Please re-register in {appsManagementLink}. " +
                         "<br>" +
                         $"This is common after a major upgrade. See {warningLink}.";
        var result = RenderingHelper.DesignWarningForSuperUserOnly(warningMsg, false, encodeMessage: false); // don't encode, as it contains special links
        return result;
    }

    /// <summary>
    /// Get the rendering engine, but avoid double execution.
    /// In some cases, the engine is needed early on to be sure if we need to do some overrides, but execution should then be later on Render()
    /// </summary>
    /// <returns></returns>
    public IEngine GetEngine()
    {
        var l = Log.Fn<IEngine>(timer: true);
        if (_engine != null) return l.Return(_engine, "cached");
        // edge case: view hasn't been built/configured yet, so no engine to find/attach
        if (Block.View == null) return l.ReturnNull("no view");
        _engine = Services.EngineFactory.CreateEngine(Block.View);
        _engine.Init(Block);
        return l.Return(_engine, "created");
    }
    private IEngine _engine;

    // 2023-11-14 2dm - nicer implementation
    // but not sure about side effects
    // as the note below mentions edge cases where the view hasn't been built yet...
    // so don't use until you're ready to test all use cases
    //public IEngine Engine => _engine.Get(() =>
    //{
    //    var l = Log.Fn<IEngine>(timer: true);
    //    // edge case: view hasn't been built/configured yet, so no engine to find/attach
    //    if (Block.View == null) return l.ReturnNull("no view");
    //    var engine = Services.EngineFactory.CreateEngine(Block.View);
    //    engine.Init(Block);
    //    return l.Return(engine, "created");
    //});

    //private readonly GetOnce<IEngine> _engine = new GetOnce<IEngine>();
}