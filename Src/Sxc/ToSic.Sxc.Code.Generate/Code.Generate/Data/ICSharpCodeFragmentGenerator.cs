namespace ToSic.Sxc.Code.Generate.Data;

/// <summary>
/// Describes a fragment generator, which provides the special parts for different types of CSharp Data file generators.
/// </summary>
/// <remarks>
/// ATM used for naked models and typed models.
/// </remarks>
internal interface ICSharpCodeFragmentGenerator
{
    /// <summary>
    /// Generates the file introduction comment with extension instructions
    /// </summary>
    /// <param name="fileSpecs">The specifications for the file being generated</param>
    /// <returns>The file introduction comment</returns>
    string GenerateFileIntroComment(CSharpFileSpecs fileSpecs);

    /// <summary>
    /// Generates the main class XML documentation comment
    /// </summary>
    /// <param name="fileSpecs">The specifications for the file being generated</param>
    /// <param name="firstPropertyName">The name of the first property for examples</param>
    /// <returns>The main class comment</returns>
    string GenerateMainClassComment(CSharpFileSpecs fileSpecs, string? firstPropertyName);

    /// <summary>
    /// Generates the auto-generated base class XML documentation comment
    /// </summary>
    /// <returns>The auto-generated class comment</returns>
    string GenerateAutoGenClassComment(CSharpFileSpecs fileSpecs);

}
