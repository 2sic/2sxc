using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        #region basic properties like Content, Header

        /// <inheritdoc />
        public dynamic Content
        {
            get
            {
                if (_content != null || _contentTried) return _content;
                _contentTried = true;
                return _content = TryToBuildFirstOfStream(Eav.Constants.DefaultStreamName);
            }
        }
        private dynamic _content;
        private bool _contentTried;


        /// <inheritdoc />
		public dynamic Header
        {
            get
            {
                if (_header != null || _headerTried) return _header;
                _headerTried = true;
                _header = TryToBuildFirstOfStream(ViewParts.StreamHeader);
                if (_header != null) return _header;
                Log.A($"Header not yet found in {ViewParts.StreamHeader}, will try {ViewParts.StreamHeaderOld}");
                return _header = TryToBuildFirstOfStream(ViewParts.StreamHeaderOld);
            }
        }
        private dynamic _header;
        private bool _headerTried;

        private dynamic TryToBuildFirstOfStream(string sourceStream)
        {
            var wrapLog = Log.Fn<object>(sourceStream);
            if (Data == null || Block.View == null) return wrapLog.ReturnNull("no data/block");
            if (!Data.Out.ContainsKey(sourceStream)) return wrapLog.ReturnNull("stream not found");

            var list = Data[sourceStream].List;
            return !list.Any()
                ? wrapLog.ReturnNull("first is null") 
                : wrapLog.Return(new DynamicEntity(list, null, null, null, DynamicEntityServices), "found");
        }
        
        #endregion
    }
}
