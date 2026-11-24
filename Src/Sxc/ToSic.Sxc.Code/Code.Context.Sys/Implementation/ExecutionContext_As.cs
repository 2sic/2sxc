using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Code.Context.Sys;

public partial class ExecutionContext: IWrapper<ICodeDataFactory>
{
    [PrivateApi] public ICodeDataFactory Cdf => field ??= GetService<ICodeDataFactory>();

    ICodeDataFactory IWrapper<ICodeDataFactory>.GetContents()
        => Cdf;


}