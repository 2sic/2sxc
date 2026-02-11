using ToSic.Sxc.Code.Sys;
using static ToSic.Eav.DataSource.DataSourceConstants;
using static ToSic.Sxc.Blocks.Sys.Views.ViewParts;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext
{
    #region basic properties like Content, Header

    /// <inheritdoc cref="IDynamicCodeDocs.Content" />
    public object? Content => _contentGo.Get(() => TryToBuildFirstOfStream(StreamDefaultName));
    private readonly GetOnce<object?> _contentGo = new();


    /// <inheritdoc cref="IDynamicCodeDocs.Header" />
    public object? Header => _header.Get(GetHeaderOrNull);
    private readonly GetOnce<object?> _header = new();

    private object? GetHeaderOrNull()
    {
        var l = Log.Fn<object?>();
        if (TryToBuildFirstOfStream(StreamHeader) is { } header)
            return l.Return(header, "found");
        // If header isn't found, it could be that an old query is used which attached the stream to the old name
        l.A($"Header not yet found in {StreamHeader}, will try {StreamHeaderOld}");
        return l.Return(TryToBuildFirstOfStream(StreamHeaderOld), "old query");

    }

    /// <summary>
    /// Note: can be null
    /// </summary>
    /// <param name="sourceStream"></param>
    /// <returns></returns>
    private object? TryToBuildFirstOfStream(string sourceStream)
    {
        var l = Log.Fn<object>(sourceStream);
        if (Block is not { DataIsReady: true })
            return l.ReturnNull("no data/view");
        var data = Block.Data;

        var stream = data.GetStream(sourceStream, nullIfNotFound: true);
        if (stream == null)
            return l.ReturnNull("stream not found");

        var list = stream.List.ToList();
        return !list.Any()
            ? l.ReturnNull("first is null") 
            : l.Return(Cdf.AsDynamicFromEntities(list, new() { ItemIsStrict = false }), "found");
    }
        
    #endregion
}