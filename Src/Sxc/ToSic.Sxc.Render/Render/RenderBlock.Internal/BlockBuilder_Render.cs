using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web.Internal.PageFeatures;
using static ToSic.Sxc.Blocks.Internal.BlockBuildingConstants;

namespace ToSic.Sxc.Blocks.Internal;

public partial class BlockBuilder
{
    [PrivateApi]
    public bool WrapInDiv { get; set; } = true;

    [PrivateApi]
    public IRenderingHelper RenderingHelper => field ??= Services.RenderHelpGen.New().Init(Block);

    public IRenderResult Run(bool topLevel, RenderSpecs specs)
    {
        // Cache Result in case of multiple runs on the same service instance
        if (_cached != null)
            return _cached;

        var l = Log.Fn<IRenderResult>(timer: true);
        try
        {
            var (html, isErr, exceptionsOrNull) = RenderInternal(specs);

            // Caching only allowed if no errors, no exceptions, and the content-block is real (no preview etc.)
            var canCache = !isErr
                           && exceptionsOrNull.SafeNone()
                           && (Block.ContentGroupExists || Block.Configuration?.PreviewViewEntity != null);

            // The change summary should only happen at top-level
            // Because once these properties are picked up, they are flushed
            // So only the top-level should get them
            var changeSummary = topLevel
                ? Services.PageChangeSummary.Value
                    .FinalizeAndGetAllChanges(Block.ParentId, ((ContextOfBlock)Block.Context).PageServiceShared, specs, Block.Context.Permissions.IsContentAdmin)
                : new();


            var result = changeSummary with
            {
                AppId = Block.AppId,        // info for LightSpeedStats
                Html = html,                // Final HTML to add to page
                IsError = isErr,            // Error status
                CanCache = canCache,        // Can this be cached?
            };

            // If data comes from other apps, ensure that cache-tracking knows to depend on these changes
            if ((Block as BlockOfBase)?.DependentApps.Any() == true)
                result.DependentApps.AddRange(((BlockOfBase)Block).DependentApps);

            // Cache Result in case of multiple runs on the same service instance
            _cached = result;
        }
        catch (Exception ex)
        {
            l.A("Error!");
            l.Ex(ex);
        }

        // Add information to code changes if relevant
        Services.CodeInfos.AddContext(() => new SpecsForLogHistory().BuildSpecsForLogHistory(Block));


        return l.Return(_cached);
    }


    private IRenderResult _cached;

    private (string Html, bool IsError, List<Exception> exsOrNull) RenderInternal(RenderSpecs specs)
    {
        var l = Log.Fn<(string, bool, List<Exception>)>(timer: true);

        // any errors from dnn requirements check (like missing c# 8.0)
        var oldExceptions = specs.RenderEngineResult?.ExceptionsOrNull;
        if (oldExceptions != null)
            return l.Return((specs.RenderEngineResult.Html, true, oldExceptions), "dnn requirements (c# 8.0...) not met");

        var exceptions = new List<Exception>();
        try
        {
            // Make sure that each block pushes its App dependencies to the root block
            // for collecting later when it's done.
            // This way first the top block does this, later on inner-child blocks
            // will also do it (since this same code is called for them when they render).
            // Later on we'll collect the result.
            (Block as BlockOfBase)?.PushAppDependenciesToRoot();

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
                    var msg = "Data is missing. ";

                    msg += (Block.Context.AppReader?.IsHealthy == false)
                        ? "The app is unhealthy, indicating that data wasn't properly loaded from SQL. "
                          + "This is the message: '" + Block.Context.AppReader.HealthMessage + "'. "
                          + "Please check the insights to see in more detail what happened."
                        : "This is common when a site is copied " +
                          "but the content / apps have not been imported yet" +
                          " - check 2sxc.org/help?tag=export-import - ";
                    msg += $" Zone/App: {Block.ZoneId}/{Block.AppId}; App NameId: {blockId?.AppNameId}; ContentBlock GUID: {blockId?.Guid}";

                    var ex = new Exception(msg);
                    exceptions.Add(ex);
                    body = RenderingHelper.DesignErrorMessage(exceptions, true);
                    err = true;
                    errorCode = ErrorDataIsMissing;
                }
            }
            #endregion

