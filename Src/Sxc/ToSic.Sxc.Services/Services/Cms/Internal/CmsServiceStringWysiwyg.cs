using System.Text.RegularExpressions;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Images.Sys;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.HtmlParsing;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.CmsService.Internal;

internal class CmsServiceStringWysiwyg()
    : ServiceWithContext("Cms.StrWys", connect: [])
{
    #region Sub-Services which should come from the same Code context

    [field: AllowNull, MaybeNull]
    private IPageService PageService => field ??= ExCtx.GetService<IPageService>(reuse: true);

    [field: AllowNull, MaybeNull]
    private HtmlImgToPictureHelper HtmlImgToPictureHelper => field ??= ExCtx.GetService<HtmlImgToPictureHelper>();

    [field: AllowNull, MaybeNull]
    private HtmlInnerContentHelper HtmlInnerContentHelper => field ??= ExCtx.GetService<HtmlInnerContentHelper>();

    #endregion

    #region Init

    public CmsServiceStringWysiwyg Init(IField field, IContentType contentType, IContentTypeAttribute attribute, IFolder folder, bool debug, object? imageSettings)
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
    protected IField Field = null!;
    /// <summary>FYI: is never allowed to be null.</summary>
    protected IContentType ContentType = null!;
    /// <summary>FYI: is never allowed to be null.</summary>
    protected IContentTypeAttribute Attribute = null!;
    /// <summary>FYI: could be null.</summary>
    protected object? ImageSettings;
    /// <summary>FYI: is never allowed to be null.</summary>
    protected IFolder Folder = null!;

    #endregion


    /// <summary>
    /// Note: very expressive name for logs
    /// </summary>
    /// <param name="value"></param>
    internal CmsProcessed HtmlForStringAndWysiwyg(string? value)
    {
        var l = Log.Fn<CmsProcessed>();
        var html = value ?? Field.Raw as string;
        if (html.IsEmptyOrWs())
            return l.Return(new(false, null, null), "no html, treat as unknown, return null to let parent do wrapping with original");

        // 1. We got HTML, so first we must ensure the feature is activated
        PageService.Activate(SxcPageFeatures.CmsWysiwyg.NameId);

        // 2. Check Inner Content
        html = HtmlInnerContentHelper.ProcessInnerContent(html, ContentType, Attribute, Field);

        // prepare classes to add
        var classes = WysiwygConstants.WysiwygContainerClass + (Debug ? $" {WysiwygConstants.WysiwygDebugClass}" : "");

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

            var picture = HtmlImgToPictureHelper.ConvertImgToPicture(originalImgTag, Folder, defaultImageSettings);

            // replace the old img tag with the new one
            html = html.Replace(originalImgTag, picture.ToString());
        }

        // reconstruct the original html and return wrapped in the realContainer
        return l.Return(new(true, html, classes), "wysiwyg changed with images");
    }

}