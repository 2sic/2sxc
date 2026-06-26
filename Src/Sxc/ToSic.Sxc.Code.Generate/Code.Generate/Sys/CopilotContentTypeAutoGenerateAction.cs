using ToSic.Eav.Apps;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Models;

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
        ILowCodeAction<ContentTypeChange, ContentTypeChange>
{
    public Task<ActionData<ContentTypeChange>> Run(LowCodeActionContext mainCtx, ActionData<ContentTypeChange> data)
    {
        var change = data.Data;
        var l = Log.Fn<ActionData<ContentTypeChange>>($"change:{change}");

        var errors = data.Exceptions.ToList();
        var appReader = appReaders.Get(change.AppId);
        var changedType = appReader.GetContentType(change.ContentTypeNameId);

        var jobs = appReader.List
            .GetModels<IDataCopilotConfiguration>()
            .Where(configuration => configuration.AutoGenerate)
            .Select(configuration => BuildJob(configuration, changedType))
            .OfType<CopilotCodeGenerateService.Job>()
            .ToList();

        if (jobs.Count == 0)
        {
            l.A($"Copilot auto-generate: no matching configurations for content-type '{changedType.NameId}' ({change.Source}).");
            return Task.FromResult(l.Return(data with { Exceptions = errors }, "no matching auto-generate configurations"));
        }

        foreach (var job in jobs)
            errors.AddRange(codeGenerate.AutoGenerate(job));

        return Task.FromResult(l.Return(data with { Exceptions = errors }, $"processed {jobs.Count} configuration(s); errors:{errors.Count}"));
    }

    private static CopilotCodeGenerateService.Job? BuildJob(IDataCopilotConfiguration configuration, IContentType changedType)
    {
        var generatorName = CopilotCodeGenerateService.Sanitize(configuration.CodeGenerator);
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

        var specs = CopilotCodeGenerateService.BuildFileGeneratorSpecs(configuration, new()
        {
            AppId = changedType.AppId,
            ContentTypes = [changedType.NameId],
        });

        return new(configuration.Id, generatorName, specs);
    }

}