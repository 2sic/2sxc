using ToSic.Eav;
using ToSic.Eav.Documentation;
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
            ) : base(imgService, url, settings, noParamOrder: noParamOrder, factor: factor, srcSet: srcSet, imgAlt: imgAlt, imgClass: imgClass, logName: $"{Constants.SxcLogName}.PicSet")
        {
        }

        /// <summary>
        /// Same as base / initial implementation, but add srcset if available
        /// </summary>
        public override Img Img
        {
            get
            {
                if (_imgTag != null) return _imgTag;
                _imgTag = base.Img;
                var srcSetValue = Srcset;
                if (!string.IsNullOrEmpty(srcSetValue))
                    _imgTag = _imgTag.Srcset(srcSetValue);
                return _imgTag;
            }
        }

        private Img _imgTag;

        /// <inheritdoc />
        public string Srcset => _srcSetCache ?? (_srcSetCache = Settings.SrcSet == null ? "" : ImgLinker.ImageOrSrcSet(UrlOriginal, Settings));
        private string _srcSetCache;

        public override string ToString() => Img.ToString();
    }
}
