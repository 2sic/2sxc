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
                .Select(line => (line.Before("//") ?? line).TrimEnd())
                .ToList();
        return paramNames;
    }

}