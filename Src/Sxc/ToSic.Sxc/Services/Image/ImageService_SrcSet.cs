using ToSic.Eav;

namespace ToSic.Sxc.Services.Image
{
    public partial class ImageService
    {
        public string SrcSet(string url, object settings = null, string noParamOrder = Parameters.Protector, object factor = null, string srcSet = null) 
            => ImgLinker.Image(url, settings, factor: factor, srcSet: (srcSet as object ) ?? true);
    }
}
