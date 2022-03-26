using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Razor.Html5;

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

                if (!string.IsNullOrEmpty(SrcSet)) _img = _img.Srcset(SrcSet);

                // TODO: MAYBE move sizes to a public property?
                if (ImgService.Features.IsEnabled(FeaturesCatalog.ImageServiceSetSizes.NameId))
                {
                    var sizes = ThisResize?.TagEnhancements?.Sizes;
                    if (!string.IsNullOrEmpty(sizes)) _img.Sizes(sizes);
                }

                return _img;
            }
        }

        private Img _img;


        public override string ToString() => Img.ToString();
    }
}
