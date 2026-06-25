using ToSic.Eav.Apps;
using ToSic.Eav.Data.Processing;

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
            .GetAll(CopilotCodeGenerateService.DataCopilotConfigurationContentType)
            .Where(configuration => configuration.Get<bool>(CopilotCodeGenerateService.FieldAutoGenerate))
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

    private static CopilotCodeGenerateService.Job? BuildJob(IEntity configuration, IContentType changedType)
    {
        var generatorName = CopilotCodeGenerateService.Sanitize(
            configuration.Get<string>(CopilotCodeGenerateService.FieldCodeGenerator));
        if (generatorName.IsEmptyOrWs())
            return null;

        var selectedTypes = CopilotCodeGenerateService.GetSelectedContentTypes(configuration);
        if (selectedTypes != null && !selectedTypes.Any(selection => selection.EqualsInsensitive(changedType.NameId)))
            return null;

        var specs = CopilotCodeGenerateService.BuildFileGeneratorSpecs(configuration, new()
        {
            AppId = changedType.AppId,
            ContentTypes = selectedTypes,
        });

        return new(configuration.EntityId, generatorName, specs);
    }
}
