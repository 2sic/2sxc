using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext: IWrapper<ICodeDataFactory>
{
    [PrivateApi] public ICodeDataFactory Cdf => field ??= GetService<ICodeDataFactory>();

    ICodeDataFactory IWrapper<ICodeDataFactory>.GetContents()
        => Cdf;


}