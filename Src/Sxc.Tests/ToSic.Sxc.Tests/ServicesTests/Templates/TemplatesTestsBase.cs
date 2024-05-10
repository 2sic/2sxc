using System.Collections.Generic;
using ToSic.Eav.LookUp;
using ToSic.Razor.Html5;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Tests.ServicesTests.Templates;

public class TemplatesTestsBase: TestBaseSxc
{
    protected ITemplateService GetTemplateServices() 
        => _service ??= GetService<ITemplateService>();
    private ITemplateService _service;


    protected ILookUp GetDicSource(string name = "dic")
    {
        return GetTemplateServices().CreateSource(name, new Dictionary<string, string>
        {
            { "Key1", "Val1" },
            { "Key2", "Val2" }
        });
    }

    protected ILookUp GetFnSource(string name = "fn")
    {
        return GetTemplateServices().CreateSource(name, s => s?.ToLowerInvariant() switch
        {
            "key1" => "Val1",
            "key2" => "Val2",
            _ => ""
        });
    }

    protected ILookUp GetFnNumberSourcesWithFormat(string name = "fn")
    {
        return GetTemplateServices().CreateSource(name, (s, f) =>
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

}