using ToSic.Razor.Markup;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Web.Sys.Html;

namespace ToSic.Sxc.Edit.EditService;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal partial class EditService(IJsonService jsonService)
    : ServiceWithContext("Sxc.Edit", connect: [jsonService]), IEditService, IEditServiceSetup
{
    public override void ConnectToRoot(IExecutionContext exCtx)
    {
        base.ConnectToRoot(exCtx);
        ((IEditServiceSetup)this).SetBlock(exCtx, exCtx.GetState<IBlock>());
    }

    IEditService IEditServiceSetup.SetBlock(IExecutionContext? exCtxOrNull, IBlock block)
    {
        var user = exCtxOrNull?.GetState<ICmsContext>()?.User;
        Enabled = block?.Context.Permissions.IsContentAdmin ?? (user?.IsSiteAdmin ?? false);
        return this;
    }

    #region Attribute-helper

    /// <inheritdoc/>
    public IRawHtmlString? Attribute(string name, string value)
        => !Enabled ? null : HtmlAttribute.Create(name, value);

    /// <inheritdoc/>
    public IRawHtmlString? Attribute(string name, object value)
        => !Enabled ? null : HtmlAttribute.Create(name, jsonService.ToJson(value));

    #endregion Attribute Helper

}