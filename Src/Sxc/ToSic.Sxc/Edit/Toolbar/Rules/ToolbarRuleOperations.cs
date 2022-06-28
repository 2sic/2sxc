using System;
using System.Collections.Generic;
using ToSic.Eav.Plumbing;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOperations;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal enum ToolbarRuleOperations
    {
        OprAdd = '+',
        OprAuto = '±',
        OprModify = '%',
        OprRemove = '-',
        OprUnknown = '¿',
        OprNone = ' ',
    }

    internal class ToolbarRuleOps
    {

        internal static Dictionary<string, ToolbarRuleOperations> ToolbarRuleOpSynonyms =
            new Dictionary<string, ToolbarRuleOperations>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "modify", OprModify },
                { "add", OprAdd },
                { "auto", OprAuto },
                { "remove", OprRemove },
            };

        //internal static char FindInFlags(string flags, ToolbarRuleOperations defOp)
        //{
        //    if (!flags.HasValue()) return (char)defOp;

        //    var parts = flags.Split(',');
        //    foreach (var f in parts)
        //    {
        //        var maybeOp = Pick(f, OprUnknown);
        //        if (maybeOp != (char)OprUnknown) return maybeOp;
        //    }

        //    return (char)defOp;
        //}

        internal static char Pick(string op, ToolbarRuleOperations defOp)
        {
            if (!op.HasValue()) return (char)defOp;
            op = op.Trim();

            if (op.Length == 1 && Enum.IsDefined(typeof(ToolbarRuleOperations), (int)op[0]))
                return op[0];


            if (ToolbarRuleOpSynonyms.TryGetValue(op, out var foundSyn))
                return (char)foundSyn;

            return (char)defOp;
        }

    }
}
