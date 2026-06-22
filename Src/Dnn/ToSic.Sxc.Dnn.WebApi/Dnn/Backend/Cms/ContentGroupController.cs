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


    // TODO: shouldn't be part of ContentGroupController any more, as it's generic now
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public void Replace(Guid parent, string part, int index, int entityId, bool add = false)
        => Real.Replace(parent, part, index, entityId, add);


    // TODO: WIP changing this from ContentGroup editing to any list editing
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public ReplacementListDto Replace(Guid parent, string part, int index)
        => Real.Replace(parent, part, index);


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