using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Services.LookUp;

internal class TemplateEngine(ILookUpEngine original): ITemplateEngine
{
    private const string TemplateKey = "Template";
    public IEnumerable<ILookUp> Sources => original.Sources;

    public string Parse(string template)
    {
        var dic = new Dictionary<string, string>
        {
            [TemplateKey] = template
        };
        var result = original.LookUp(dic);
        return result[TemplateKey];
    }
}