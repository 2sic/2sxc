using System;
using ToSic.Eav;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Services.Image
{
    public partial class ImageService: IImageService
    {
        public ITag SourceTags(string url, object settings = null, string noParamOrder = Parameters.Protector, object formats = null,
            object factor = null)
        {
            // Check formats
            var format = GetFormat(url);
            if (format == null || format.ResizeFormats.Count == 0) return Tag.Custom(null);

            // 1. Prepare Settings

            // 1.x. Settings

            // 1.x. Formats if not yet known

            // 1.x. Formats if known, verify its useful?

            // 1.x. Factor ?


            throw new NotImplementedException();
        }

        public object PictureTag(string url, object settings = null, string noParamOrder = Parameters.Protector, object factor = null,
            string alt = null, Action<string> img = null)
        {
            throw new NotImplementedException();
        }
    }
}
