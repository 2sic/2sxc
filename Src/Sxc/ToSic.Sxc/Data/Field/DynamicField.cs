using System.IO;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Data
{
    public class DynamicField: IDynamicField
    {

        internal DynamicField(IDynamicEntity parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }

        public IDynamicEntity Parent { get; }

        /// <inheritdoc />
        public dynamic Raw => _raw.Get(() => Parent.Get(Name, convertLinks: false));
        private readonly GetOnce<dynamic> _raw = new GetOnce<dynamic>();


        /// <inheritdoc />
        public dynamic Value => _value.Get(() => Parent.Get(Name, convertLinks: true));
        private readonly GetOnce<dynamic> _value = new GetOnce<dynamic>();

        /// <inheritdoc />
        public string Url => Value as string;


        public IDynamicMetadata Metadata => _dynMeta.Get(() => new DynamicMetadata(MetadataOfItem, Parent.Entity, Parent._Dependencies));
        private readonly GetOnce<IDynamicMetadata> _dynMeta = new GetOnce<IDynamicMetadata>();


        /// <inheritdoc />
        public IMetadataOf MetadataOfItem => _itemMd.Get(() =>
            {
                if (!(Raw is string rawString) || string.IsNullOrWhiteSpace(rawString)) return null;
                var app = Parent._Dependencies?.BlockOrNull?.Context?.AppState;
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
            return decItem != null ? new ImageDecorator(decItem) : null;
        });
        private readonly GetOnce<ImageDecorator> _imgDec2 = new GetOnce<ImageDecorator>();
        
        IMetadataOf IHasMetadata.Metadata => MetadataOfItem;
    }
}
