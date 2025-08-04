using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

/// <summary>
/// The help DB for Razor issues related to DNN only. 
/// </summary>
partial class HelpDbRazor
{
    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> DnnMissingInHybrid => field ??=
    [
        new()
        {
            Name = "Object-Dnn-Not-In-Hybrid",
            Detect = @"error CS0118: 'Dnn' is a 'namespace' but is used like a 'variable'",
            UiMessage = $"""
                         You are probably trying to use the 'Dnn' object which is not supported in 'Custom.Hybrid.Razor' templates. 

                         """,
            DetailsHtml = $"""
                           You are probably trying to use the <code>Dnn</code> object which is not supported in <code>Custom.Hybrid.Razor</code> templates. Use: 
                           <ol>
                               <li>Other APIs such as <code>CmsContext</code> to get page/module etc. information</li>
                               <li>If really necessary (not recommended) use the standard Dnn APIs to get the necessary objects.</li>
                           </ol>

                           """
        },
    ];
}
