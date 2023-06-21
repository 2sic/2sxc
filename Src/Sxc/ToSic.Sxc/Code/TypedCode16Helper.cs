using System.Collections.Generic;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.AsConverter;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Code
{
    public class TypedCode16Helper
    {
        private readonly AsConverterService _asc;
        internal ContextData Data { get; }
        public TypedCode16Helper(AsConverterService asc, IContextData data)
        {
            _asc = asc;
            Data = data as ContextData;
        }

        public ITypedItem MyItem => _myItem.Get(() => _asc.AsItem(Data.MyContent));
        private readonly GetOnce<ITypedItem> _myItem = new GetOnce<ITypedItem>();

        public IEnumerable<ITypedItem> MyItems => _myItems.Get(() => _asc.AsItems(Data.MyContent));
        private readonly GetOnce<IEnumerable<ITypedItem>> _myItems = new GetOnce<IEnumerable<ITypedItem>>();

        public ITypedItem MyHeader => _myHeader.Get(() => _asc.AsItem(Data.MyHeader));
        private readonly GetOnce<ITypedItem> _myHeader = new GetOnce<ITypedItem>();

    }
}
