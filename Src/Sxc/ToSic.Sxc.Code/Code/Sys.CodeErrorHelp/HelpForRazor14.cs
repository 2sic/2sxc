﻿using ToSic.Sys.Code.Help;
using static ToSic.Sxc.Code.Sys.CodeErrorHelp.HelpForRazor12;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class HelpForRazor14
{
    [field: AllowNull, MaybeNull]
    internal static List<CodeHelp> Compile14 => field ??= CodeHelpBuilder.BuildListFromDiverseSources(
        // All which are common in 12 and 14
        Issues12To14,

        // use `Convert`
        SystemConvertIncorrectUse,

        // Use Dnn
        DnnObjectNotInHybrid,

        // v16.01 AsTyped / AsTypedList
        HelpRemoved14("AsTyped", "brc-1602", null),
        HelpRemoved14("AsTypedList", "brc-1602", null)
    );


    internal static CodeHelp HelpRemoved14(string property, string linkCode, params (string Code, string? Comment)[]? alt)
        => new GenNotExist(property, alt)
        {
            LinkCode = linkCode,
            MsgNotSupportedIn = "was a bad design choice in Razor14 and had to be removed - see link",
        }.Generate();


    internal static CodeHelp SystemConvertIncorrectUse = new()
    {
        Name = "System.Convert-Incorrect-Use",
        Detect = "error CS0117: 'System.Convert' does not contain a definition for",
        LinkCode = null,
        UiMessage = @"
You are probably calling Convert.ToXXX(...), so you probably want to use either 'System.Convert' or the Sxc 'IConvertService'. 
Older Razor/WebApi classes provided the IConvertService on an an object called 'Convert' which caused confusions, which could be why you see this error. ",
        DetailsHtml = @"
You seem to be calling <code>Convert.To...(...)</code>, so you probably want to use either 'System.Convert' or the Sxc 'IConvertService'. <br>
Older Razor/WebApi classes provided the IConvertService on an an object called 'Convert' which caused confusions. <br>
<ol>
    <li>If you want <code>System.Convert</code> make sure your code is correct (see MS docs) </li>
    <li>If you want the <code>IConvertService</code> use <code>Kit.Convert.To...(...)</code> </li>
</ol>"
    };

}