using ToSic.Eav.Data.Processing;
using ToSic.Sys.DI;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Execute resolved generation jobs and collect failures without blocking the save flow.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CopilotSchemaExecuteBatchAction(
    FileSaver fileSaver,
    LazySvc<IEnumerable<IFileGenerator>> generators)
    : ServiceBase(SxcLogName + ".AutoGen.Execute", connect: [fileSaver, generators]),
        ILowCodeAction<CopilotGenerationBatch, CopilotGenerationBatch>
{
    public Task<ActionData<CopilotGenerationBatch>> Run(LowCodeActionContext mainCtx, ActionData<CopilotGenerationBatch> result)
    {
        var batch = result.Data;
        var l = Log.Fn<ActionData<CopilotGenerationBatch>>(
            $"origin:{batch.Trigger.Source}, app:{batch.Trigger.AppId}, type:{batch.Trigger.ContentTypeNameId}, jobs:{batch.Jobs.Count}");
        var errors = result.Exceptions.ToList();

        foreach (var job in batch.Jobs)
        {
            try
            {
                var generator = generators.Value.FirstOrDefault(g => g.Name.EqualsInsensitive(job.GeneratorName));
                if (generator == null)
                {
                    Log.A($"Copilot auto-generate: generator '{job.GeneratorName}' not found.");
                    errors.Add(new InvalidOperationException(
                        $"Generator '{job.GeneratorName}' not found for configuration '{job.ConfigurationId}'."));
                    continue;
                }

                (generator as IHasLog)?.LinkLog(Log);
                fileSaver.GenerateAndSaveFiles(generator, job.Specs);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                Log.Ex(ex);
            }
        }

        return Task.FromResult(l.Return(result with { Exceptions = errors }, $"processed:{batch.Jobs.Count}, errors:{errors.Count}"));
    }
}
