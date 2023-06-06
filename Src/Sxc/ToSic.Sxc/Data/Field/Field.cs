using System.IO;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public class Field: IField
    {

        internal Field(IDynamicEntity parent, string name, DynamicEntity.MyServices services)
        {
            _parent = parent;
            _services = services;
            Name = name;
        }
        private readonly IDynamicEntity _parent;
        private readonly DynamicEntity.MyServices _services;

        /// <inheritdoc />
        public string Name { get; }

        public ITypedItem Parent => _parent;

        /// <inheritdoc />
        public object Raw => _raw.Get(() => _parent.Get(Name, convertLinks: false));
        private readonly GetOnce<object> _raw = new GetOnce<object>();


        /// <inheritdoc />
        public object Value => _value.Get(() => _parent.Get(Name, convertLinks: true));
        private readonly GetOnce<object> _value = new GetOnce<object>();

        /// <inheritdoc />
        public string Url => _url ?? (_url = Parent.Url(Name));
        private string _url;


        public IMetadata Metadata => _dynMeta.Get(() => new Metadata(MetadataOfItem, Parent.Entity, _services));
        private readonly GetOnce<IMetadata> _dynMeta = new GetOnce<IMetadata>();


        /// <inheritdoc />
        private IMetadataOf MetadataOfItem => _itemMd.Get(() =>
            {
                if (!(Raw is string rawString) || string.IsNullOrWhiteSpace(rawString)) return null;
                var app = _services?.BlockOrNull?.Context?.AppState;
                var md = app?.GetMetadataOf(TargetTypes.CmsItem, rawString, "");

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
            return decItem != null ? new ImageDecorator(decItem, _services.Dimensions) : null;
        });
        private readonly GetOnce<ImageDecorator> _imgDec2 = new GetOnce<ImageDecorator>();
        
        IMetadataOf IHasMetadata.Metadata => MetadataOfItem;
    }
}
