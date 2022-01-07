using ToSic.Eav;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Images
{
    public abstract class ImgPicSetBase: HybridHtmlString
    {
        protected ImgPicSetBase(
            ImageService imgService, 
            string url, 
            object settings, 
            // ReSharper disable once UnusedParameter.Local
            string noParamOrder = Parameters.Protector, 
            object factor = null, 
            string srcSet = null,
            string imgAlt = null,
            string imgClass = null,
            string logName = Constants.SxcLogName + ".IPSBas"
            ) : base(logName)
        {
            ImgService = imgService;
            FactorParam = factor;
            SrcSetParam = srcSet;
            ImgAlt = imgAlt;
            ImgClass = imgClass;
            ImgLinker = imgService.ImgLinker;
            Url = url;
            Settings = PrepareResizeSettings(settings, factor, srcSet);

        }
        protected readonly ImgResizeLinker ImgLinker;
        protected readonly ImageService ImgService;
        protected readonly object FactorParam;
        protected readonly string SrcSetParam;
        protected readonly string ImgAlt;
        protected readonly string ImgClass;

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

            if (srcSet != null) (resizeSettings as ResizeSettings).SrcSet = srcSet;

            return resizeSettings;
        }

        public abstract override string ToString();
    }
}
