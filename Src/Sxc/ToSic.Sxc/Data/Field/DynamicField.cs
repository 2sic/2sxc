﻿using ToSic.Eav.Apps.Decorators;
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

        public IMetadataOf MetadataOfItem()
        {
            if(_metadataOfItem != null) return _metadataOfItem;
            var ctx = Parent._Dependencies?.BlockOrNull?.Context;
            if (ctx == null) return null;

            var value = Parent.Get(Name, convertLinks: false); //.Entity.GetBestValue<string>(Name,);
            if (value == null) return null;

            if(!(value is string valString) || string.IsNullOrWhiteSpace(valString)) return null;

            var app = ctx.AppState;
            if (app == null) return null;
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