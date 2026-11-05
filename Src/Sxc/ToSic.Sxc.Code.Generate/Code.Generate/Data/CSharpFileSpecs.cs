using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

/// <summary>
/// 
/// </summary>
/// <param name="UserName">Gets the username for file generation</param>
internal record CSharpFileSpecs(
    CSharpCodeSpecs FileSetSpecs,
    IContentType Type,
    string UserName,
    string BaseName
)
{
    public string ClassName => $"{FileSetSpecs.SetSpecs.Prefix}{BaseName}{ClassSuffix}";

    /// <summary>
    /// Gets the class suffix to append to the main class name (e.g., "Model" for CustomModel)
    /// </summary>
    public string ClassSuffix => FileSetSpecs.SetSpecs.Suffix ?? "";
}
