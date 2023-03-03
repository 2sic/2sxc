using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
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

        private readonly LazySvc<IValueConverter> _valueConverter;

        public CmsServiceStringWysiwyg(
            LazySvc<IValueConverter> valueConverter
            ) : base("Cms.StrWys")
        {
            ConnectServices(
                _valueConverter = valueConverter
            );
        }
        private ServiceKit14 ServiceKit => _svcKit.Get(() => _DynCodeRoot.GetKit<ServiceKit14>());
        private readonly GetOnce<ServiceKit14> _svcKit = new GetOnce<ServiceKit14>();

        #endregion

        #region Init

        public CmsServiceStringWysiwyg Init(IDynamicField field, IContentType contentType, IContentTypeAttribute attribute)
        {
            Field = field;
            ContentType = contentType;
            Attribute = attribute;
            return this;
        }

        protected IDynamicField Field;
        protected IContentType ContentType;
        protected IContentTypeAttribute Attribute;

        #endregion

        internal const string WysiwygClassToAdd = "wysiwyg";
        private const string WysiwygCssPrefix = "wysiwyg";

        internal string Process() => Log.Func(l =>
        {
            var html = Field.Raw as string;
            if (string.IsNullOrWhiteSpace(html))
                return (null, "no html, treat as unknown, return null to let parent do wrapping with original");

            // 1. We got HTML, so first we must ensure the feature is activated
            ServiceKit.Page.Activate(BuiltInFeatures.CmsWysiwyg.NameId);

            // 2. Check Inner Content
            html = ProcessInnerContent(html);

            // 3. Check Responsive Images
            // extract img tags from html using regex case insensitive
            // and check if we have an img tags with data-cmsid="file:..." attributes
            var imgTags = RegexUtil.ImagesDetection.Matches(html);
            if (imgTags.Count == 0)
                return (html, "can't find img tags with data-cmsid, return HTML so classes are added");

            foreach (var imgTag in imgTags)
            {
                var oldImgTag = imgTag.ToString();

                #region empty picture parameters

                string src = null;
                string factor = null;
                object width = default;
                string imgAlt = null;
                string imgClass = null;
                var otherAttributes = new Dictionary<string, string>();

                #endregion


                // get all attributes
                var attributes = RegexUtil.AttributesDetection.Matches(oldImgTag);
                foreach (Match attributeMatch in attributes)
                {
                    var key = attributeMatch.Groups["Key"].Value;
                    var value = attributeMatch.Groups["Value"].Value;
                    switch (key.ToLowerInvariant())
                    {
                        case "data-cmsid":
                            src = _valueConverter.Value.ToValue(value); // convert 'file:22' to real value 'folder/image.png'
                            break;
                        case "src":
                            if (src == null) src = src ?? value; // should not overwrite data-cmsid
                            break;
                        case "width":
                            width = value;
                            break;
                        case "alt":
                            imgAlt = value;
                            break;
                        case "class": // specially look at the classes
                            factor = GetImgServiceResizeFactor(value); // use the "#/#" as the `factor` parameter
                            imgClass = value; // add it as class
                            break;
                        default:
                            // store alt-attribute, class etc. from the original if it had it (to re-attach latter)
                            otherAttributes[key] = string.IsNullOrEmpty(value) ? null : value;
                            break;
                    }
                }

                // use the IImageService to create Picture tags for it
                var picture = ServiceKit.Image.Picture(link: src, factor: factor, width: width, imgAlt: imgAlt,
                    imgClass: imgClass);

                // re-attach an alt-attribute, class etc. from the original if it had it
                // TODO: @2DM - this could fail because of fluid API - picture.img isn't updated
                var newImg = otherAttributes.Aggregate(picture.Img, (img, attr) => img.Attr(attr.Key, attr.Value));

                // replace the old img tag with the new one
                html = html.Replace(oldImgTag, picture.ToString());
            }

            // reconstruct the original html and return wrapped in the realContainer
            return (html, "wysiwyg changed");
        });

        private string ProcessInnerContent(string html) => Log.Func(() =>
        {
            // Sort attributes in the order they will be in
            var sortedFields = ContentType.Attributes.OrderBy(a => a.SortOrder).ToList();
            var index = sortedFields.IndexOf(Attribute);
            if (index == -1 || sortedFields.Count < index)
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


        public static string GetImgServiceResizeFactor(string value)
        {
            // check if we can find something like "wysiwyg-width#of#" - this is for resize ratios
            var widthMatch = RegexUtil.WysiwygWidthNumDetection.Match(value);
            // convert to a format like "#/#"
            return widthMatch.Success ? $"{widthMatch.Groups["num"].Value}/{widthMatch.Groups["all"].Value}" : null;
        }

    }
}
