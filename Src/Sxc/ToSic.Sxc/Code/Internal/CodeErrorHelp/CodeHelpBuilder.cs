using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CodeHelpBuilder
{
    /// <summary>
    /// Get a list containing the first help and various derived helps
    /// </summary>
    /// <param name="first"></param>
    /// <param name="generators"></param>
    /// <returns></returns>
    public static List<CodeHelp> BuildVariations(CodeHelp first, params Func<CodeHelp, CodeHelp>[] generators)
    {
        var result = new List<CodeHelp> { first };
        result.AddRange(generators.Select(func => func(first)));
        return result;
    }


    /// <summary>
    /// Generate a list of help using help-objects, generator objects or list of help
    /// </summary>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static List<CodeHelp> BuildListFromDiverseSources(params object[] parts)
        => parts?.SelectMany(r => r switch
            {
                CodeHelp ch => new[] { ch },
                GenNotExist gen => new[] { gen.Generate() },
                IEnumerable<CodeHelp> list => list,
                _ => Array.Empty<CodeHelp>()
            })
            .ToList();
}