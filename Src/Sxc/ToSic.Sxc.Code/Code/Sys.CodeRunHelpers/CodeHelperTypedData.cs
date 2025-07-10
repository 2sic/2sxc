using ToSic.Eav.DataSource;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// Code Helper for typed code v16+
/// </summary>
/// <param name="helperSpecs"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeHelperTypedData(CompileCodeHelperSpecs helperSpecs, string? logName = null)
    : CodeHelperV00Base(helperSpecs, SxcLogName + (logName ?? ".TC16HL"))
{
    public bool DefaultStrict = true;

    [field: AllowNull, MaybeNull]
    internal ContextData Data => field ??= (ContextData)ExCtx.GetState<IDataSource>();

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= ExCtx.GetCdf();

    public ITypedItem MyItem => _myItem.Get(() => Cdf.AsItem(Data.MyItems.FirstOrDefault(), new() { ItemIsStrict = DefaultStrict })!)!;
    private readonly GetOnce<ITypedItem> _myItem = new();

    public IEnumerable<ITypedItem> MyItems => _myItems.Get(() =>
        Cdf.EntitiesToItems(Data.MyItems, new() { ItemIsStrict = DefaultStrict, DropNullItems = true }))!;
    private readonly GetOnce<IEnumerable<ITypedItem>> _myItems = new();

    public ITypedItem MyHeader => _myHeader.Get(() => Cdf.AsItem(Data.MyHeaders.FirstOrDefault(), new() { ItemIsStrict = DefaultStrict })!)!;
    private readonly GetOnce<ITypedItem> _myHeader = new();

    [field: AllowNull, MaybeNull]
    private ICodeTypedApiHelper TypedApiHelper => field ??= ExCtx.GetTypedApi();

    public ITypedStack AllResources => TypedApiHelper.AllResources;

    public ITypedStack AllSettings => TypedApiHelper.AllSettings;

    //public IDevTools DevTools => _devTools.Get(() => new DevTools(IsRazor, CodeFileName, Log));
    //private readonly GetOnce<IDevTools> _devTools = new GetOnce<IDevTools>();
    
}