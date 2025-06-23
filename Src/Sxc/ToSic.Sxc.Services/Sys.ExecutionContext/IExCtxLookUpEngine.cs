using ToSic.Eav.LookUp.Sys.Engines;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxLookUpEngine
{
    ILookUpEngine LookUpForDataSources { get; }
}