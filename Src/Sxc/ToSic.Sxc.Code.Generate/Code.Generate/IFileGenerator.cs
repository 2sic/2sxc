using ToSic.Lib.Data;

namespace ToSic.Sxc.Code.Generate;

/// <summary>
/// Describes a file generator which can generate (code) files.
/// </summary>
[WorkInProgressApi("still being standardized")]
public interface IFileGenerator : IHasIdentityNameId
{
    /// <summary>
    /// Generator name, to select it in a list of generators
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Generator version for additional info when selecting the generator.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Generator description for selecting the generator.
    /// </summary>
    public string Description { get; }

    public string DescriptionHtml { get; }

    /// <summary>
    /// Language this generator creates, e.g. "CSharp", "TypeScript", "JavaScript"
    /// </summary>
    public string OutputLanguage { get; }

    /// <summary>
    /// WIP, string name for what kind of output we'll generate.
    /// Not final yet, should be used in dialogs to only provide generators with the correct output type.
    /// </summary>
    public string OutputType { get; }

    /// <summary>
    /// Call to run the generator and get the files
    /// </summary>
    public IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs);
}