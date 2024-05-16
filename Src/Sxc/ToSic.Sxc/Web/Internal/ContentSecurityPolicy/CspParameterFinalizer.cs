using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CspParameterFinalizer() : ServiceBase($"{CspConstants.LogPrefix}.ParFin")
{
    public CspParameters Finalize(CspParameters original)
    {
        var l = Log.Fn<CspParameters>();
        // Empty, skip
        if (original == null || !original.HasKeys()) return l.Return(original, "none");
        var merged = MergedWithAll(original);
        var deduped = DeduplicateValues(merged);
        return l.ReturnAsOk(deduped);
    }

    public CspParameters MergedWithAll(CspParameters original)
    {
        var l = Log.Fn<CspParameters>();
        // Empty, skip
        if (original == null || !original.HasKeys()) return l.Return(original, "none");
            
        // Create copy and remove the All from it
        var copy = new CspParameters(original);
        copy.Remove(CspConstants.AllSrcName);

        // No values, skip
        var values = original
            .GetValues(CspConstants.AllSrcName)
            ?.Where(v => !string.IsNullOrWhiteSpace(v))
            .ToArray();
        if (values == null || values.Length == 0) return l.Return(copy, "no values");

        // Make sure the default exists, as it must get all values from the all
        copy.Add(CspConstants.DefaultSrcName, null);

        // Get the keys which should receive the all-src params
        // Note that most end with -src, but some have -src-elem or something, so we use .Contains
        var existingSrcKeys = copy.AllKeys
            .Where(s => s.HasValue() && s.Contains(CspConstants.SuffixSrc));   
        foreach (var key in existingSrcKeys)
        foreach (var value in values)
            copy.Add(key, value);
        return l.ReturnAsOk(copy);
    }

    public CspParameters DeduplicateValues(CspParameters original)
    {
        var l = Log.Fn<CspParameters>();
        // Empty, skip
        if (original == null || !original.HasKeys()) return l.Return(original, "none");
        var copy = new CspParameters(original);
        var keys = copy.AllKeys;
        var dedupeCount = 0;
        foreach (var key in keys)
        {
            var values = copy.GetValues(key);
            if (values == null || values.Length == 0) continue;
            var groupsOfSameValues = values
                .SelectMany(v => v.Split(' '))
                .GroupBy(v => v)
                .ToList();

            // Don't modify if none are duplicate
            if (!groupsOfSameValues.Any(g => g.Count() > 1)) continue;
            copy.Remove(key);

            foreach (var value in groupsOfSameValues) 
                copy.Add(key, value.Key);
            dedupeCount++;
        }

        return l.Return(copy, $"deduped {dedupeCount}");
    }
        
}