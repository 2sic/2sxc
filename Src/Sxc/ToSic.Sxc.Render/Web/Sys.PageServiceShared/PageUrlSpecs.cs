using System.Collections.Specialized;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Web.Sys.PageServiceShared;

public class PageUrlSpecs
{

    internal Dictionary<string, UrlParameterSpecs> Specs { get; } = new(StringComparer.InvariantCultureIgnoreCase);

    public void Set(string key, string? values = null)
        => Set(key, values?.Split(','));

    public void Set(string key, IEnumerable<string>? values)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        foreach (var k in key.CsvToArrayWithoutEmpty())
            Specs[k] = new(k, values ?? []);
    }

    public void Add(string key, string? values = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        // Try to get previous values
        if (Specs.TryGetValue(key, out var existing))
        {
            // If there are existing values, combine them with the new ones
            var combinedValues = existing.Values
                .Concat((values ?? "").CsvToArrayWithoutEmpty())
                .Distinct(StringComparer.InvariantCultureIgnoreCase);

            Set(key, combinedValues);
        }
        else
        {
            // If there are no existing values, just set the new ones
            Set(key, values);
        }
    }
    
    public void Remove(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;
        Specs.Remove(key);
    }


    public bool ContainsKey(string key)
        => Specs.ContainsKey(key);

    public string Keys() => string.Join(",", Specs.Keys);

    public IEnumerable<string> Values(string key) =>
        Specs.TryGetValue(key, out var value)
            ? value.Values
            : [];

    public string? ValuesCsv(string key) =>
        Specs.TryGetValue(key, out var value) && value.Values.Any()
            ? string.Join(",", value.Values)
            : null;

    public IParameters GetInvalid(IParameters parameters) =>
        GetExtract(parameters, true);

    public IParameters GetValid(IParameters parameters) =>
        GetExtract(parameters, false);

    public IParameters GetExtract(IParameters parameters, bool getInvalid)
    {
        // find all parameters which are not in the expected list
        var unexpected = parameters
            .Where(pair => IsInvalid(pair.Key, pair.Value) == getInvalid);

        // convert to NameValueCollection and return as IParameters
        var nvc = new NameValueCollection();
        foreach (var pair in unexpected)
            nvc.Add(pair.Key, pair.Value);

        var result = new Context.Sys.Parameters { Nvc = nvc };
        return result;
    }

    private bool IsInvalid(string key, string value)
    {
        // Check if key exists - if not, it's unexpected
        if (!Specs.TryGetValue(key, out var spec))
            return true;

        // Spec found, but name-casing may be off.
        if (spec.Name != key || !spec.ForceNameLowerCase)
            return true;

        var values = spec.Values.ToArray();

        // If there are no expected values, then any value is acceptable
        if (!values.Any())
            return false;

        // Check if the parameter value is in the expected values
        var comparer = spec.ValuesCaseVariationAllowed
            ? StringComparer.InvariantCultureIgnoreCase
            : StringComparer.InvariantCulture;

        return !values.Contains(value, comparer);
    }
}