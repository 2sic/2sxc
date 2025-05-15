using ToSic.Eav.DataSource;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.CodeApi;

internal abstract class CodeAnyApiHelper: ICodeAnyApiHelper, IExCtxGetKit
{
    internal void Setup(CodeApiService parent)
    {
        Parent = parent;
    }
    protected CodeApiService Parent { get; private set; }

    public ICmsContext CmsContext => Parent.CmsContext;
    public IDataSource Data => Parent.Data;

    public TService GetService<TService>(NoParamOrder protector = default, bool reuse = false, Type type = default) where TService : class
        => Parent.GetService<TService>(protector, reuse, type);

    public IDevTools DevTools => Parent.DevTools;

    public ICodeDataFactory Cdf => Parent.Cdf;
    public ILinkService Link => Parent.Link;
    public TKit GetKit<TKit>() where TKit : ServiceKit
        => Parent.GetKit<TKit>();
}

public static class CodeApiHelperExtensions
{
    internal static T SetupQ<T>(this T codeApiHelper, CodeApiService parent) where T : CodeAnyApiHelper
    {
        codeApiHelper.Setup(parent);
        return codeApiHelper;
    }
}