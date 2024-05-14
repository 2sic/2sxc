using System.Text.Json;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Serialization;
using ToSic.Razor.Markup;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.EditService;

partial class EditService
{
    #region Context Attributes

    /// <inheritdoc/>
    public IRawHtmlString ContextAttributes(ICanBeEntity target,
        NoParamOrder noParamOrder = default,
        string field = null,
        string contentType = null,
        Guid? newGuid = null,
        string apps = null,
        int max = 100)
    {
        Log.A("ctx attribs - enabled:{Enabled}");
        if (!Enabled) return null;

        if (field == null) throw new("need parameter 'field'");

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

        return Build.Attribute(innerContentAttribute, serialized);
    }

    // 2024-01-10 2dm disabled #WrapInContext - was for internal only, seems not to be used? Was created 2018? https://github.com/2sic/2sxc/issues/1479
    ///// <inheritdoc/>
    //[PrivateApi]
    //public IRawHtmlString WrapInContext(object content,
    //    NoParamOrder noParamOrder = default,
    //    string tag = SxcUiConstants.DefaultContextTag,
    //    bool full = false,
    //    bool? enableEdit = null,
    //    int instanceId = 0,
    //    int contentBlockId = 0
    //)
    //{
    //    var renderingHelper = _renderHelper.Value;

    //    return new RawHtmlString(
    //        renderingHelper.WrapInContext(content.ToString(),
    //            instanceId: instanceId > 0
    //                ? instanceId
    //                : Block.ParentId,
    //            contentBlockId: contentBlockId > 0
    //                ? contentBlockId
    //                : Block.ContentBlockId,
    //            editContext: enableEdit ?? Enabled)
    //    );
    //}

    #endregion Context Attributes
}