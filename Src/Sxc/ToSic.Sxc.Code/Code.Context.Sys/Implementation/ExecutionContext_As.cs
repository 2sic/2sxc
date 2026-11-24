using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Code.Context.Sys;

public partial class CodeContext: IWrapper<ICodeDataFactory>
{
    [PrivateApi] public ICodeDataFactory Cdf => field ??= GetService<ICodeDataFactory>();

    ICodeDataFactory IWrapper<ICodeDataFactory>.GetContents()
        => Cdf;


}