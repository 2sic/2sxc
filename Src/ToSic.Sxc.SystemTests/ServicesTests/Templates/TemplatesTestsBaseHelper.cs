using ToSic.Eav.LookUp;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.Templates;

public class TemplatesTestsBaseHelper(ITemplateService templateSvc) 
{
    internal ILookUp GetDicSource(string name = "dic") =>
        templateSvc.CreateSource(name, new Dictionary<string, string>
        {
            { "Key1", "Val1" },
            { "Key2", "Val2" }
        });

    internal ILookUp GetFnSource(string name = "fn") =>
        templateSvc.CreateSource(name, s => s?.ToLowerInvariant() switch
        {
            "key1" => "Val1",
            "key2" => "Val2",
            _ => ""
        });

    internal ILookUp GetFnNumberSourcesWithFormat(string name = "fn") =>
        templateSvc.CreateSource(name, (s, f) =>
        {
            var result = s?.ToLowerInvariant() switch
            {
                "key1" => 42,
                "key2" => 27,
                _ => 0
            };
            return string.IsNullOrWhiteSpace(f)
                ? result.ToString()
                : result.ToString(f);
        });
}