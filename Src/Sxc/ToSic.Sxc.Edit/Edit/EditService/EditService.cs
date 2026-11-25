using ToSic.Razor.Markup;
using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Sys.Html;

namespace ToSic.Sxc.Edit.EditService;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal partial class EditService(Ctx<ICodeContext> ctxSvcProvider, IJsonService jsonService)
    : ServiceBase("Sxc.Edit", connect: [jsonService]), IEditService
{
    #region Field-helper

    /// <inheritdoc/>
    public IRawHtmlString? Attribute(string name, string value)
        => !Enabled ? null : HtmlAttribute.Create(name, value);

    /// <inheritdoc/>
    public IRawHtmlString? Attribute(string name, object value)
        => !Enabled ? null : HtmlAttribute.Create(name, jsonService.ToJson(value));

    #endregion Field Helper

}