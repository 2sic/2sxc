using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services.CmsService.Internal;

internal class HtmlInnerContentHelper()
    : ServiceForDynamicCode("Cms.StrWys", connect: [])
{
    private IRenderService RenderService => field ??= _CodeApiSvc.GetService<IRenderService>(reuse: true);

    public string ProcessInnerContent(string html, IContentType contentType, IContentTypeAttribute attribute, IField field)
    {
        var l = Log.Fn<string>();

        // Find out if "next" field has inner-content. For that, sort attributes in the order they will be in
        var sortedFields = contentType.Attributes
            .OrderBy(a => a.SortOrder)
            .ToList();
        var index = sortedFields.IndexOf(attribute);
        if (index == -1 || sortedFields.Count <= index + 1)
            return l.Return(html, "can't check next attribute for content-blocks");

        var nextField = sortedFields[index + 1];
        var nextIsEntityField = nextField.Type == ValueTypes.Entity;
        var nextInputType = nextField.InputType();
        var nextHasContentBlocks = nextInputType.EqualsInsensitive(RenderConstants.InputTypeForContentBlocksField);

        // Next ist not inner content, exit early
        if (!nextIsEntityField || !nextHasContentBlocks)
            return l.Return(html, "no inner content; next field is not content-block");

        html = RenderService
            .All(field.Parent, field: nextField.Name, merge: html)
            .ToString();

        return l.ReturnAsOk(html);
    }
}