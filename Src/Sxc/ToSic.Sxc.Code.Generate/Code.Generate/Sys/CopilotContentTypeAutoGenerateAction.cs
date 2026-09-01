using ToSic.Eav.Apps;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Models;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Runs Copilot code generation after content-type schema changes.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CopilotContentTypeAutoGenerateAction(
    CopilotCodeGenerateService codeGenerate,
    IAppReaderFactory appReaders)
    : ServiceBase(SxcLogName + ".AutoGen.CT", connect: [codeGenerate, appReaders]),
        IWork<ContentTypeChange, ContentTypeChange>
{
    public Task<Package<ContentTypeChange>> Handle(WorkContext mainCtx, Package<ContentTypeChange> package)
    {
        var change = package.Data;
        var l = Log.Fn<Package<ContentTypeChange>>($"change:{change}");

        var errors = package.Exceptions.ToList();
        var appReader = appReaders.Get(change.AppId);
        var changedType = appReader.GetContentType(change.ContentTypeNameId);

        var jobs = appReader.List
            .GetModels<IDataCopilotConfiguration>()
            .OfType<IDataCopilotConfiguration>()
            .Where(configuration => configuration.AutoGenerate)
            .Select(configuration => BuildJob(configuration, changedType))
            .OfType<CopilotCodeGenerateService.Job>()
            .ToList();

        if (jobs.Count == 0)
        {
            l.A($"Copilot auto-generate: no matching configurations for content-type '{changedType.NameId}' ({change.Source}).");
            return Task.FromResult(l.Return(package with { Exceptions = errors }, "no matching auto-generate configurations"));
        }

        foreach (var job in jobs)
            errors.AddRange(codeGenerate.AutoGenerate(job));

        return Task.FromResult(l.Return(package with { Exceptions = errors }, $"processed {jobs.Count} configuration(s); errors:{errors.Count}"));
    }

    private static CopilotCodeGenerateService.Job? BuildJob(IDataCopilotConfiguration configuration, IContentType changedType)
    {
        var generatorName = configuration.CodeGenerator.Trim();
        if (generatorName.IsEmptyOrWs())
            return null;

        switch (configuration.ContentTypeSet)
        {
            // Empty is default, meaning all in scope "Default" + app settings/resources
            case "" when changedType.Scope != ScopeConstants.Default && changedType.Name != "AppSettings" && changedType.Name != "AppResources":
            // Only on custom do we check specifically for the type name/id
            case "custom" when !(configuration.GetSelectedContentTypes() ?? []).Any(s => s.EqualsInsensitive(changedType.NameId)):
                return null;
        }

        // Don't restrict to the changed type. Narrowing it to the changed type would make every
        // entity-field on that type fall back to ITypedItem, because required content types wouldn't
        // be in the set. Leaving it null gives the configuration's own set - exactly what a manual
        // "generate data models" run uses.
        var specs = CopilotCodeGenerateService.BuildFileGeneratorSpecs(configuration, new()
        {
            AppId = changedType.AppId,
        });

        return new(configuration.Id, generatorName, specs);
    }

}