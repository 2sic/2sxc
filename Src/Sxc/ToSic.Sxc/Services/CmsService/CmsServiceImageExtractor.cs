using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Utils;

namespace ToSic.Sxc.Services.CmsService
{
    public class CmsServiceImageExtractor
    {
        private readonly LazySvc<IValueConverter> _valueConverter;
        public CmsServiceImageExtractor(LazySvc<IValueConverter> valueConverter)
        {
            _valueConverter = valueConverter;
        }

        internal ImageProperties ExtractProperties(string oldImgTag)
        {
            string src = null;
            string factor = null;
            object width = default;
            string imgAlt = null;
            string imgClasses = null;
            string picClasses = null;
            var otherAttributes = new Dictionary<string, string>();

            // get all attributes
            var attributes = RegexUtil.AttributesDetection.Value.Matches(oldImgTag);
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
                        src = src ?? value; // should not overwrite data-cmsid
                        break;
                    case "width":
                        width = value;
                        break;
                    case "alt":
                        imgAlt = value;
                        break;
                    case "class": // specially look at the classes
                        factor = GetImgServiceResizeFactor(value); // use the "#/#" as the `factor` parameter
                        imgClasses = value; // add it as class
                        picClasses = GetPictureClasses(value);
                        break;
                    default:
                        // store alt-attribute, class etc. from the original if it had it (to re-attach latter)
                        otherAttributes[key] = value.NullIfNoValue();
                        break;
                }
            }

            return new ImageProperties() {Src = src, Factor = factor, ImgAlt = imgAlt, ImgClasses = imgClasses, PicClasses = picClasses, Width = width, OtherAttributes = otherAttributes};
        }

        public static string GetPictureClasses(string classes)
        {
            // TODO: filter to only return wysiwyg-* classes
            return classes;
        }

        public static string GetImgServiceResizeFactor(string value)
        {
            // check if we can find something like "wysiwyg-width#of#" - this is for resize ratios
            //var widthMatch = RegexUtil.WysiwygWidthNumDetection.Match(value);
            var widthMatch = RegexUtil.WysiwygWidthLazy.Value.Match(value);
            // convert to a format like "#/#"
            if (!widthMatch.Success) return null;
            var numString = widthMatch.Groups["percent"].Value;
            // We want to return a nice factor, in case the rules have optimized values
            switch (numString)
            {
                case "100": return "1";
                case "50": return "1/2";
                case "33": return "1/3";
                case "66": return "2/3";
                case "25": return "1/4";
                case "75": return "3/4";
                default: return numString;
            }
            //return widthMatch.Success ? $"{widthMatch.Groups["num"].Value}/{widthMatch.Groups["all"].Value}" : null;
        }

        internal class ImageProperties
        {
            public string Src;
            public string Factor;
            public string ImgAlt;
            public string ImgClasses;
            public string PicClasses;
            public object Width;
            public Dictionary<string, string> OtherAttributes;
        }
    }
}
