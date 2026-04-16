using ToSic.Eav.Models;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Typed trigger used inside the copilot low-code pipeline after the external
/// <see cref="IDataProcessor"/> contract has validated the schema event context.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal record CopilotSchemaTrigger(
    int AppId,
    string ContentTypeNameId,
    string Source);

/// <summary>
/// Materialized generation work for one schema trigger.
/// The resolve action prepares this once so the execute action only performs IO.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal record CopilotGenerationBatch(
    CopilotSchemaTrigger Trigger,
    IContentType? ChangedType,
    IReadOnlyList<CopilotGenerationJob> Jobs);

/// <summary>
/// Fully resolved generation job derived from one matching copilot configuration.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal record CopilotGenerationJob(
    int ConfigurationId,
    string GeneratorName,
    FileGeneratorSpecs Specs);
