using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Images
{
    public abstract class ImgPicSetBase: HybridHtmlString
    {
        protected ImgPicSetBase(
            ImageService imgService, 
            string url, 
            object settings, 
            string noParamOrder = Parameters.Protector, 
            object factor = null, 
            string srcSet = null,
            string logName = Constants.SxcLogName + ".IPSBas"
            ) : base(logName)
        {
            ImgService = imgService;
            _factorParam = factor;
            _srcSetParam = srcSet;
            ImgLinker = imgService.ImgLinker;
            Url = url;
            Settings = PrepareResizeSettings(settings, factor, srcSet);

        }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;
        protected readonly object _factorParam;
        protected readonly string _srcSetParam;

        public string Url { get; }
        internal IResizeSettings Settings { get; }


        protected IResizeSettings PrepareResizeSettings(object settings, object factor, string srcSet)
        {
            // 1. Prepare Settings
            if (!(settings is IResizeSettings resizeSettings))
            {
                resizeSettings = ImgLinker.ResizeParamMerger.BuildResizeParameters(settings, factor: factor, srcSet: true);
            }
            else
            {
                // TODO: STILL USE THE FACTOR!
            }

            if (srcSet != null) resizeSettings.SrcSet = srcSet;

            return resizeSettings;
        }

        public abstract override string ToString();
    }
}
