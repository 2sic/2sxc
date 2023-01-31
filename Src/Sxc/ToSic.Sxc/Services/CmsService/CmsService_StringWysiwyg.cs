//using System.Collections.Generic;
//using System.Linq;
//using System.Text.RegularExpressions;
//using ToSic.Eav.Data;
//using ToSic.Lib.Logging;
//using ToSic.Sxc.Blocks;
//using ToSic.Sxc.Data;
//using ToSic.Sxc.Utils;
//using ToSic.Sxc.Web.PageFeatures;

//namespace ToSic.Sxc.Services.CmsService
//{
//    public partial class CmsService
//    {
//        private const string WysiwygClassToAdd = "wysiwyg";
//        private const string WysiwygCssPrefix = "wysiwyg";

//        private string StringWysiwyg(IDynamicField field, IContentType contentType, IContentTypeAttribute attribute) => Log.Func(l =>
//        {
//            var html = field.Raw as string;
//            if (string.IsNullOrWhiteSpace(html))
//                return (null, "no html, treat as unknown, return null to let parent do wrapping");

//            // We got HTML, so first we must ensure the feature is activated
//            _pageService.Value.Activate(BuiltInFeatures.CmsWysiwyg.NameId);


//            #region Check if we have inner content

//            // Sort attributes in the order they will be in
//            var allAttributes = contentType.Attributes.OrderBy(a => a.SortOrder);
//            var locInAttributes = contentType.Attributes.IndexOf(attribute);
//            if (locInAttributes != -1 && contentType.Attributes.Count >= locInAttributes)
//            {
//                var possibleInnerContent = contentType.Attributes[locInAttributes + 1];
//                var isEntityField = possibleInnerContent.ControlledType == ValueTypes.Entity;
//                // TODO: CHECK IF INNER-CONTENT FIELD
//                var inputType = possibleInnerContent.InputType();
//                var isInnerContentField = inputType.Contains(RenderService.InputTypeForContentBlocksField) || true; // TODO
//                // TODO: ACTIVATE RENDER
//                if (isEntityField && isInnerContentField)
//                    html = _renderService.Value.All(field.Parent as DynamicEntity, field: possibleInnerContent.Name,
//                        merge: html).ToString();
//            }

//            #endregion

//            // extract img tags from html using regex case insensitive
//            // and check if we have an img tags with data-cmsid="file:..." attributes
//            var imgTags = RegexUtil.ImagesDetection.Matches(html);
//            // if not, return wrapped
//            if (imgTags.Count == 0)
//                return (html, "can't find img tags with data-cmsid, return HTML so classes are added");

//            foreach (var imgTag in imgTags)
//            {
//                var oldImgTag = imgTag.ToString();

//                #region empty picture parameters

//                object link = null;
//                string factor = null;
//                object width = default;
//                string imgAlt = null;
//                string imgClass = null;
//                var otherAttributes = new Dictionary<string, string>();

//                #endregion


//                // get all attributes
//                var attributes = RegexUtil.AttributesDetection.Matches(oldImgTag);
//                foreach (Match attributeMatch in attributes)
//                {
//                    var key = attributeMatch.Groups["Key"].Value;
//                    var value = attributeMatch.Groups["Value"].Value;
//                    switch (key.ToLowerInvariant())
//                    {
//                        case "data-cmsid":
//                            link = _valueConverter.Value.ToValue(value); // convert 'file:22' to real value 'folder/image.png'
//                            break;
//                        case "src":
//                            if (link == null) link = value; // should not overwrite data-cmsid
//                            break;
//                        case "width":
//                            width = value;
//                            break;
//                        case "alt":
//                            imgAlt = value;
//                            break;
//                        case "class": // specially look at the classes
//                            factor = GetImgServiceResizeFactor(value); // use the "#/#" as the `factor` parameter
//                            imgClass = value; // add it as class
//                            break;
//                        default:
//                            // store alt-attribute, class etc. from the original if it had it (to re-attach latter)
//                            otherAttributes[key] = string.IsNullOrEmpty(value) ? null : value;
//                            break;
//                    }
//                }

//                // use the IImageService to create Picture tags for it
//                var picture = _imgService.Value.Picture(link: link, factor: factor, width: width, imgAlt: imgAlt,
//                    imgClass: imgClass);

//                // re-attach an alt-attribute, class etc. from the original if it had it
//                // TODO: @2DM - this could fail because of fluid API - picture.img isn't updated
//                var newImg = otherAttributes.Aggregate(picture.Img, (img, attr) => img.Attr(attr.Key, attr.Value));

//                // replace the old img tag with the new one
//                html = html.Replace(oldImgTag, picture.ToString());
//            }

//            // reconstruct the original html and return wrapped in the realContainer
//            return (html, "wysiwyg changed");
//        });


//        public static string GetImgServiceResizeFactor(string value)
//        {
//            // check if we can find something like "wysiwyg-width#of#" - this is for resize ratios
//            var widthMatch = RegexUtil.WysiwygWidthNumDetection.Match(value);
//            // convert to a format like "#/#"
//            return widthMatch.Success ? $"{widthMatch.Groups["num"].Value}/{widthMatch.Groups["all"].Value}" : null;
//        }

//    }


//}
