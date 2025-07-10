using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code;

/// <summary>
/// Standalone object with the typical typed APIs - for use in the `ITypedApiService`
/// </summary>
/// <param name="exCtx"></param>
/// <param name="apiHelper"></param>
internal class TypedApiStandalone(IExecutionContext exCtx, ICodeTypedApiHelper apiHelper) : ITypedApi, IHasLog
{
    public ILog Log => exCtx.Log!;

    public IAppTyped App => apiHelper.AppTyped;

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

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= exCtx.GetCdf();

    public ITypedItem? AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => Cdf.AsItem(data, new() { ItemIsStrict = propsRequired ?? true, UseMock = mock == true });

    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => Cdf.AsItems(list, new() { ItemIsStrict = propsRequired ?? true });

    public IEntity AsEntity(ICanBeEntity thing)
        => Cdf.AsEntity(thing);

    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => Cdf.AsTyped(original, new() { FirstIsRequired = false, ItemIsStrict = propsRequired ?? true });

    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => Cdf.AsTypedList(list, new() { FirstIsRequired = false, ItemIsStrict = propsRequired ?? true });

    public ITypedStack AsStack(params object[] items)
        => Cdf.AsStack(items);

    public T AsStack<T>(params object[] items)
        where T : class, ICanWrapData, new()
        => Cdf.AsStack<T>(items);

    #region As / AsList WIP v17

    /// <inheritdoc />
    public T As<T>(object source, NoParamOrder protector = default, bool mock = false)
        where T : class, ICanWrapData
        => Cdf.AsCustom<T>(source: source, protector: protector, mock: mock);

    /// <inheritdoc />
    public IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ICanWrapData
        => Cdf.AsCustomList<T>(source: source, protector: protector, nullIfNull: nullIfNull);

    #endregion

    #region MyContext & UniqueKey

    /// <inheritdoc cref="ITypedApi.MyContext" />
    public ICmsContext MyContext => apiHelper.CmsContext;

    /// <inheritdoc cref="ITypedApi.MyPage" />
    public ICmsPage MyPage => apiHelper.CmsContext.Page;

    /// <inheritdoc cref="ITypedApi.MyUser" />
    public ICmsUser MyUser => apiHelper.CmsContext.User;

    /// <inheritdoc cref="ITypedApi.MyView" />
    public ICmsView MyView => apiHelper.CmsContext.View;

    /// <inheritdoc cref="ITypedApi.UniqueKey" />
    public string UniqueKey => Kit.Key.UniqueKey;

    #endregion
}
