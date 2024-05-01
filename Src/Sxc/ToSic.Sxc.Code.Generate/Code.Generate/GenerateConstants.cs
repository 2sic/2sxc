using ToSic.Eav;

namespace ToSic.Sxc.Code.Generate;

/// <summary>
/// Constants used in the code generation process.
/// </summary>
/// <remarks>
/// WIP v17.04
/// </remarks>
[WorkInProgressApi("still being standardized")]
public class GenerateConstants
{
    /// <summary>
    /// Placeholder for the root of the app, which will be replaced with the actual path.
    ///
    /// It's usually used to prefix the target path of a generated file.
    /// </summary>
    public const string PathPlaceholderAppRoot = "[app:root]";

    /// <summary>
    /// Placeholder for the edition of the app, which will be replaced with the actual edition.
    ///
    /// It's usually used in the path of the generated file, to create edition-specific files.
    /// </summary>
    public const string PathPlaceholderEdition = "[target:edition]";

    public const string PathToAppCode = $"{PathPlaceholderAppRoot}/{PathPlaceholderEdition}/{Constants.AppCode}";
}