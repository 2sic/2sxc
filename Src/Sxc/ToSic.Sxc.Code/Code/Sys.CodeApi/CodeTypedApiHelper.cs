using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Sys.CodeApi;

internal class CodeTypedApiHelper(ExecutionContext exCtx) : ICodeTypedApiHelper
{
    #region ICodeAnyApiHelper

    public ICmsContext CmsContext => exCtx.CmsContext;
    public IDataSource Data => exCtx.Block!.Data; // If accessed, it must be working

    public TService GetService<TService>(NoParamOrder npo = default, bool reuse = false, Type? type = default)
        where TService : class
        => exCtx.GetService<TService>(npo, reuse, type);

    public IDevTools DevTools => exCtx.DevTools;

    public ICodeDataFactory Cdf => exCtx.Cdf;
    public ILinkService Link => exCtx.Link;

    #endregion

    public IAppTyped AppTyped => field ??= exCtx.GetService<IAppTyped>(reuse: true);
    public ITypedStack AllSettings => exCtx.AllSettings;
    public ITypedStack AllResources => exCtx.AllResources;
    public ServiceKit16 ServiceKit16 => exCtx.GetKit<ServiceKit16>();
}