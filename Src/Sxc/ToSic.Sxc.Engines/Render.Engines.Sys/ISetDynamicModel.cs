using ToSic.Sxc.Render.Sys.Specs;

namespace ToSic.Sxc.Render.Engines.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ISetDynamicModel
{
    void SetDynamicModel(RenderSpecs renderSpecs);
}