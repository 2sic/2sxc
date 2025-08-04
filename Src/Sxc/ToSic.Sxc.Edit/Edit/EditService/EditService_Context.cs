using System.Text.Json;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Razor.Markup;
using ToSic.Sxc.Web.Sys.Html;

namespace ToSic.Sxc.Edit.EditService;

partial class EditService
{
    #region Context Attributes

    /// <inheritdoc/>
    public IRawHtmlString? ContextAttributes(ICanBeEntity target,
        NoParamOrder noParamOrder = default,
        string? field = null,
        string? contentType = null,
        Guid? newGuid = null,
        string? apps = null,
        int max = 100)
    {
        var l = Log.Fn<IRawHtmlString>("ctx attribs - enabled:{Enabled}");
        if (!Enabled)
            return null;

        if (field == null)
            throw new("need parameter 'field'");

        var serialized = JsonSerializer.Serialize(new
        {
            apps,
            field,
            guid = newGuid.ToString(),
            max,
            parent = target.Entity.EntityId,
            parentGuid = target.Entity.EntityGuid,
            type = contentType ?? AppConstants.ContentGroupRefTypeName,
        }, JsonOptions.SafeJsonForHtmlAttributes);

        return l.Return(HtmlAttribute.Create(InnerContentAttribute, serialized));
    }

    #endregion Context Attributes
}