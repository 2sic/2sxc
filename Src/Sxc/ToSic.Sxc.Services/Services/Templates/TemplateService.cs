using ToSic.Eav.LookUp.Sources;
using ToSic.Lib.DI;
using ToSic.Lib.LookUp;
using ToSic.Lib.LookUp.Engines;
using ToSic.Lib.LookUp.Sources;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services.Internal;
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
        var original = ((Apps.App?)ExCtxOrNull?.GetApp())?.ConfigurationProvider
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

}