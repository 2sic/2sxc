using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

partial class HelpDbRazor
{
    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> ChangesInV14LikeConvert => field ??=
    [
        new()
        {
            Name = "System.Convert-Incorrect-Use",
            Detect = "error CS0117: 'System.Convert' does not contain a definition for",
            LinkCode = null,
            UiMessage = """

                        You are probably calling Convert.ToXXX(...), so you probably want to use either 'System.Convert' or the Sxc 'IConvertService'. 
                        Older Razor/WebApi classes provided the IConvertService on an an object called 'Convert' which caused confusions, which could be why you see this error. 
                        """,
            DetailsHtml = """

                          You seem to be calling <code>Convert.To...(...)</code>, so you probably want to use either 'System.Convert' or the Sxc 'IConvertService'. <br>
                          Older Razor/WebApi classes provided the IConvertService on an an object called 'Convert' which caused confusions. <br>
                          <ol>
                              <li>If you want <code>System.Convert</code> make sure your code is correct (see MS docs) </li>
                              <li>If you want the <code>IConvertService</code> use <code>Kit.Convert.To...(...)</code> </li>
                          </ol>
                          """
        }
    ];

    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> TryingToUseV16InV14AsItWasTemporarilyMixed => field ??=
    [
        // v16.01 AsTyped / AsTypedList
        HelpRemoved14("AsTyped", "brc-1602", null),
        HelpRemoved14("AsTypedList", "brc-1602", null),

    ];


    private static CodeHelp HelpRemoved14(string property, string linkCode, params (string Code, string? Comment)[]? alt)
        => new GenNotExist(property, alt)
        {
            LinkCode = linkCode,
            MsgNotSupportedIn = "was a bad design choice in Razor14 and had to be removed - see link",
        }.Generate();


}