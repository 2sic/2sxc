using ToSic.Sxc.Render.Sys.Specs;

namespace ToSic.Sxc.Engines.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ISetDynamicModel
{
    void SetDynamicModel(RenderSpecs viewData);
}