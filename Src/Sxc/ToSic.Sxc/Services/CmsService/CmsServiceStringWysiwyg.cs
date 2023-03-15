using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Utils;
using ToSic.Sxc.Web.PageFeatures;
using static ToSic.Sxc.Blocks.RenderService;

namespace ToSic.Sxc.Services.CmsService
{
    public class CmsServiceStringWysiwyg: ServiceForDynamicCode
    {

        #region Constructor / DI

        private readonly CmsServiceImageExtractor _imageExtractor;

        public CmsServiceStringWysiwyg(
            CmsServiceImageExtractor imageExtractor
            ) : base("Cms.StrWys")
        {
            ConnectServices(
                _imageExtractor = imageExtractor
            );
        }
        private ServiceKit14 ServiceKit => _svcKit.Get(() => _DynCodeRoot.GetKit<ServiceKit14>());
        private readonly GetOnce<ServiceKit14> _svcKit = new GetOnce<ServiceKit14>();

        #endregion

        #region Init

        public CmsServiceStringWysiwyg Init(IDynamicField field, IContentType contentType, IContentTypeAttribute attribute, bool debug)
        {
            Field = field;
            ContentType = contentType;
            Attribute = attribute;
            Debug = debug;
            return this;
        }

        protected IDynamicField Field;
        protected IContentType ContentType;
        protected IContentTypeAttribute Attribute;
        protected bool Debug;

        #endregion

        internal const string WysiwygContainerClass = "wysiwyg-container";
        internal const string WysiwygDebugClass = "wysiwyg-debug";
        private const string WysiwygCssPrefix = "wysiwyg";

        internal CmsProcessed Process() => Log.Func(l =>
        {
            var html = Field.Raw as string;
            if (string.IsNullOrWhiteSpace(html))
                return (new CmsProcessed(false, null, null), "no html, treat as unknown, return null to let parent do wrapping with original");

            // 1. We got HTML, so first we must ensure the feature is activated
            ServiceKit.Page.Activate(BuiltInFeatures.CmsWysiwyg.NameId);

            // 2. Check Inner Content
            html = ProcessInnerContent(html);

            // prepare classes to add
            var classes = WysiwygContainerClass + (Debug ? " " + "wysiwyg-debug" : "");

            // 3. Check Responsive Images
            // extract img tags from html using regex case insensitive
            // and check if we have an img tags with data-cmsid="file:..." attributes
            var imgTags = RegexUtil.ImagesDetection.Matches(html);
            if (imgTags.Count == 0)
                return (new CmsProcessed(false, html, classes), "can't find img tags with data-cmsid, return HTML so classes are added");

            foreach (var imgTag in imgTags)
            {
                var originalImgTag = imgTag.ToString();

                var parts = _imageExtractor.ExtractProperties(originalImgTag);

                // use the IImageService to create Picture tags for it
                var picture = ServiceKit.Image.Picture(link: parts.src, factor: parts.factor, width: parts.width, imgAlt: parts.imgAlt,
                    imgClass: parts.imgClasses, picClass: parts.picClasses);

                // re-attach an alt-attribute, class etc. from the original if it had it
                // TODO: @2DM - this could fail because of fluid API - picture.img isn't updated
                var newImg = parts.otherAttributes.Aggregate(picture.Img, (img, attr) => img.Attr(attr.Key, attr.Value));

                // replace the old img tag with the new one
                html = html.Replace(originalImgTag, picture.ToString());
            }

            // reconstruct the original html and return wrapped in the realContainer
            return (new CmsProcessed(true, html, classes), "wysiwyg changed");
        });
        

        private string ProcessInnerContent(string html) => Log.Func(() =>
        {
            // Sort attributes in the order they will be in
            var sortedFields = ContentType.Attributes.OrderBy(a => a.SortOrder).ToList();
            var index = sortedFields.IndexOf(Attribute);
            if (index == -1 || sortedFields.Count <= index + 1)
                return (html, "can't check next attribute for content-blocks");

            var nextField = sortedFields[index + 1];
            var nextIsEntityField = nextField.Type == ValueTypes.Entity;
            var nextInputType = nextField.InputType();
            var nextHasContentBlocks = nextInputType.EqualsInsensitive(InputTypeForContentBlocksField);
            
            // TODO: ACTIVATE RENDER
            if (!nextIsEntityField || !nextHasContentBlocks)
                return (html, "next field is not content-block");

            html = ServiceKit.Render
                .All(Field.Parent as DynamicEntity, field: nextField.Name, merge: html)
                .ToString();

            return (html, "ok");
        });
    }
}
