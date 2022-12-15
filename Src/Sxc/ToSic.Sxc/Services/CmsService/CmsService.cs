using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Utils;

namespace ToSic.Sxc.Services.CmsService
{
    [PrivateApi("WIP")]
    public class CmsService: HasLog, ICmsService
    {
        private readonly Lazy<IImageService> _imgService;
        private readonly Lazy<IValueConverter> _valueConverter;

        public CmsService(Lazy<IImageService> imgService, Lazy<IValueConverter> valueConverter) : base(Constants.SxcLogName + ".CmsSrv")
        {
            _imgService = imgService;
            _valueConverter = valueConverter;
        }

        public IHtmlTag Show(object thing, string noParamOrder = Eav.Parameters.Protector, object container = null)
        {
            var l = Log.Fn<IHtmlTag>();

            // Prepare the real container
            var realContainer = GetContainer(container);

            // If it's not a field, we cannot find out more about the object
            // In that case, just wrap the result in the container and return it
            if (!(thing is IDynamicField field))
                return l.Return(realContainer.Wrap(thing), "No field, will just treat as value");

            // Get Content type and field information
            var contentType = field.Parent.Entity.Type;
            if (contentType == null)
                return l.Return(realContainer.Wrap(thing), "can't find content-type, treat as value");

            var attribute = contentType[field.Name];
            if (attribute == null) 
                return l.Return(realContainer.Wrap(thing), "no attribute info, treat as value");

            // Now we handle all kinds of known special treatments
            // Start with strings...
            if (attribute.ControlledType == ValueTypes.String)
            {
                // ...wysiwyg
                if (attribute.InputType() == InputTypes.InputTypeWysiwyg)
                {
                    var html = field.Raw;
                    if (string.IsNullOrWhiteSpace(html))
                        return l.Return(realContainer.Wrap(thing), "no html, treat as value");

                    // extract img tags from html using regex case insensitive
                    // and check if we have an img tags with data-cmsid="file:..." attributes
                    var imgTags = RegexUtil.ImagesDetection.Matches(html);
                    // if not, return wrapped
                    if (imgTags.Count == 0) l.Return(realContainer.Wrap(thing), "can't find img tags with data-cmsid");

                    foreach (var imgTag in imgTags)
                    {
                        var oldImgTag = imgTag.ToString();

                        #region empty picture parameters

                        object link = null;
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
                                    link = _valueConverter.Value.ToValue(value); // convert 'file:22' to real value 'folder/image.png'
                                    break;
                                case "src":
                                    if (link == null) link = value; // should not overwrite data-cmsid
                                    break;
                                case "width":
                                    width = value;
                                    break;
                                case "alt":
                                    imgAlt = value;
                                    break;
                                case "class": // specially look at the classes
                                    factor = GetFactor(value); // use the "#/#" as the `factor` parameter
                                    imgClass = value; // add it as class
                                    break;
                                default:
                                    // store alt-attribute, class etc. from the original if it had it (to re-attach latter)
                                    otherAttributes[key] = string.IsNullOrEmpty(value) ? null : value;
                                    break;
                            }
                        }
                        // use the IImageService to create Picture tags for it
                        var picture = _imgService.Value.Picture(link: link, factor: factor, width: width, imgAlt: imgAlt, imgClass: imgClass);

                        // re-attach an alt-attribute, class etc. from the original if it had it
                        var img = picture.Img;
                        foreach (var otherAttribute in otherAttributes)
                            img = img.Attr(otherAttribute.Key, otherAttribute.Value);

                        // replace the old img tag with the new one
                        html = html.Replace(oldImgTag, picture.ToString());
                    }

                    // reconstruct the original html and return wrapped in the realContainer
                    return l.Return(realContainer.Wrap(html), "wysiwyg");
                }
            }


            // Fallback...
            return l.Return(realContainer.Wrap(thing), "nothing else hit, will treat as value");
        }

        public static string GetFactor(string value)
        {
            // check if we can find something like "wysiwyg-width#of#" - this is for resize ratios
            var widthMatch = RegexUtil.WysiwygWidthNumDetection.Match(value);
            // convert to a format like "#/#"
            return widthMatch.Success ? $"{widthMatch.Groups["num"].Value}/{widthMatch.Groups["all"].Value}" : null;
        }

        private IHtmlTag GetContainer(object container)
        {
            var l = Log.Fn<IHtmlTag>();
            // Already an ITag
            if (container is IHtmlTag iTagContainer) return l.Return(iTagContainer, "container is Blade tag");

            if (container is string tagName)
            {
                if (!tagName.IsEmpty() && !tagName.Contains(" "))
                    return l.Return(Tag.Custom(tagName), "was a tag name, created tag");
                throw new ArgumentException("Must be a tag name like 'div' or a RazorBlade Html Tag object",
                    nameof(container));
            }

            // Nothing to do, just return an empty tag which can be filled...
            return l.Return(Tag.RawHtml(), "no container, return empty tag");
        }

    }
}
