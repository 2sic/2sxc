using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Services.Template;

namespace ToSic.Sxc.Services.Templates;

internal class TemplateService(LazySvc<ILookUpEngineResolver> getEngineLazy) : ServiceForDynamicCode($"{SxcLogName}.LUpSvc"), ITemplateService
{
    #region Get Engine Default / Empty

    public ITemplateEngine Default(NoParamOrder protector = default, IEnumerable<ILookUp> sources = default)
    {
        var sourcesList = sources?.ToList();
        var noSources = sourcesList == null || sourcesList.Count == 0;
        if (_default != null && noSources)
            return _default;

        // in some cases, like when testing, the _CodeApiSvc is not available
        // then it should still work, but of course not know about the app's sources
        var original = ((Apps.App)_CodeApiSvc?.App)?.ConfigurationProvider
            ?? getEngineLazy.Value.GetLookUpEngine(0);
        
        return noSources
            ? _default = new TemplateEngineTokens(original)
            : new TemplateEngineTokens(new LookUpEngine(original, Log, sources: sourcesList));
    }

    private ITemplateEngine _default;

    public ITemplateEngine Empty(NoParamOrder protector = default, IEnumerable<ILookUp> sources = null)
    {
        var sourcesList = sources?.ToList();
        var noSources = sourcesList == null || sourcesList.Count == 0;
        if (_empty != null && noSources)
            return _empty;

        var originalEngine = new LookUpEngine(Log, sources: sourcesList) as ILookUpEngine;
        
        return noSources
            ? _empty ??= new TemplateEngineTokens(originalEngine)
            : new TemplateEngineTokens(originalEngine);
    }
    private ITemplateEngine _empty;

    #endregion

    #region GetSource

    public ILookUp GetSource(string name)
        => Default().GetSources(depth: 10).FirstOrDefault(s => s.Name.EqualsInsensitive(name));


    #endregion

    #region Quick Parse

    private ITemplateEngine Engine => _engine ??= Default();
    private ITemplateEngine _engine;


    string ITemplateService.Parse(string template, NoParamOrder protector, bool allowHtml, IEnumerable<ILookUp> sources)
        => Engine.Parse(template, protector, allowHtml: allowHtml, sources: sources);

    #endregion

    #region Create Sources

    public ILookUp CreateSource(string name, IDictionary<string, string> values) 
        => new LookUpInDictionary(NameOrErrorIfBad(name), values);

    public ILookUp CreateSource(string name, ILookUp original)
        => new LookUpInLookUps(NameOrErrorIfBad(name), [original]);

    public ILookUp CreateSource(string name, Func<string, string> getter)
        => new LookUpWithFunction(NameOrErrorIfBad(name), getter);

    public ILookUp CreateSource(string name, Func<string, string, string> getter)
        => new LookUpWithFunctionAndFormat(NameOrErrorIfBad(name), getter);

    public ILookUp CreateSource(string name, ICanBeEntity item, NoParamOrder protector = default, string[] dimensions = default) 
        => new LookUpInEntity(name, item.Entity, dimensions: dimensions ?? _CodeApiSvc.Cdf.Dimensions);

    public ILookUp MergeSources(string name, IEnumerable<ILookUp> sources)
    {
        var sourceList = sources?.ToList()
                         ?? throw new ArgumentNullException(nameof(sources), @"Sources must not be null"); ;

        if (!sourceList.Any())
            throw new ArgumentException(@"Sources must not be empty", nameof(sources));

        return new LookUpInLookUps(name, sourceList);
    }

    private string NameOrErrorIfBad(string name)
        => string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name must not be empty", nameof(name))
            : name.ToLowerInvariant();

    #endregion

}