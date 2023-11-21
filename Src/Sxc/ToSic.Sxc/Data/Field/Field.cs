using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Images;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public class Field: IField
    {

        internal Field(ITypedItem parent, string name, CodeDataFactory cdf)
        {
            Parent = parent;
            _cdf = cdf;
            Name = name;
        }

        private readonly CodeDataFactory _cdf;

        /// <inheritdoc />
        public string Name { get; }

        public ITypedItem Parent { get; }

        /// <inheritdoc />
        public object Raw => _raw.Get(() => Parent.Get(Name, required: false));
        private readonly GetOnce<object> _raw = new GetOnce<object>();


        /// <inheritdoc />
        [PrivateApi("Was public till 16.03, but don't think it should be surfaced...")]
        public object Value => _value.Get(() => Url ?? Raw); // (Parent as IDynamicEntity)?.Get(Name, convertLinks: true) ?? Raw);
        private readonly GetOnce<object> _value = new GetOnce<object>();

        /// <inheritdoc />
        public string Url => _url ?? (_url = Parent.Url(Name));
        private string _url;


        // 2023-08-14 v16.03 removed by 2dm as never used; KISS
        public IMetadata Metadata => _dynMeta.Get(() => new Metadata(MetadataOfValue, _cdf));
        private readonly GetOnce<IMetadata> _dynMeta = new GetOnce<IMetadata>();


        private IMetadataOf MetadataOfValue => _itemMd.Get(() =>
            {
                if (!(Raw is string rawString) || string.IsNullOrWhiteSpace(rawString)) return null;
                var appState = _cdf?.BlockOrNull?.Context?.AppState;
                var md = appState?.GetMetadataOf(TargetTypes.CmsItem, rawString, "");
                ImageDecorator.AddRecommendations(md, Url, _cdf?._DynCodeRoot); // needs the url so it can check if we use image recommendations
                return md;
            });
        private readonly GetOnce<IMetadataOf> _itemMd = new GetOnce<IMetadataOf>();

        public ImageDecorator ImageDecoratorOrNull =>
            _imgDec2.Get(() => ImageDecorator.GetOrNull(this, _cdf.Dimensions));
        private readonly GetOnce<ImageDecorator> _imgDec2 = new GetOnce<ImageDecorator>();
        
        IMetadataOf IHasMetadata.Metadata => MetadataOfValue;
    }
}
