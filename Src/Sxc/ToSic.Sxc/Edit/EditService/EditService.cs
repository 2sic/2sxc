using ToSic.Lib.DI;
using ToSic.Razor.Markup;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.EditService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class EditService : ServiceForDynamicCode, IEditService
{

    public EditService(IJsonService jsonService/*, LazySvc<IRenderingHelper> renderHelper*/) : base("Sxc.Edit")
    {
        ConnectServices(
            _jsonService = jsonService
            // 2024-01-10 2dm disabled #WrapInContext - was for internal only, seems not to be used? Was created 2018? https://github.com/2sic/2sxc/issues/1479
            //_renderHelper = renderHelper.SetInit(h => h.Init(Block))
        );
    }
    private readonly IJsonService _jsonService;
    // 2024-01-10 2dm disabled #WrapInContext - was for internal only, seems not to be used? Was created 2018? https://github.com/2sic/2sxc/issues/1479
    //private readonly LazySvc<IRenderingHelper> _renderHelper;

    public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
    {
        base.ConnectToRoot(codeRoot);
        SetBlock(codeRoot, ((IDynamicCodeRootInternal)codeRoot)._Block);
    }

    internal IEditService SetBlock(IDynamicCodeRoot codeRoot, IBlock block)
    {
        Block = block;
        var user = codeRoot?.CmsContext?.User;
        Enabled = Block?.Context.UserMayEdit ?? (user?.IsSiteAdmin ?? false) || (user?.IsSystemAdmin ?? false);
        return this;
    }

    protected IBlock Block;

    #region Attribute-helper

    /// <inheritdoc/>
    public IRawHtmlString Attribute(string name, string value)
        => !Enabled ? null : Build.Attribute(name, value);

    /// <inheritdoc/>
    public IRawHtmlString Attribute(string name, object value)
        => !Enabled ? null : Build.Attribute(name, _jsonService.ToJson(value));

    #endregion Attribute Helper

}