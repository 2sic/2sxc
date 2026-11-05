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
public class CodeHelperTypedData(CompileCodeHelperSpecs helperSpecs)
    : HelperBase(helperSpecs.ExCtx.Log, $"{SxcLogName}.TC16HL")
{
    public CompileCodeHelperSpecs Specs { get; } = helperSpecs;

    public bool DefaultStrict = true;

    [field: AllowNull, MaybeNull]
    internal ContextData Data => field ??= Specs.ExCtx.GetContextData();

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= Specs.ExCtx.GetCdf();

    public ITypedItem MyItem => field ??= Cdf.AsItem(Data.MyItems.FirstOrDefault(), new() { ItemIsStrict = DefaultStrict });

    public IEnumerable<ITypedItem> MyItems => field ??= Cdf.EntitiesToItems(Data.MyItems, new() { ItemIsStrict = DefaultStrict, DropNullItems = true });

    public ITypedItem MyHeader => field ??= Cdf.AsItem(Data.MyHeaders.FirstOrDefault(), new() { ItemIsStrict = DefaultStrict });

    [field: AllowNull, MaybeNull]
    private ICodeTypedApiHelper TypedApiHelper => field ??= Specs.ExCtx.GetTypedApi();

    public ITypedStack AllResources => TypedApiHelper.AllResources;

    public ITypedStack AllSettings => TypedApiHelper.AllSettings;

    public IDevTools DevTools => field ??= new DevTools(Specs.CodeFileName, Log);
}