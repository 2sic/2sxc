using System.Linq;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public class CspParameterFinalizer
    {
        public CspParameters Finalize(CspParameters original)
        {
            // Empty, skip
            if (original == null || !original.HasKeys()) return original;
            var merged = MergedWithAll(original);
            var deduped = DeduplicateValues(merged);
            return deduped;
        }

        public CspParameters MergedWithAll(CspParameters original)
        {
            // Empty, skip
            if (original == null || !original.HasKeys()) return original;
            
            // Create copy and remove the All from it
            var copy = new CspParameters(original);
            copy.Remove(CspService.AllSrcName);

            // No values, skip
            var values = original
                .GetValues(CspService.AllSrcName)
                ?.Where(v => !string.IsNullOrWhiteSpace(v))
                .ToArray();
            if (values == null || values.Length == 0) return copy;

            // Make sure the default exists, as it must get all values from the all
            copy.Add(CspService.DefaultSrcName, null);
            var keys = copy.AllKeys;
            foreach (var key in keys)
            foreach (var value in values)
                copy.Add(key, value);
            return copy;
        }

        public CspParameters DeduplicateValues(CspParameters original)
        {
            // Empty, skip
            if (original == null || !original.HasKeys()) return original;
            var copy = new CspParameters(original);
            var keys = copy.AllKeys;
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
            }

            return copy;
        }
        
    }
}
