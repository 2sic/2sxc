using System.Collections.Generic;
using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CodeHelpDbUnknown
{
    internal static CodeHelp UnknownNamespace = new(name: "unknown-ns",
        detect: "error CS0234: The type or namespace name",
        uiMessage: @"
Your code seems to have an invalid namespace - eg as a '@using xxx' or '@inherits xxx'. Check and fix your code.
",
        detailsHtml: @"
Your code seems to have an invalid namespace - eg as a <code>@using xxx</code> or <code>@inherits xxx</code>. Check and fix your code.
");

    internal static List<CodeHelp> CompileUnknown = new()
    {
        // error can't find a namespace
        UnknownNamespace,
    };
}