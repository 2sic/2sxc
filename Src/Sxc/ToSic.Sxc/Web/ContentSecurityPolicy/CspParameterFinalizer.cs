using System.Linq;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public class CspParameterFinalizer: HasLog
    {
        public CspParameterFinalizer(): base(CspConstants.LogPrefix + ".ParFin")
        {

        }

        public CspParameters Finalize(CspParameters original)
        {
            var wrapLog = Log.Call<CspParameters>();
            // Empty, skip
            if (original == null || !original.HasKeys()) return wrapLog("none", original);
            var merged = MergedWithAll(original);
            var deduped = DeduplicateValues(merged);
            return wrapLog("ok", deduped);
        }

        public CspParameters MergedWithAll(CspParameters original)
        {
            var wrapLog = Log.Call<CspParameters>();
            // Empty, skip
            if (original == null || !original.HasKeys()) return wrapLog("none", original);
            
            // Create copy and remove the All from it
            var copy = new CspParameters(original);
            copy.Remove(CspConstants.AllSrcName);

            // No values, skip
            var values = original
                .GetValues(CspConstants.AllSrcName)
                ?.Where(v => !string.IsNullOrWhiteSpace(v))
                .ToArray();
            if (values == null || values.Length == 0) return wrapLog("no values", copy);

            // Make sure the default exists, as it must get all values from the all
            copy.Add(CspConstants.DefaultSrcName, null);
            var keys = copy.AllKeys;
            foreach (var key in keys)
            foreach (var value in values)
                copy.Add(key, value);
            return wrapLog("ok", copy);
        }

        public CspParameters DeduplicateValues(CspParameters original)
        {
            var wrapLog = Log.Call<CspParameters>();
            // Empty, skip
            if (original == null || !original.HasKeys()) return wrapLog("none", original);
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

            return wrapLog($"deduped {dedupeCount}", copy);
        }
        
    }
}
