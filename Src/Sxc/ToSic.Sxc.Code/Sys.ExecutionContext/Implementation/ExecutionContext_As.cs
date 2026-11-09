using ToSic.Sxc.Data.Sys.CodeDataFactory;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext: IWrapper<ICodeDataFactory>
{
    [PrivateApi]
    internal ICodeDataFactory Cdf => field ??= new Func<ICodeDataFactory>(() =>
    {
        ((CodeDataFactory)Services.Cdf).ConnectToRoot(this);
        return Services.Cdf;
    })();

    ICodeDataFactory IWrapper<ICodeDataFactory>.GetContents()
        => Cdf;


}