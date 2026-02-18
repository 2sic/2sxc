using ToSic.Eav.WebApi.Sys.Helpers.Validation;

namespace ToSic.Sxc.Backend.SaveHelpers;

/// <summary>
/// Perform special validation checks on update, such as ensuring that we're updating the correct target item etc.
/// </summary>
/// <param name="parentLog"></param>
internal class SaveDataUpdateValidator(ILog parentLog) : ValidatorBase(parentLog, "Val.UpdOk")
{
    internal (int? ResetId, HttpExceptionAbstraction? Exception) IfUpdateValidateAndCorrectIds(WorkEntities workEntities, int count, IEntity newEntity)
    {
        var l = Log.Fn<(int?, HttpExceptionAbstraction?)>();
        var previousEntity = workEntities.Get(newEntity.EntityId)
                             ?? workEntities.Get(newEntity.EntityGuid);

        int? resetId = default;
        if (previousEntity == null)
            return l.Return((null, null), "no previous entity found");


        l.A("found previous entity, will check types/ids/attributes");
        CompareTypes(count, previousEntity, newEntity);

        // for saving, ensure we are using the DB entity-ID 
        if (newEntity.EntityId == 0)
        {
            l.A("found existing entity - will set the ID to that to overwrite");
            resetId = previousEntity.EntityId;
        }

        CompareIdentities(count, previousEntity, newEntity);
        CompareAttributes(count, previousEntity, newEntity);

        var exception = BuildExceptionIfHasIssues(Errors, l, "EntityIsOk() done");

        return l.Return((resetId, exception), "ok");
    }


    private void CompareTypes(int count, IEntity originalEntity, IEntity newEntity)
    {
        var l = Log.Fn($"ids:{newEntity.Type.NameId}/{originalEntity.Type.NameId}");
        if (originalEntity.Type.NameId != newEntity.Type.NameId)
            Add($"entity type mismatch on {count}");
        l.Done();
    }

    private void CompareIdentities(int count, IEntity originalEntity, IEntity newEntity)
    {
        var l = Log.Fn($"ids:{newEntity.EntityId}/{originalEntity.EntityId}");
        if (originalEntity.EntityId != newEntity.EntityId)
            Add($"entity ID mismatch on {count} - {newEntity.EntityId}/{originalEntity.EntityId}");

        l.A($"Guids:{newEntity.EntityGuid}/{originalEntity.EntityGuid}");
        if (originalEntity.EntityGuid != newEntity.EntityGuid)
            Add($"entity GUID mismatch on {count} - {newEntity.EntityGuid}/{originalEntity.EntityGuid}");
        l.Done();
    }

    private void CompareAttributes(int count, IEntity original, IEntity ent)
    {
        var l = Log.Fn();
        if (original.Attributes.Count != ent.Attributes.Count)
            Add($"entity {count} has different amount " +
                $"of attributes {ent.Attributes.Count} " +
                $"than the original {original.Attributes.Count}");
        else
            foreach (var origAttr in original.Attributes)
            {
                var newAttr = ent.Attributes.FirstOrDefault(a => a.Key == origAttr.Key);
                if (newAttr.Equals(default(KeyValuePair<string, IAttribute>)))
                    Add($"attribute {origAttr.Key} not found in save");
                else if (origAttr.Value.Type != newAttr.Value.Type)
                    Add($"found different type on attribute {origAttr.Key} " +
                        $"- '{origAttr.Value.Type}'/'{newAttr.Value.Type}'");
            }

        l.Done();
    }
}