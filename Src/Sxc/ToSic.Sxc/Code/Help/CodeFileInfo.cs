using System.Collections.Generic;
using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Code.Help
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class CodeFileInfo
    {
        private CodeFileInfo(string inherits, CodeFileTypes type, List<CodeHelp> help)
        {
            Inherits = inherits;
            Type = type;
            Help = help ?? new List<CodeHelp>();
        }

        public string Inherits { get; }

        public CodeFileTypes Type { get; }
        public List<CodeHelp> Help { get; }

        public static CodeFileInfo CodeFileUnknown =
            new CodeFileInfo("unknown", CodeFileTypes.Unknown, CodeHelpDbUnknown.CompileUnknown);
        public static CodeFileInfo CodeFileOther =
            new CodeFileInfo("other", CodeFileTypes.Other, CodeHelpDbUnknown.CompileUnknown);

        public static CodeFileInfo CodeFileNotFound =
            new CodeFileInfo("", CodeFileTypes.FileNotFound, new List<CodeHelp>());

        public static List<CodeFileInfo> CodeFileList = new List<CodeFileInfo>
        {
            CodeFileUnknown,
            CodeFileOther,
            new CodeFileInfo("Custom.Hybrid.Razor12", CodeFileTypes.V12, CodeHelpDbV12.Compile12),
            new CodeFileInfo("Custom.Hybrid.Razor14", CodeFileTypes.V14, CodeHelpDbV14.Compile14),
            new CodeFileInfo("Custom.Hybrid.RazorTyped", CodeFileTypes.V16, CodeHelpDbV16.Compile16),
        };
    }
}
