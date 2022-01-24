using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class ResponsiveImage: ResponsiveBase, IResponsiveImage
    {
        [PrivateApi("don't show")]
        internal ResponsiveImage(
            ImageService imgService, 
            string url, 
            object settings, 
            string noParamOrder = Parameters.Protector, 
            object factor = null, 
            string srcSet = null,
            string imgAlt = null,
            string imgClass = null
            ) : base(imgService, url, settings, factor: factor, srcSet: srcSet, imgAlt: imgAlt, imgClass: imgClass, logName: $"{Constants.SxcLogName}.PicSet")
        {
        }

        /// <inheritdoc />
        public Img ImgTag
        {
            get
            {
                if (_imgTag != null) return _imgTag;

                _imgTag = Tag.Img()
                    .Src(ImgLinker.Image(Url, new ResizeSettings(Settings, false), null))
                    .Srcset(SrcSet);
                // Only add these if they were really specified
                if (ImgAlt != null) _imgTag.Alt(ImgAlt);
                if (ImgClass != null) _imgTag.Class(ImgClass);
                return _imgTag;
            }
        }

        private Img _imgTag;

        /// <inheritdoc />
        public string SrcSet => _srcSetCache ?? (_srcSetCache = ImgLinker.Image(Url, Settings, srcSet: true));
        private string _srcSetCache;

        public override string ToString() => ImgTag.ToString();
    }
}
