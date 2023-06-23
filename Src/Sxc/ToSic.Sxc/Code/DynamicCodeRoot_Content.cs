using System.Linq;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data;
using static ToSic.Eav.DataSource.DataSourceConstants;
using static ToSic.Sxc.Blocks.ViewParts;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        #region basic properties like Content, Header

        /// <inheritdoc />
        public dynamic Content => _contentGo.Get(() => TryToBuildFirstOfStream(StreamDefaultName));
        private readonly GetOnce<object> _contentGo = new GetOnce<object>();


        /// <inheritdoc />
        public dynamic Header => _header.Get(Log, l =>
        {
            var header = TryToBuildFirstOfStream(StreamHeader);
            if (header != null) return header;
            // If header isn't found, it could be that an old query is used which attached the stream to the old name
            l.A($"Header not yet found in {StreamHeader}, will try {StreamHeaderOld}");
            return TryToBuildFirstOfStream(StreamHeaderOld);
        });
        private readonly GetOnce<object> _header = new GetOnce<object>();

        private dynamic TryToBuildFirstOfStream(string sourceStream)
        {
            var wrapLog = Log.Fn<object>(sourceStream);
            if (Data == null || Block.View == null) return wrapLog.ReturnNull("no data/block");
            if (!Data.Out.ContainsKey(sourceStream)) return wrapLog.ReturnNull("stream not found");

            var list = Data[sourceStream].List.ToList();
            return !list.Any()
                ? wrapLog.ReturnNull("first is null") 
                : wrapLog.Return(AsC.AsDynamic(list), "found");
        }
        
        #endregion
    }
}
