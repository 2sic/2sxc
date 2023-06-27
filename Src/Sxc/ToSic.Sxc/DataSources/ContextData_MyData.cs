using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.DataSource;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.AsConverter;
using static ToSic.Eav.DataSource.DataSourceConstants;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.DataSources
{
    internal partial class ContextData: IMyData
    {
        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _DynCodeRoot = codeRoot;
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }
        private AsConverterService AsC => _DynCodeRoot.AsC;

        #region New Item/Items

        ITypedItem IMyData.FakeItem => _fakeItem.Get(() => AsC.AsItem(AsC.FakeEntity(_DynCodeRoot.App?.AppId)));
        private readonly GetOnce<ITypedItem> _fakeItem = new GetOnce<ITypedItem>();
        IEnumerable<ITypedItem> IMyData.FakeItems => _fakeItems.Get(() => AsC.AsItems(((IMyData)this).FakeItem));
        private readonly GetOnce<IEnumerable<ITypedItem>> _fakeItems = new GetOnce<IEnumerable<ITypedItem>>();

        ITypedItem IMyData.Item(string streamName, string noParamOrder, ITypedItem fallback, bool? required)
        {
            Protect(noParamOrder, $"{nameof(fallback)}, {nameof(required)}");

            var l = Log.Fn<ITypedItem>();

            var stream = GetStream(streamName, nullIfNotFound: true);

            if (stream == null)
            {
                if (required == false) return l.Return(fallback, "no stream, fallback/null");
                throw l.Ex(new Exception($"Stream name '{streamName ?? StreamDefaultName}' not found. " +
                                         $"If you want this to return null, set '{nameof(required)}: false'. " +
                                         $"If you want to return another object, set both {nameof(required)} and {nameof(fallback)}"));
            }

            // If we got null, or an empty list
            if (stream.Any()) return l.Return(AsC.AsItem(stream), "found");
            if (fallback != null) return l.Return(fallback, "fallback");
            if (required != true) return l.ReturnNull("null, not required");

            var prefix = "." + nameof(IMyData.Item) + "(" + streamName + (streamName.HasValue() ? ", " : "");
            var ex = new ArgumentException(
                $"Called '{prefix})' for stream '{streamName ?? StreamDefaultName}' and got an empty list. " +
                $"If you want to handle data-not-found yourself, use '{prefix}{nameof(fallback)}: MyData.FakeItem)' (or use another item as fallback) or '{prefix}{nameof(required)}: false)'");

            throw l.Ex(ex);
        }

        IEnumerable<ITypedItem> IMyData.Items(string streamName, string noParamOrder, IEnumerable<ITypedItem> fallback, bool? required, bool? preferNull)
        {
            Protect(noParamOrder, $"{nameof(fallback)}, {nameof(required)}, {nameof(preferNull)}");

            var l = Log.Fn<IEnumerable<ITypedItem>>();

            var stream = GetStream(streamName, nullIfNotFound: true);

            if (stream == null)
            {
                if (required == false)
                    return l.Return(fallback ?? (preferNull == false ? new List<ITypedItem>() : null), "no stream: fallback/null");
                throw l.Ex(new Exception($"Stream '{streamName ?? StreamDefaultName}' not found. " +
                                         $"If you want this to return null, set '{nameof(required)}: false'. " +
                                         $"If you want to return another object, set both {nameof(required)} and {nameof(fallback)}"));
            }
            
            // Found stuff - simple case, just return
            if (stream.Any()) return l.Return(AsC.AsItems(stream), "found");

            if (fallback != null) return l.Return(fallback, "stream empty, fallback");

            // Stream is empty, required explicitly true
            if (required == true)
                throw l.Ex(new Exception($"Stream '{streamName ?? StreamDefaultName}' found but was empty, and {nameof(required)} was {required}"));

            return preferNull == true 
                ? l.ReturnNull("stream empty, null preferred") 
                : l.Return(new List<ITypedItem>(), "empty list");
        }

        public IDataSource DataSource => _querySource as IDataSource ?? _blockSource;

        #endregion
    }
}
