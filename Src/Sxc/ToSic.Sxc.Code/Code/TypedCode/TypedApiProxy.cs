using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Data;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code;

internal class TypedApiProxy(IExecutionContext exCtx, ICodeTypedApiHelper apiHelper) : ITypedApi, IHasLog
{
    public ILog Log => exCtx.Log!;

    public IAppTyped App => apiHelper.AppTyped;
    public ITypedStack AllResources => apiHelper.AllResources;
    public ITypedStack AllSettings => apiHelper.AllSettings;

    public ITypedItem? AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => exCtx.GetCdf().AsItem(data, new() { ItemIsStrict = propsRequired ?? true, UseMock = mock == true });

    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => exCtx.GetCdf().AsItems(list, new() { ItemIsStrict = propsRequired ?? true });

    public IEntity AsEntity(ICanBeEntity thing)
        => exCtx.GetCdf().AsEntity(thing);
}
