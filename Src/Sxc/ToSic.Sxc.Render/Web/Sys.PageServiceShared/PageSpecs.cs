using ToSic.Sxc.Context;

namespace ToSic.Sxc.Web.Sys.PageServiceShared;

public class PageSpecs
{
    public const string AllowedUrlParameters = "allowedUrlParameters";
    public const string ExpectedUrlParameters = "expectedUrlParameters";

    internal Dictionary<string, string> Collected { get; } = new(StringComparer.InvariantCultureIgnoreCase);

    public void Set(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        if (value == null)
        {
            Collected.Remove(key);
            return;
        }
        Collected[key] = value;
    }

    public void AddCsv(string key, string more)
    {
        if (string.IsNullOrWhiteSpace(more))
            return;
        var addition = more.CsvToArrayWithoutEmpty();

        if (Collected.TryGetValue(key, out var existing))
        {
            // Deduplicate values, but only if the new value is not already in the list
            var existingValues = existing.Split(',');
            var missingAdditions = addition
                .Except(existingValues, StringComparer.InvariantCultureIgnoreCase);

            var combined = existingValues
                .Concat(missingAdditions)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            Set(key, string.Join(",", combined));
        }
        else
        {
            Set(key, string.Join(",", addition));
        }
    }

    public bool ContainsKey(string key)
        => Collected.ContainsKey(key);

    public string? Get(string key)
        => Collected.TryGetValue(key, out var value) ? value : null;

    public IEnumerable<string> GetUnexpected(string key, IParameters parameters)
    {
        var expected = Get(key) ?? "";
        var expectedList = expected.Split(',');
        
        // find all parameters which are not in the expected list
        var unexpected = parameters.Keys().Where(k => !expectedList.Contains(k, StringComparer.InvariantCultureIgnoreCase));

        return unexpected;
    }
}