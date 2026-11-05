using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal interface ICodeGenProperty
{
    /// <summary>
    /// What data type this code fragment generator will convert.
    /// </summary>
    ValueTypes ForDataType { get; }

    /// <summary>
    /// Create the code fragment(s)
    /// </summary>
    /// <param name="fieldDef">The field definition to generate code for.</param>
    /// <param name="tabs">The number of tabs to indent the generated code.</param>
    /// <returns>A list of code fragments.</returns>
    List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs);
}