using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Services.Kits
{
    public class KitV14: KitBase
    {

        public IConvertService Convert => _convert.Get(GetService<IConvertService>);
        private readonly ValueGetOnce<IConvertService> _convert = new ValueGetOnce<IConvertService>();


        public IFeaturesService Features => _features.Get(GetService<IFeaturesService>);
        private readonly ValueGetOnce<IFeaturesService> _features = new ValueGetOnce<IFeaturesService>();

        public IImageService Images => _image.Get(GetService<IImageService>);
        private readonly ValueGetOnce<IImageService> _image = new ValueGetOnce<IImageService>();

        public IPageService Page => _page.Get(GetService<IPageService>);
        private readonly ValueGetOnce<IPageService> _page = new ValueGetOnce<IPageService>();


        public IToolbarService Toolbar => _toolbar.Get(GetService<IToolbarService>);
        private readonly ValueGetOnce<IToolbarService> _toolbar = new ValueGetOnce<IToolbarService>();
    }

}
