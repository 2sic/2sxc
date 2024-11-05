using System.Text.RegularExpressions;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.HtmlParsing;
using ToSic.Sxc.Web.Internal.PageFeatures;
using static ToSic.Sxc.Blocks.Internal.Render.RenderService;

namespace ToSic.Sxc.Services.CmsService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsServiceStringWysiwyg(CmsServiceImageExtractor imageExtractor)
    : ServiceForDynamicCode("Cms.StrWys", connect: [imageExtractor])
{

    private ServiceKit14 ServiceKit => _svcKit ??= _CodeApiSvc.GetKit<ServiceKit14>();
    private ServiceKit14 _svcKit;

    #region Init

    public CmsServiceStringWysiwyg Init(IField field, IContentType contentType, IContentTypeAttribute attribute, IFolder folder, bool debug, object imageSettings)
    {
        var l = Log.Fn<CmsServiceStringWysiwyg>();
        Field = field;
        ContentType = contentType;
        Folder = folder;
        Attribute = attribute;
        Debug = debug;
        ImageSettings = imageSettings;
        return l.ReturnAsOk(this);
    }

    /// <summary>FYI: is never allowed to be null.</summary>
    protected IField Field;
    /// <summary>FYI: is never allowed to be null.</summary>
    protected IContentType ContentType;
    /// <summary>FYI: is never allowed to be null.</summary>
    protected IContentTypeAttribute Attribute;
    /// <summary>FYI: could be null.</summary>
    protected object ImageSettings;
    /// <summary>FYI: is never allowed to be null.</summary>
    protected IFolder Folder;

    #endregion

    /// <summary>
    /// The container Class - must usually be assigned, so that CSS inside it works
    /// </summary>
    internal const string WysiwygContainerClass = "wysiwyg-container";

    /// <summary>
    /// Debug class to show debug borders etc. with CSS
    /// </summary>
    internal const string WysiwygDebugClass = "wysiwyg-debug";
    private const string WysiwygCssPrefix = "wysiwyg";  // not used ATM

    /// <summary>
    /// Note: very expressive name for logs
    /// </summary>
    /// <param name="value"></param>
    internal CmsProcessed HtmlForStringAndWysiwyg(string value)
    {
        var l = Log.Fn<CmsProcessed>();
        var html = value ?? Field.Raw as string;
        if (string.IsNullOrWhiteSpace(html))
            return l.Return(new(false, null, null), "no html, treat as unknown, return null to let parent do wrapping with original");

        // 1. We got HTML, so first we must ensure the feature is activated
        ServiceKit.Page.Activate(SxcPageFeatures.CmsWysiwyg.NameId);

        // 2. Check Inner Content
        html = ProcessInnerContent(html);

        // prepare classes to add
        var classes = WysiwygContainerClass + (Debug ? $" {WysiwygDebugClass}" : "");

        // 3. Check Responsive Images
        // extract img tags from html using regex case insensitive
        // and check if we have an img tags with data-cmsid="file:..." attributes
        var imgTags = RegexUtil.ImagesDetection.Value.Matches(html);
        if (imgTags.Count == 0)
            return l.Return(new(true, html, classes), "can't find img tags with data-cmsid, done");

        // check if field metadata specifies alternate Lightbox or image resize settings

        var fieldMd = ImageDecorator.GetOrNull(Attribute, [null]);

        // Assume fallback-image settings to be the specified or "Wysiwyg"
        var defaultImageSettings = fieldMd?.ResizeSettings ?? ImageSettings ?? "Wysiwyg";

        l.A($"Found {imgTags.Count} images to process with default {defaultImageSettings}");

        foreach (var imgTag in imgTags.Cast<Match>())
        {
            var originalImgTag = imgTag.ToString();

            var imgProps = imageExtractor.ExtractImageProperties(originalImgTag, Field.Parent.Guid, Folder);

            // if we have a real file, pre-get the inner parameters as we would want to use it for resize-settings
            var preparedImgParams = imgProps.File.NullOrGetWith(ResponsiveSpecsOfTarget.ExtractSpecs);

            // if the file itself specifies a resize settings, use it, otherwise use the default settings
            var imgSettings = preparedImgParams?.ImgDecoratorOrNull?.ResizeSettings ?? defaultImageSettings;

            // In most cases use the preparedImgParams, but if it's null, use the src attribute
            var target = (object)preparedImgParams ?? imgProps.Src;

            // use the IImageService to create Picture tags for it
            var picture = ServiceKit.Image.Picture(link: target, settings: imgSettings, factor: imgProps.Factor, width: imgProps.Width,
                imgAlt: imgProps.ImgAlt, imgClass: imgProps.ImgClasses, imgAttributes: imgProps.OtherAttributes,
                pictureClass: imgProps.PicClasses);

            // replace the old img tag with the new one
            html = html.Replace(originalImgTag, picture.ToString());
        }

        // reconstruct the original html and return wrapped in the realContainer
        return l.Return(new(true, html, classes), "wysiwyg changed with images");
    }

    private string ProcessInnerContent(string html) 
    {
        var l = Log.Fn<string>();
        // Sort attributes in the order they will be in
        var sortedFields = ContentType.Attributes.OrderBy(a => a.SortOrder).ToList();
        var index = sortedFields.IndexOf(Attribute);
        if (index == -1 || sortedFields.Count <= index + 1)
            return l.Return(html, "can't check next attribute for content-blocks");

        var nextField = sortedFields[index + 1];
        var nextIsEntityField = nextField.Type == ValueTypes.Entity;
        var nextInputType = nextField.InputType();
        var nextHasContentBlocks = nextInputType.EqualsInsensitive(InputTypeForContentBlocksField);
            
        if (!nextIsEntityField || !nextHasContentBlocks)
            return l.Return(html, "no inner content; next field is not content-block");

        html = ServiceKit.Render
            .All(Field.Parent, field: nextField.Name, merge: html)
            .ToString();

        return l.ReturnAsOk(html);
    }
}