using ToSic.Eav.LookUp;
using ToSic.Lib.LookUp.Engines;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxLookUpEngine
{
    ILookUpEngine LookUpForDataSources { get; }
}