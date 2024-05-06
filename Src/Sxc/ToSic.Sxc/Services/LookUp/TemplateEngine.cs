using ToSic.Eav.LookUp;
using ToSic.Lib.Data;

namespace ToSic.Sxc.Services.LookUp;

internal class TemplateEngine(ILookUpEngine original): ITemplateEngine, IWrapper<ILookUpEngine>
{
    private const string TemplateKey = "Template";
    IEnumerable<ILookUp> ITemplateEngine.Sources => original.Sources;

    string ITemplateEngine.Parse(string template, NoParamOrder protector, IEnumerable<ILookUp> sources)
    {
        var dic = new Dictionary<string, string>
        {
            [TemplateKey] = template
        };
        var result = original.LookUp(dic, overrides: sources);
        return result[TemplateKey];
    }

    /// <summary>
    /// For now just on explicit implementation, for debug, without enlarging the public API
    /// </summary>
    /// <returns></returns>
    ILookUpEngine IWrapper<ILookUpEngine>.GetContents() => original;
}