using ToSic.Sxc.Code.Sys;

namespace Custom.Razor.Sys;

[PrivateApi("not sure yet if this will stay in Hybrid or go to Web.Razor or something, so keep it private for now")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IRazor12: IRazor, IDynamicCode12
{
    [PrivateApi]
    dynamic DynamicModel { get; }

}