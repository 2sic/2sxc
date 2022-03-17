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
            object factor = default, 
            object rules = default,
            string imgAlt = default,
            string imgClass = default
            ) : base(imgService, url, settings, noParamOrder: noParamOrder, factor: factor, rules: rules, imgAlt: imgAlt, imgClass: imgClass, logName: $"{Constants.SxcLogName}.PicSet")
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
        public string Srcset => _srcSetCache ?? (_srcSetCache = string.IsNullOrWhiteSpace(ThisResize?.TagEnhancements?.SrcSet) ? "" : ImgLinker.SrcSet(UrlOriginal, Settings, SrcSetType.ImgSrcSet));
        private string _srcSetCache;

        public override string ToString() => Img.ToString();
    }
}
