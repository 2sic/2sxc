using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

public partial class HelpDbRazor
{

    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> RuntimeProblemsOnly => field ??=
    [
        new()
        {
            // Dynamic: Error when it doesn't find a method because the object is null; 
            Name = "Method on Object not found",
            Detect = "Cannot perform runtime binding on a null reference",
            LinkCode = "err-binding-on-null-reference",
            UiMessage = "Method not found - typically on a dynamic object.",
        },
    ];
}