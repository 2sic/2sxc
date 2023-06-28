using System.Collections.Generic;
using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Code.Help
{
    public class CodeHelpDb
    {
        public static Dictionary<CodeFileTypes, List<CodeHelp>> CompileHelp = new Dictionary<CodeFileTypes, List<CodeHelp>>
        {
            [CodeFileTypes.Unknown] = CodeHelpDbUnknown.CompileUnknown,
            [CodeFileTypes.Other] = CodeHelpDbUnknown.CompileUnknown,
            [CodeFileTypes.V12] = CodeHelpDbV12.Compile12,
            [CodeFileTypes.V14] = CodeHelpDbV14.Compile14,
            [CodeFileTypes.V16] = CodeHelpDbV16.Compile16,
        };
    }
}
