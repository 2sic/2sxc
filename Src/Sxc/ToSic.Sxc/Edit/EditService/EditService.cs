using ToSic.Razor.Markup;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.EditService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class EditService(IJsonService jsonService)
    : ServiceForDynamicCode("Sxc.Edit", connect: [jsonService]), IEditService
{
    // 2024-01-10 2dm disabled #WrapInContext - was for internal only, seems not to be used? Was created 2018? https://github.com/2sic/2sxc/issues/1479
    //_renderHelper = renderHelper.SetInit(h => h.Init(Block))

    // 2024-01-10 2dm disabled #WrapInContext - was for internal only, seems not to be used? Was created 2018? https://github.com/2sic/2sxc/issues/1479
    //private readonly LazySvc<IRenderingHelper> _renderHelper;

    public override void ConnectToRoot(ICodeApiService codeRoot)
    {
        base.ConnectToRoot(codeRoot);
        SetBlock(codeRoot, ((ICodeApiServiceInternal)codeRoot)._Block);
    }

    internal IEditService SetBlock(ICodeApiService codeRoot, IBlock block)
    {
        Block = block;
        var user = codeRoot?.CmsContext?.User;
        Enabled = Block?.Context.Permissions.IsContentAdmin ?? (user?.IsSiteAdmin ?? false);
        return this;
    }

    protected IBlock Block;

    #region Attribute-helper

    /// <inheritdoc/>
    public IRawHtmlString Attribute(string name, string value)
        => !Enabled ? null : Build.Attribute(name, value);

    /// <inheritdoc/>
    public IRawHtmlString Attribute(string name, object value)
        => !Enabled ? null : Build.Attribute(name, jsonService.ToJson(value));

    #endregion Attribute Helper

}