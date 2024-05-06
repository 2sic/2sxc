using ToSic.Eav.LookUp;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services.LookUp;

internal class TemplateService(): ServiceForDynamicCode($"{SxcLogName}.LUpSvc"), ITemplateService
{
    public ITemplateEngine Default(NoParamOrder protector = default, IEnumerable<ILookUp> sources = default)
    {
        var original = ((Apps.App)_CodeApiSvc.App).ConfigurationProvider;
        var originalEngine = sources != null
            ? new LookUpEngine(original, Log, sources: sources?.ToList())
            : original;
        return new TemplateEngine(originalEngine);
    }

    public ITemplateEngine Empty(NoParamOrder protector = default, IEnumerable<ILookUp> sources = null)
    {
        var originalEngine = new LookUpEngine(Log, sources: sources) as ILookUpEngine;
        return new TemplateEngine(originalEngine);
    }

    public ILookUp CreateSource(string name, IDictionary<string, string> values) 
        => new LookUpInDictionary(name, values);

    public ILookUp CreateSource(string name, ILookUp original)
        => new LookUpInLookUps(name, original);

    public ILookUp CreateSource(string name, Func<string, string> getter)
        => new LookUpWithFunction(name, getter);

    public ILookUp CreateSource(string name, IEntity entity, NoParamOrder protector = default, string[] dimensions = default)
        => new LookUpInEntity(name, entity, dimensions: dimensions ?? _CodeApiSvc.Cdf.Dimensions);

    public ILookUp CreateSource(string name, ITypedItem item, NoParamOrder protector = default, string[] dimensions = default) 
        => new LookUpInEntity(name, item.Entity, dimensions: dimensions ?? _CodeApiSvc.Cdf.Dimensions);
}