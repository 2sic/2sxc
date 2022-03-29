using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;

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
        private readonly PropertyToRetrieveOnce<dynamic> _raw = new PropertyToRetrieveOnce<dynamic>();


        /// <inheritdoc />
        public dynamic Value => _value.Get(() => Parent.Get(Name, convertLinks: true));
        private readonly PropertyToRetrieveOnce<dynamic> _value = new PropertyToRetrieveOnce<dynamic>();

        /// <inheritdoc />
        public string Url => Value as string;


        public IMetadataOf MetadataOfItem => _itemMd.Get(() =>
            {
                if (!(Raw is string valString) || string.IsNullOrWhiteSpace(valString)) return null;
                var app = Parent._Dependencies?.BlockOrNull?.Context?.AppState;
                return app?.GetMetadataOf(TargetTypes.CmsItem, valString, "");
            });
        private readonly PropertyToRetrieveOnce<IMetadataOf> _itemMd = new PropertyToRetrieveOnce<IMetadataOf>();



        public ImageDecorator ImageDecoratorOrNull => _imgDec2.Get(() =>
        {
            var decItem = MetadataOfItem?.FirstOrDefaultOfType(ImageDecorator.TypeName);
            return decItem != null ? new ImageDecorator(decItem) : null;
        });
        private readonly PropertyToRetrieveOnce<ImageDecorator> _imgDec2 = new PropertyToRetrieveOnce<ImageDecorator>();

    }
}
