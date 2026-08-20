using System;
using System.IO;
using ToSic.Sxc.Oqt.Server.Installation;

namespace ToSic.Sxc.Oqt.Tests.Installation;

public class HotReloadEnabledCheckTests
{
    private static string TempJson(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"launchSettings-{Guid.NewGuid():N}.json");
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void DisablesHotReloadWhenMissing()
    {
        var file = TempJson("""
            { "profiles": {
                "IIS Express": { "commandName": "IISExpress" },
                "Oqtane.Server": { "commandName": "Project", "hotReloadEnabled": true }
            } }
            """);

        Assert.True(HotReloadEnabledCheck.AddHotReloadPropertyWhenIsMissing(file));

        var result = File.ReadAllText(file);
        // Assert.DoesNotContain("true", result);
        Assert.Equal(2, result.Split("\"hotReloadEnabled\"").Length - 1);

        // already disabled everywhere => no rewrite
        Assert.False(HotReloadEnabledCheck.AddHotReloadPropertyWhenIsMissing(file));

        File.Delete(file);
    }

    [Fact]
    public void IgnoresMissingOrBrokenFiles()
    {
        Assert.False(HotReloadEnabledCheck.AddHotReloadPropertyWhenIsMissing(Path.Combine(Path.GetTempPath(), "does-not-exist.json")));

        var noProfiles = TempJson("""{ "iisSettings": {} }""");
        Assert.False(HotReloadEnabledCheck.AddHotReloadPropertyWhenIsMissing(noProfiles));
        File.Delete(noProfiles);

        var broken = TempJson("not json");
        Assert.False(HotReloadEnabledCheck.AddHotReloadPropertyWhenIsMissing(broken));
        File.Delete(broken);
    }
}
