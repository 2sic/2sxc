using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code;

internal class TypedApiProxy(IExecutionContext exCtx, ICodeTypedApiHelper apiHelper) : ITypedApi, IHasLog
{
    public ILog Log => exCtx.Log!;

    public IAppTyped App => apiHelper.AppTyped;
    
    public IDataSource Data => apiHelper.Data;

    public ITypedStack AllResources => apiHelper.AllResources;
    public ITypedStack AllSettings => apiHelper.AllSettings;


    #region Services

    public TService GetService<TService>() where TService : class
        => exCtx.GetService<TService>();

    [field: AllowNull, MaybeNull]
    public ServiceKit16 Kit => field ??= apiHelper.ServiceKit16;

    public ILinkService Link => apiHelper.Link;

    #endregion

    #region My Data Stuff

    [field: AllowNull, MaybeNull]
    private CodeHelperTypedData CodeHelper => field ??= new(helperSpecs: new(exCtx, false, "error no code file"));

    /// <inheritdoc />
    public ITypedItem MyItem => CodeHelper.MyItem;

    /// <inheritdoc />
    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    /// <inheritdoc />
    public ITypedItem MyHeader => CodeHelper.MyHeader;

    /// <inheritdoc />
    public IDataSource MyData => CodeHelper.Data;

    #endregion


    public ITypedItem? AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => exCtx.GetCdf().AsItem(data, new() { ItemIsStrict = propsRequired ?? true, UseMock = mock == true });

    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => exCtx.GetCdf().AsItems(list, new() { ItemIsStrict = propsRequired ?? true });

    public IEntity AsEntity(ICanBeEntity thing)
        => exCtx.GetCdf().AsEntity(thing);
}
