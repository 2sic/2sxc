using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext: IWrapper<ICodeDataFactory>
{
    [PrivateApi]
    internal ICodeDataFactory Cdf => field ??= new Func<ICodeDataFactory>(() =>
    {
        var cd = GetService<ICodeDataFactory>();
        ((INeedsExecutionContext)cd).ConnectToRoot(this);
        return cd;
    })();

    ICodeDataFactory IWrapper<ICodeDataFactory>.GetContents()
        => Cdf;


}