using ToSic.Eav.DataSource;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.CodeApi;

internal abstract class CodeAnyApiHelper(ExecutionContext parent) : ICodeAnyApiHelper
{
    protected ExecutionContext Parent = parent;

    public IBlock Block => Parent.Block;

    public ICmsContext CmsContext => Parent.CmsContext;
    public IDataSource Data => Parent.Data;

    public TService GetService<TService>(NoParamOrder protector = default, bool reuse = false, Type type = default) where TService : class
        => Parent.GetService<TService>(protector, reuse, type);

    public IDevTools DevTools => Parent.DevTools;

    public ICodeDataFactory Cdf => Parent.Cdf;
    public ILinkService Link => Parent.Link;

    public IConvertService Convert => Parent.Convert;
}