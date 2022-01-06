using System;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Services.Image
{
    public partial class ImageService: HasLog<ImageService>, IImageService
    {
        public ImgResizeLinker ImgLinker { get; }

        public ImageService(ImgResizeLinker imgLinker) : base(Constants.SxcLogName + ".ImgSvc")
        {
            ImgLinker = imgLinker.Init(Log);
        }

        public ITag SourceTags(string url, object settings = null, string noParamOrder = Parameters.Protector, object formats = null,
            object factor = null)
        {
            // Check formats
            var format = GetFormat(url);
            if (format == null || format.ResizeFormats.Count == 0) return Tag.Custom(null);

            // 1. Prepare Settings

            // 1.x. Settings

            // 1.x. Factor ?

            // Generate Meta Tags

            var sources = format.ResizeFormats.Select(rf => Tag.Source().Type(rf.MimeType).Srcset("todo"));
            var result = Tag.Custom(null, sources);
            return result;


            throw new NotImplementedException();
        }

        public object PictureTag(string url, object settings = null, string noParamOrder = Parameters.Protector, object factor = null,
            string alt = null, Action<string> img = null)
        {
            throw new NotImplementedException();
        }
    }
}
