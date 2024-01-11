using System.Collections.ObjectModel;

namespace ToSic.Sxc.Images;

internal class RecipeHelpers
{
    internal static ReadOnlyDictionary<string, object> MergeDics(IDictionary<string, object> parentOrNull, IDictionary<string, object> myOrNull)
    {
        if (myOrNull == null) return new(parentOrNull ?? new Dictionary<string, object>());
        if (parentOrNull == null || !parentOrNull.Any())
            return new(myOrNull);

        var newMaster = new Dictionary<string, object>(parentOrNull);

        foreach (var pair in myOrNull)
            if (pair.Value == null) newMaster.Remove(pair.Key);
            else newMaster[pair.Key] = pair.Value;

        return new(newMaster);
    }

}