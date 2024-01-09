using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeHelpDb
{
    /// <summary>
    /// Get a list containing the first help and various derived helps
    /// </summary>
    /// <param name="first"></param>
    /// <param name="funcs"></param>
    /// <returns></returns>
    public static List<CodeHelp> ManyHelps(CodeHelp first, params Func<CodeHelp, CodeHelp>[] funcs)
    {
        var result = new List<CodeHelp> { first };
        result.AddRange(funcs.Select(func => func(first)));
        return result;
    }


    /// <summary>
    /// Generate a list of help using help-objects, generator objects or list of help
    /// </summary>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static List<CodeHelp> BuildList(params object[] parts) =>
        parts?.SelectMany(r =>
        {
            switch (r)
            {
                case CodeHelp ch: return new[] { ch };
                case GenNotExist gen: return new[] { gen.Generate() };
                case IEnumerable<CodeHelp> list: return list;
                default: return Array.Empty<CodeHelp>();
            }
        }).ToList();
}