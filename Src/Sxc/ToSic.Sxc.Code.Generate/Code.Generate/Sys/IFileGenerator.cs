namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Describes a file generator which can generate (code) files.
/// </summary>
[WorkInProgressApi("still being standardized")]
public interface IFileGenerator
{
    public FileGeneratorSpecs GeneratorSpecs { get; }

    /// <summary>
    /// Call to run the generator and get the files
    /// </summary>
    public IGeneratedFileSet[] Generate(IGenerateFileSetSpecs specs);
}