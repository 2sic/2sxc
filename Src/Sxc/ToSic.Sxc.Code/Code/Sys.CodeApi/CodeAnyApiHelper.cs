using ToSic.Eav.DataSource;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Sys.CodeApi;

internal abstract class CodeAnyApiHelper(ExecutionContext exCtx) : ICodeAnyApiHelper
{
    protected ExecutionContext ExCtx = exCtx;

    public IBlock Block => ExCtx.Block!; // If accessed, the code doing it must have the block

    public ICmsContext CmsContext => ExCtx.CmsContext;
    public IDataSource Data => ExCtx.Block!.Data; // If accessed, it must be working

    public TService GetService<TService>(NoParamOrder npo = default, bool reuse = false, Type? type = default)
        where TService : class
        => ExCtx.GetService<TService>(npo, reuse, type);

    public IDevTools DevTools => ExCtx.DevTools;

    public ICodeDataFactory Cdf => ExCtx.Cdf;
    public ILinkService Link => ExCtx.Link;

    public IConvertService Convert => ExCtx.Convert;
}