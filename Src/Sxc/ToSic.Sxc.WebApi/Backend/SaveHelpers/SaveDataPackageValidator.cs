using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.WebApi.Sys.Helpers.Validation;

namespace ToSic.Sxc.Backend.SaveHelpers;

/// <summary>
/// Check for any major discrepancies in the edit-save package,
/// especially missing headers or inconsistent group assignments.
/// </summary>
/// <param name="parentLog"></param>
internal class SaveDataPackageValidator(ILog parentLog) : ValidatorBase(parentLog, "Val.PackOk")
{
    /// <summary>
    /// The package format for loading and saving are the same, but we want to make sure
    /// that the save package doesn't contain unexpected trash (which would indicate the UI was broken)
    /// or that invalid combinations get back here
    /// </summary>
    /// <returns></returns>
    internal HttpExceptionAbstraction? ContainsOnlyExpectedNodes(EditSaveDto package)
    {
        var l = Log.Fn<HttpExceptionAbstraction?>();

        // check that items are mostly intact
        if (package.Items == null! || package.Items.Count == 0)
            Add("package didn't contain items, unexpected!");
        else
        {
            // do various validity tests on items
            VerifyAllGroupAssignmentsValid(package.Items);
            ValidateEachItemInBundle(package.Items);
        }

        BuildExceptionIfHasIssues(out var preparedException, "ContainsOnlyExpectedNodes() done");
        return l.Return(preparedException);
    }

    /// <summary>
    /// Do various validity checks on each item
    /// </summary>
    private void ValidateEachItemInBundle(IList<BundleWithHeader<JsonEntity>> list)
    {
        var l = Log.Fn($"{list.Count}");
        foreach (var item in list)
        {
            if (item.Header == null! /* paranoid */ || item.Entity == null!)
                Add($"item {list.IndexOf(item)} header or entity is missing");
            else if (item.Header.Guid != item.Entity.Guid) // check this first (because .Group may not exist)
            {
                if (!item.Header.IsContentBlockMode)
                    Add($"item {list.IndexOf(item)} has guid mismatch on header/entity, and doesn't have a group");
                else if (!item.Header.IsEmpty)
                    Add($"item {list.IndexOf(item)} header / entity guid miss match");
                // otherwise we're fine
            }
        }
        l.Done();
    }

    /// <summary>
    /// ensure all want to save to the same assignment type - either all in group or none!
    /// </summary>
    private void VerifyAllGroupAssignmentsValid(IReadOnlyCollection<BundleWithHeader<JsonEntity>> list)
    {
        var l = Log.Fn($"{list.Count}");
        var groupAssignments = list
            .Select(i => i.Header.ContentBlockAppId)
            .Where(g => g != null)
            .ToList();
        if (groupAssignments.Count == 0)
        {
            l.Done("none of the items is part of a list/group");
            return;
        }

        if (groupAssignments.Count != list.Count)
            Add($"Items in package with group: {groupAssignments} " +
                $"- should be 0 or {list.Count} (items in list) " +
                "- must stop, never expect items to come from different sources");
        else
        {
            var firstInnerContentAppId = groupAssignments.First();
            if (list.Any(i => i.Header.ContentBlockAppId != firstInnerContentAppId))
                Add("not all items have the same Group.ContentBlockAppId - this is required when using groups");
        }

        l.Done("done");
    }

}