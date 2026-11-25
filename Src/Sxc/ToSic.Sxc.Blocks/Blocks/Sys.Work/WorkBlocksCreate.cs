using ToSic.Eav.Apps.Sys;

namespace ToSic.Sxc.Blocks.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkBlocksCreate(AppWorkChain<WorkEntityCreate> workEntCreate, AppWorkChain<WorkEntityUpdate> workEntUpdate)
    : ServiceWithSetup<IAppWorkContext>("AWk.EntCre", connect: [workEntCreate, workEntUpdate])
{
    public int NewBlockReference(int parentId, string field, int index, string app = "", Guid? guid = null)
    {
        var l = Log.Fn<int>($"get CB parent:{parentId}, field:{field}, order:{index}, app:{app}, guid:{guid}");
        var contentTypeName = AppConstants.ContentGroupRefTypeName;
        var values = new Dictionary<string, object>
        {
            {BlockBuildingConstants.CbPropertyTitle, ""},
            {BlockBuildingConstants.CbPropertyApp, app},
        };
        var newGuid = guid ?? Guid.NewGuid();
        var entityId = CreateItemAndAddToList(parentId, field, index, contentTypeName, values, newGuid);

        return l.ReturnAndLog(entityId);
    }


    private int CreateItemAndAddToList(int parentId, string field, int index, string typeName, Dictionary<string, object> values, Guid newGuid)
    {
        var l = Log.Fn<int>($"{nameof(parentId)}:{parentId}, {nameof(field)}:{field}, {nameof(index)}, {index}, {nameof(typeName)}:{typeName}");

        // create the new entity 
        var entityId = workEntCreate.New(MyOptions)
            .GetOrCreate(newGuid, typeName, values);

        #region attach to the current list of items

        var cbEnt = MyOptions.AppReader.List.GetOne(parentId)
                    ?? throw new NullReferenceException($"Tried to use the just-created entity with id '{parentId}' but can't find it.");
        var blockList = cbEnt.Children(field);

        var intList = blockList
            .OfType<IEntity>()
            .Select(b => b.EntityId)
            .ToList(); // must be list, as it will be modified

        // add only if it's not already in the list (could happen if http requests are run again)
        if (!intList.Contains(entityId))
        {
            if (index > intList.Count)
                index = intList.Count;
            intList.Insert(index, entityId);
        }
        var updateDic = new Dictionary<string, object> { { field, intList } };
        workEntUpdate.New(MyOptions).UpdateParts(cbEnt.EntityId, updateDic, new());

        #endregion

        return l.ReturnAndLog(entityId);
    }
}
