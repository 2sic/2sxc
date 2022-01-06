using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Images
{
    public partial class ImageService: HasLog<ImageService>, IImageService
    {
        public ImageService(ImgResizeLinker imgLinker) : base(Constants.SxcLogName + ".ImgSvc") => ImgLinker = imgLinker.Init(Log);
        internal ImgResizeLinker ImgLinker { get; }

        public IPictureSet Picture(string url, object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null, string srcSet = null) 
            => new PictureSet(this, url, settings, factor: factor, srcSet: srcSet);

        public IImgSet Img(string url, object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            string srcSet = null) 
            => new ImgSet(this, url, settings, factor: factor, srcSet: srcSet);
        

    }
}