            #region App is unhealthy

            if (Block.Context.AppReader?.IsHealthy == false)
            {
                Log.A("app is unhealthy, show health message");
                exceptions.Add(new(AppIsUnhealthy + Block.Context.AppReader.HealthMessage));
                body = RenderingHelper.DesignErrorMessage(exceptions, true, AppIsUnhealthy + Render.RenderingHelper.DefaultVisitorError)
                       + $"{body}";
                err = true;
                errorCode = ErrorAppIsUnhealthy;
            }
            #endregion

            #region try to render the block or generate the error message

            if (body == null)
                try
                {
                    if (Block.View != null) // when a content block is still new, there is no definition yet
                    {
                        l.A("standard case, found template, will render");
                        var engine = GetEngine();
                        var renderEngineResult = engine.Render(specs);
                        body = renderEngineResult.Html;
                        if (renderEngineResult.ExceptionsOrNull != null)
                            exceptions.AddRange(renderEngineResult.ExceptionsOrNull);
                        errorCode = renderEngineResult.ErrorCode ?? errorCode;
                        if (errorCode == null && body?.Contains(ErrorHtmlMarker) == true) 
                            errorCode = ErrorGeneral;

                        var pageServiceShared = ((ContextOfBlock)Block.Context).PageServiceShared;

                        // Activate-js-api is true, if the html has some <script> tags which tell it to load the 2sxc
                        // only set if true, because otherwise we may accidentally overwrite the previous setting
                        if (renderEngineResult.ActivateJsApi)
                        {
                            l.A("template referenced 2sxc.api JS in script-tag: will enable");
                            pageServiceShared.PageFeatures.Activate(SxcPageFeatures.JsCore.NameId);
                        }

                        // Put all assets into the global page service for final processing later on
                        pageServiceShared.AddAssets(renderEngineResult);
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
            if (licenseNotOk != null)
                body = licenseNotOk + body;

            #region Wrap it all up into a nice wrapper tag

            // Figure out some if we should add the edit context
            // by default the editors will get it
            // in special cases the razor requests it to added as well
            var addEditCtx = Block.Context.Permissions.IsContentAdmin;
            var addJsApiOnly = false;
            if (!addEditCtx && Block.BlockFeatureKeys.Any())
            {
                var features = Block.BlockFeatures(Log);
                addEditCtx = features.Contains(SxcPageFeatures.ContextModule);
                addJsApiOnly = features.Contains(SxcPageFeatures.JsApiOnModule);
            }

            #region Add Custom Tags to the end if provided by the ModuleService - like TurnOn - not ideal yet

            // This is not ideal, because it actually changes what's in the DIV
            // We would rather add it to the end, but ATM that doesn't trigger turnOn in AJAX reload
            // Note: DNN implementation will ignore the module ID, but Oqtane needs it
            var additionalTags = Services.ModuleService.GetMoreTagsAndFlush(Block.Context.Module.Id);

            var bodyWithAddOns = additionalTags.Any()
                ? body + "\n" + string.Join("\n", additionalTags.Select(t => t?.ToString()))
                : body;

            #endregion

            var stats = new RenderStatistics
            {
                RenderMs = (int)l.Timer.ElapsedMilliseconds,
                UseLightSpeed = specs.UseLightspeed,
            };

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
        if (InstallationOk)
            return (null, false);

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
        if (AnyLicenseOk)
            return null;

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
        if (_engine != null)
            return l.Return(_engine, "cached");
        // edge case: view hasn't been built/configured yet, so no engine to find/attach
        if (Block.View == null)
            return l.ReturnNull("no view");
        _engine = Services.EngineFactory.CreateEngine(Block.View);
        _engine.Init(Block);
        return l.Return(_engine, "created");
    }
    private IEngine _engine;

}