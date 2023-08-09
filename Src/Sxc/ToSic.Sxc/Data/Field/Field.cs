using System.IO;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
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
        public object Value => _value.Get(() => (Parent as IDynamicEntity)?.Get(Name, convertLinks: true) ?? Raw);
        private readonly GetOnce<object> _value = new GetOnce<object>();

        /// <inheritdoc />
        public string Url => _url ?? (_url = Parent.Url(Name));
        private string _url;


        public IMetadata Metadata => _dynMeta.Get(() => new Metadata(MetadataOfItem, Parent.Entity, _cdf));
        private readonly GetOnce<IMetadata> _dynMeta = new GetOnce<IMetadata>();


        private IMetadataOf MetadataOfItem => _itemMd.Get(() =>
            {
                if (!(Raw is string rawString) || string.IsNullOrWhiteSpace(rawString)) return null;
                var appState = _cdf?.BlockOrNull?.Context?.AppState;
                var md = appState?.GetMetadataOf(TargetTypes.CmsItem, rawString, "");

                // Optionally add image-metadata recommendations
                if (md?.Target != null && Value is string valString && valString.HasValue())
                {
                    var ext = Path.GetExtension(valString);
                    if (ext.HasValue() && Classification.IsImage(ext))
                        md.Target.Recommendations = new[] { ImageDecorator.TypeNameId };
                }

                return md;
            });
        private readonly GetOnce<IMetadataOf> _itemMd = new GetOnce<IMetadataOf>();



        public ImageDecorator ImageDecoratorOrNull => _imgDec2.Get(() =>
        {
            var decItem = MetadataOfItem?.FirstOrDefaultOfType(ImageDecorator.TypeNameId);
            return decItem != null ? new ImageDecorator(decItem, _cdf.Dimensions) : null;
        });
        private readonly GetOnce<ImageDecorator> _imgDec2 = new GetOnce<ImageDecorator>();
        
        IMetadataOf IHasMetadata.Metadata => MetadataOfItem;
    }
}
