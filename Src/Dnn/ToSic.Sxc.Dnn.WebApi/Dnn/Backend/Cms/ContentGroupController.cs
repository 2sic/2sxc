using ToSic.Sxc.Backend.Cms;
using RealController = ToSic.Sxc.Backend.Cms.ContentGroupControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
    public void Replace(Guid guid, string part, int index, int entityId, bool add = false)
        => Real.Replace(guid, part, index, entityId, add);


    // TODO: WIP changing this from ContentGroup editing to any list editing
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public ReplacementListDto Replace(Guid guid, string part, int index)
        => Real.Replace(guid, part, index);


    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public List<EntityInListDto> ItemList(Guid guid, string part)
        => Real.ItemList(guid, part);


    // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public bool ItemList([FromUri] Guid guid, List<EntityInListDto> list, [FromUri] string part = null)
        => Real.ItemList(guid, list, part);

}