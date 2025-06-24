using ToSic.Eav.DataSource;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.CodeApi;

internal abstract class CodeAnyApiHelper(ExecutionContext exCtx) : ICodeAnyApiHelper
{
    protected ExecutionContext ExCtx = exCtx;

    public IBlock Block => ExCtx.Block;

    public ICmsContext CmsContext => ExCtx.CmsContext;
    public IDataSource Data => ExCtx.Block.Data;

    public TService GetService<TService>(NoParamOrder protector = default, bool reuse = false, Type? type = default)
        where TService : class
        => ExCtx.GetService<TService>(protector, reuse, type);

    public IDevTools DevTools => ExCtx.DevTools;

    public ICodeDataFactory Cdf => ExCtx.Cdf;
    public ILinkService Link => ExCtx.Link;

    public IConvertService Convert => ExCtx.Convert;
}