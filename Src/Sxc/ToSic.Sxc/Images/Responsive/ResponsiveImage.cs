using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Razor.Html5;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class ResponsiveImage: ResponsiveBase, IResponsiveImage
    {
        [PrivateApi("don't show")]
        internal ResponsiveImage(ImageService imgService, ResponsiveParams responsiveParams) : base(imgService, responsiveParams)
        {
        }

        /// <summary>
        /// Same as base / initial implementation, but add srcset if available
        /// </summary>
        public override Img Img
        {
            get
            {
                if (_img != null) return _img;
                _img = base.Img;

                if (ImgService.Features.IsEnabled(FeaturesCatalog.ImageServiceMultipleSizes.NameId))
                {
                    var srcSetValue = Srcset;
                    if (!string.IsNullOrEmpty(srcSetValue)) _img = _img.Srcset(srcSetValue);
                }
                
                if (ImgService.Features.IsEnabled(FeaturesCatalog.ImageServiceSetSizes.NameId))
                {
                    var sizes = ThisResize?.TagEnhancements?.Sizes;
                    if (!string.IsNullOrEmpty(sizes)) _img.Sizes(sizes);
                }

                return _img;
            }
        }

        private Img _img;

        /// <inheritdoc />
        public string Srcset => _srcSetCache
                                ?? (_srcSetCache = string.IsNullOrWhiteSpace(ThisResize?.TagEnhancements?.Variants)
                                    ? ""
                                    : ImgLinker.SrcSet(Call.Url, Settings, SrcSetType.Img, Call.Field)
                                );
        private string _srcSetCache;

        public override string ToString() => Img.ToString();
    }
}
