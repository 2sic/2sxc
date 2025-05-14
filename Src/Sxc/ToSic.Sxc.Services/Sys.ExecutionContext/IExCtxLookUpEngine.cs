using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxLookUpEngine
{
    ILookUpEngine LookUpForDataSources { get; }
}