using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Sxc.Backend.Cms.ContentGroupControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentGroupController() : DnnSxcControllerBase(RealController.LogSuffix), IContentGroupController
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public EntityInListDto Header(Guid guid) 
        => Real.Header(guid);


    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public List<EntityInListDto> ItemList(Guid parent, string part)
        => Real.ItemList(parent, part);


    // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public bool ItemList([FromUri] Guid parent, List<EntityInListDto> list, [FromUri] string part = null)
        => Real.ItemList(parent, list, part);

}