using ToSic.Lib.Helpers;
using static ToSic.Eav.DataSource.Internal.DataSourceConstants;
using static ToSic.Sxc.Blocks.Internal.ViewParts;

namespace ToSic.Sxc.Code.Internal;

public partial class CodeApiService
{
    #region basic properties like Content, Header

    /// <inheritdoc cref="IDynamicCode.Content" />
    public dynamic Content => _contentGo.Get(() => TryToBuildFirstOfStream(StreamDefaultName));
    private readonly GetOnce<object> _contentGo = new();


    /// <inheritdoc cref="IDynamicCode.Header" />
    public dynamic Header => _header.GetL(Log, l =>
    {
        var header = TryToBuildFirstOfStream(StreamHeader);
        if (header != null) return header;
        // If header isn't found, it could be that an old query is used which attached the stream to the old name
        l.A($"Header not yet found in {StreamHeader}, will try {StreamHeaderOld}");
        return TryToBuildFirstOfStream(StreamHeaderOld);
    });
    private readonly GetOnce<object> _header = new();

    private dynamic TryToBuildFirstOfStream(string sourceStream)
    {
        var l = Log.Fn<object>(sourceStream);
        if (Data == null || _Block.View == null) return l.ReturnNull("no data/block");
        if (!Data.Out.ContainsKey(sourceStream)) return l.ReturnNull("stream not found");

        var list = Data[sourceStream].List.ToList();
        return !list.Any()
            ? l.ReturnNull("first is null") 
            : l.Return(Cdf.AsDynamicFromEntities(list, false), "found");
    }
        
    #endregion
}