using ToSic.Razor.Blade;

namespace ToSic.Sxc.Configuration.Sys;

public class ConfigStringHelpers
{
    public static List<string> ConfigLinesWithoutComments(string config)
    {
        var paramNames = string.IsNullOrWhiteSpace(config)
            ? []
            : config
                .SplitNewLine()
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                .Select(line => (line.Before(" //") ?? line).TrimEnd())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();
        return paramNames;
    }

    public static List<(string Key, string? Values)> ConfigPairs(string config)
    {
        var lines = ConfigLinesWithoutComments(config);
        var pairs = lines
            .Select(line =>
            {
                var pair = line.Split('=');
                // it's very important that if there is no "=" then we must use null for the value
                // as that specifies that any value is possible, while an empty string would specify that only an empty value is possible
                var values = pair.Length == 2 ? pair[1].Trim() : null;
                return (Key: pair[0].Trim(), Values: values);
            })
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Key))
            .ToList();

        return pairs;
    }

}