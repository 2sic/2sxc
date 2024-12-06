using System.Text.Json.Serialization;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Data.Shared;
using ToSic.Eav.Internal.Features;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Configuration.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Internal.JsContextEdit;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Web.Internal.JsContext;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsContextAll(JsContextLanguage jsLangCtxSvc, IJsApiService jsApiService, CodeInfosInScope codeWarnings, IAppJsonService appJson, Lazy<IFeaturesService> featuresSvc)
    : ServiceBase("Sxc.CliInf", connect: [jsLangCtxSvc, jsApiService, codeWarnings])
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsContextEnvironment Environment;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsContextUser User;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsContextLanguage Language;
        
    [JsonPropertyName("contentBlockReference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ContentBlockReferenceDto ContentBlockReference; // todo: still not sure if these should be separate...
        
    [JsonPropertyName("contentBlock")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ContentBlockDto ContentBlock;

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ErrorDto Error;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UiDto Ui;

    [JsonPropertyName("jsApi")]
    public JsApi JsApi;

    public JsContextAll GetJsApiOnly(IBlock block)
    {
        var l = Log.Fn<JsContextAll>();

        var addPublicKey = Features(block).Contains(SxcPageFeatures.EncryptFormData)
            && featuresSvc.Value.IsEnabled(SxcFeatures.NetworkDataEncryption.NameId);

        JsApi = jsApiService.GetJsApi(
            pageId: block.Context.Page.Id,
            siteRoot: null,
            rvt: null,
            withPublicKey: addPublicKey
        );
        return l.Return(this);
    }

    public JsContextAll GetJsContext(string systemRootUrl, IBlock block, string errorCode, List<Exception> exsOrNull,
        RenderStatistics statistics)
    {
        var l = Log.Fn<JsContextAll>();
        var ctx = block.Context;

        Environment = new(systemRootUrl, ctx);
        Language = jsLangCtxSvc.Init(ctx.Site);

        // New in v13 - if the view is from remote, don't allow design
        var blockCanDesign = block.View?.Entity.HasAncestor() ?? false
            ? (bool?)false
            : null;

        User = new(ctx.User, block.App?.Data?.List);

        ContentBlockReference = new(block, ctx.Publishing.Mode);
        ContentBlock = new(block, statistics, appJson);

        // If auto toolbar is false / not certain, and we have features activated...
        // find out if the Toolbars-Auto is enabled, in which case we should activate them
        var autoToolbar = ctx.Permissions.IsContentAdmin || Features(block).Contains(SxcPageFeatures.ToolbarsAutoInternal);

        l.A($"{nameof(autoToolbar)}: {autoToolbar}");

        Ui = new(autoToolbar);

        GetJsApiOnly(block);

        Error = new(block, errorCode, exsOrNull, codeWarnings);
        return l.Return(this);
    }

    private List<IPageFeature> Features(IBlock block) =>
        _pageFeatures ??= block.BlockFeatureKeys.Any()
            ? block.Context.PageServiceShared.PageFeatures.GetWithDependents(block.BlockFeatureKeys, Log)
            : [];

    private List<IPageFeature> _pageFeatures;
}