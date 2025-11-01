using ToSic.Eav.LookUp;
using ToSic.Eav.LookUp.Sources;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Services.Template;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Templates;

internal class TemplateService(LazySvc<ILookUpEngineResolver> getLookupsLazy) : ServiceWithContext($"{SxcLogName}.LUpSvc"), ITemplateService
{
    #region Get Engine Default / Empty

    public ITemplateEngine Default(NoParamOrder protector = default, IEnumerable<ILookUp>? sources = default)
    {
        var sourcesList = sources?.ToList();
        var noSources = sourcesList == null || sourcesList.Count == 0;
        if (_default != null && noSources)
            return _default;

        // in some cases, like when testing, the _CodeApiSvc is not available
        // then it should still work, but of course not know about the app's sources
        var original = ((IExCtxLookUpEngine?)ExCtxOrNull)?.LookUpForDataSources // #DropAppConfigurationProvider ((Apps.App?)ExCtxOrNull?.GetApp())?.ConfigurationProvider
            ?? getLookupsLazy.Value.GetLookUpEngine(ExCtxOrNull?.GetState<IBlock>()?.Context?.Module?.Id ?? 0);
        
        return noSources
            ? _default = new TemplateEngineTokens(original)
            : new TemplateEngineTokens(new LookUpEngine(original, Log, sources: sourcesList));
    }

    private ITemplateEngine? _default;

    public ITemplateEngine Empty(NoParamOrder protector = default, IEnumerable<ILookUp>? sources = null)
    {
        var sourcesList = sources?.ToList();
        var noSources = sourcesList == null || sourcesList.Count == 0;
        if (_empty != null && noSources)
            return _empty;

        ILookUpEngine originalEngine = new LookUpEngine(Log, sources: sourcesList);
        
        return noSources
            ? _empty ??= new TemplateEngineTokens(originalEngine)
            : new TemplateEngineTokens(originalEngine);
    }
    private ITemplateEngine? _empty;

    #endregion

    #region GetSource

    public ILookUp? GetSource(string name)
        => Default().GetSources(depth: 10).FirstOrDefault(s => s.Name.EqualsInsensitive(name));


    #endregion

    #region Quick Parse

    [field: AllowNull, MaybeNull]
    private ITemplateEngine Engine => field ??= Default();


    string ITemplateService.Parse(string template, NoParamOrder protector, bool allowHtml, IEnumerable<ILookUp>? sources)
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

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= ExCtx.GetCdf();

    public ILookUp CreateSource(string name, ICanBeEntity item, NoParamOrder protector = default, string[]? dimensions = default) 
        => new LookUpInEntity(name, item.Entity, dimensions: dimensions ?? Cdf.Dimensions);

    public ILookUp MergeSources(string name, IEnumerable<ILookUp>? sources)
    {
        var sourceList = sources
                             ?.ToList()
                         ?? throw new ArgumentNullException(nameof(sources), @"Sources must not be null");

        if (!sourceList.Any())
            throw new ArgumentException(@"Sources must not be empty", nameof(sources));

        return new LookUpInLookUps(name, sourceList);
    }

    private string NameOrErrorIfBad(string name)
        => string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException(@"Name must not be empty", nameof(name))
            : name.ToLowerInvariant();

    #endregion

    #region Create Templated Entity

    public ITypedItem ParseAsItem(ICanBeEntity original, NoParamOrder protector = default, bool allowHtml = false, ITemplateEngine? parser = null, IEnumerable<ILookUp>? sources = null)
    {
        var entity = original.Entity;
        if (entity == null)
            throw new ArgumentException(@"The original item must have an entity", nameof(original));

        parser ??= Empty(sources: sources);

        var templated = new TypedItemOfEntityWithOverrides(entity, Cdf, true, new ValueTemplateParser(parser));
        return templated;
    }

    public T ParseAs<T>(ICanBeEntity original, NoParamOrder protector = default, bool allowHtml = false, ITemplateEngine? parser = null, IEnumerable<ILookUp>? sources = null)
        where T : class, ICanWrapData
    {
        var templated = ParseAsItem(original, protector, allowHtml, parser, sources);
        return Cdf.AsCustom<T>(source: templated);
    }


    private class ValueTemplateParser(ITemplateEngine? parser, bool allowHtml = false) : IValueOverrider
    {
        public string? String(string name, string? originalValue)
            => originalValue == null || parser == null
                ? null
                : parser.Parse(originalValue, allowHtml: allowHtml);
    }
    #endregion


}

