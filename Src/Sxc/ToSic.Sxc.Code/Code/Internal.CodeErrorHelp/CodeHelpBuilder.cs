using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[ShowApiWhenReleased(ShowApiMode.Never)]
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
    [return: NotNullIfNotNull(nameof(parts))]
    public static List<CodeHelp>? BuildListFromDiverseSources(params object[] parts)
        => parts?.SelectMany(r => r switch
            {
                CodeHelp ch => [ch],
                GenNotExist gen => [gen.Generate()],
                IEnumerable<CodeHelp> list => list,
                _ => []
            })
            .ToList();
}