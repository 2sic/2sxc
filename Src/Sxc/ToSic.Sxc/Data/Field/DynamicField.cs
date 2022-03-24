using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data
{
    public class DynamicField: IDynamicField
    {

        internal DynamicField(IDynamicEntity parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        public string Name { get; }

        public IDynamicEntity Parent { get; }

        public object Raw
        {
            get
            {
                if (_rawRetrieved) return _raw;
                _rawRetrieved = true;
                return _raw = Parent.Get(Name, convertLinks: false);
            }
        }
        private object _raw;
        private bool _rawRetrieved;

        public string Url
        {
            get
            {
                if(_urlRetrieved) return _url;
                _urlRetrieved = true;
                return _url = Parent.Get(Name, convertLinks: true) as string;
            }
        }

        private string _url;
        private bool _urlRetrieved;


        public IMetadataOf MetadataOfItem()
        {
            if(_metadataOfItem != null) return _metadataOfItem;
            var app = Parent._Dependencies?.BlockOrNull?.Context?.AppState;
            if (app == null) return null;

            var value = Raw;

            if(!(value is string valString) || string.IsNullOrWhiteSpace(valString)) return null;

            _metadataOfItem = app.GetMetadataOf(TargetTypes.CmsItem, valString, "");
            return _metadataOfItem;
        }
        private IMetadataOf _metadataOfItem;

        public ImageDecorator ImageDecoratorOrNull()
        {
            if(_imgDecTried) return _imgDec;
            _imgDecTried = true;
            var md = MetadataOfItem();
            if(md == null) return null;
            var decItem = md.FirstOrDefaultOfType(ImageDecorator.TypeName);
            if (decItem != null)
                _imgDec = new ImageDecorator(decItem);
            return _imgDec;
        }
        private ImageDecorator _imgDec;
        private bool _imgDecTried;

    }
}
