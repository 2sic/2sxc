using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Html5;

namespace ToSic.Sxc.Images
{
    public class ResponsiveImage: ResponsiveBase
    {
        [PrivateApi("don't show")]
        internal ResponsiveImage(ImageService imgService, ResponsiveParams callParams) : base(imgService, callParams, "Img")
        {
        }

        /// <summary>
        /// Same as base / initial implementation, but add srcset if available
        /// </summary>
        public override Img Img => _img2.Get(() =>
        {
            var img = base.Img;
            if (!string.IsNullOrEmpty(SrcSet)) img.Srcset(SrcSet);
            if (!string.IsNullOrEmpty(Sizes)) img.Sizes(Sizes);
            return img;
        });
        private readonly GetOnce<Img> _img2 = new GetOnce<Img>();

    }
}
