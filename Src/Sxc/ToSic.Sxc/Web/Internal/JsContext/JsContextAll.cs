using System.Text.Json.Serialization;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Data.Shared;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Internal.JsContextEdit;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Web.Internal.JsContext;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsContextAll : ServiceBase
{
    private readonly CodeInfosInScope _codeWarnings;
    public JsContextEnvironment Environment;
    public JsContextUser User;
    public JsContextLanguage Language;
        
    [JsonPropertyName("contentBlockReference")]
    public ContentBlockReferenceDto ContentBlockReference; // todo: still not sure if these should be separate...
        
    [JsonPropertyName("contentBlock")]
    public ContentBlockDto ContentBlock;
    // ReSharper disable once InconsistentNaming
    public ErrorDto error;
    public UiDto Ui;
    public JsApi JsApi;

    public JsContextAll(JsContextLanguage jsLangCtx, IJsApiService jsApiService, CodeInfosInScope codeWarnings) : base("Sxc.CliInf")
    {
        ConnectServices(
            _jsLangCtx = jsLangCtx,
            _jsApiService = jsApiService,
            _codeWarnings = codeWarnings
        );
    }

    private readonly JsContextLanguage _jsLangCtx;
    private readonly IJsApiService _jsApiService;

    public JsContextAll GetJsContext(string systemRootUrl, IBlock block, string errorCode, List<Exception> exsOrNull,
        RenderStatistics statistics)
    {
        var l = Log.Fn<JsContextAll>();
        var ctx = block.Context;

        Environment = new(systemRootUrl, ctx);
        Language = _jsLangCtx.Init(ctx.Site);

        // New in v13 - if the view is from remote, don't allow design
        var blockCanDesign = block.View?.Entity.HasAncestor() ?? false ? (bool?)false : null;

        User = new(ctx.User);

        ContentBlockReference = new(block, ctx.Publishing.Mode);
        ContentBlock = new(block, statistics);

        // If auto toolbar is false / not certain, and we have features activated...
        // find out if the Toolbars-Auto is enabled, in which case we should activate them
        var autoToolbar = ctx.UserMayEdit || (
            block.BlockFeatureKeys.Any() && block.Context.PageServiceShared.PageFeatures
                .GetWithDependents(block.BlockFeatureKeys, Log)
                .Contains(SxcPageFeatures.ToolbarsAutoInternal)
        );

        l.A($"{nameof(autoToolbar)}: {autoToolbar}");
        Ui = new(autoToolbar);
        JsApi = _jsApiService.GetJsApi(pageId: Environment.PageId,
            siteRoot: null,
            rvt: null
        );

        error = new(block, errorCode, exsOrNull, _codeWarnings);
        return l.Return(this);
    }
}