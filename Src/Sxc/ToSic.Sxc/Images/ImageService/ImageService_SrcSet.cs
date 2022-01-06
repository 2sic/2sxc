using ToSic.Sxc.Web;

namespace ToSic.Sxc.Images
{
    public partial class ImageService
    {
        public IHybridHtmlString SrcSet(string url, object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null, string srcSet = null)
            => new HybridHtmlString(ImgLinker.Image(url, settings, factor: factor, srcSet: (srcSet as object) ?? true));
    }
}
