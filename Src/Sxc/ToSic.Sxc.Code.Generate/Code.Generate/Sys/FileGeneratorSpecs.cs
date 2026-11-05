using ToSic.Sxc.Sys;

namespace ToSic.Sxc.Code.Generate.Sys;

public record FileGeneratorSpecs(IFileGenerator Generator)
{
    /// <summary>
    /// Generator name, to select it in a list of generators.
    /// </summary>
    /// <remarks>
    /// Note that generator names should remain stable, because they are used in the generator-selection in the UI.
    /// If a generator is renamed, configurations will break.
    /// So if it is ever renamed, make sure this name is set there.
    /// </remarks>
    public string Name
    {
        get => field ??= Generator.GetType().Name;
        init;
    }

    /// <summary>
    /// Generator version for additional info when selecting the generator.
    /// </summary>
    public string Version { get; init; } = SxcSharedAssemblyInfo.AssemblyVersion;

    /// <summary>
    /// Generator description for selecting the generator.
    /// </summary>
    public required string Description { get; init; }

    public required string DescriptionHtml { get; init; }

    /// <summary>
    /// Language this generator creates, e.g. "CSharp", "TypeScript", "JavaScript"
    /// </summary>
    public required string OutputLanguage { get; init; }

    /// <summary>
    /// WIP, string name for what kind of output we'll generate.
    /// Not final yet, should be used in dialogs to only provide generators with the correct output type.
    /// </summary>
    public required string OutputType { get; init; }

}