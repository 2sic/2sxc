using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class ImgSet: ImgPicSetBase, IImgSet
    {
        [PrivateApi("don't show")]
        internal ImgSet(ImageService imgService, string url, object settings, string noParamOrder = Parameters.Protector, object factor = null, string srcSet = null)
            : base(imgService, url, settings, factor: factor, srcSet: srcSet, logName: $"{Constants.SxcLogName}.PicSet")
        {
        }

        /// <inheritdoc />
        public Img ImgTag => _imgTag ?? (_imgTag = Tag.Img()
            .Src(ImgLinker.Image(Url, new ResizeSettings(Settings, false), null))
            .Srcset(SrcSet));
        private Img _imgTag;

        /// <inheritdoc />
        public string SrcSet => _srcSetCache ?? (_srcSetCache = ImgLinker.Image(Url, Settings, srcSet: true));
        private string _srcSetCache;

        public override string ToString() => ImgTag.ToString();
    }
}
