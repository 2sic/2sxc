using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Sys.CodeApi;

internal class CodeTypedApiHelper(ExecutionContext exCtx) : ICodeTypedApiHelper
{
    #region Common State

    public ICmsContext CmsContext => exCtx.CmsContext;

    public IDataSource Data => exCtx.Block!.Data; // If accessed, it must be working

    public IAppTyped AppTyped => field ??= exCtx.GetService<IAppTyped>(reuse: true);

    #endregion


    #region Common Services

    public TService GetService<TService>(NoParamOrder npo = default, bool reuse = false, Type? type = default)
        where TService : class
        => exCtx.GetService<TService>(npo, reuse, type);

    public ServiceKit16 ServiceKit16 => exCtx.GetKit<ServiceKit16>();

    public IDevTools DevTools => throw new NotImplementedException("This is a future feature, we're just reserving the object name");

    public ICodeDataFactory Cdf => exCtx.Cdf;

    public ILinkService Link => field ??= GetService<ILinkService>(reuse: true);

    #endregion


    #region Common Local Data

    public ITypedStack AllSettings => exCtx.AllSettings;

    public ITypedStack AllResources => exCtx.AllResources;

    #endregion

}