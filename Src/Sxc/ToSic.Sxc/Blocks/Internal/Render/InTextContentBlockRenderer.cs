using System.Text;
using System.Text.RegularExpressions;
using ToSic.Lib.Services;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Blocks.Internal.Render;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class InTextContentBlockRenderer(SimpleRenderer simpleRenderer)
    : ServiceBase(SxcLogName + ".RndTxt", connect: [simpleRenderer])
{
    // RegEx formulas
    static readonly Regex InlineCbDetector = new("<hr[^>]+sxc[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
    static readonly Regex  GuidExtractor = new("guid=\\\"([^\\\"]*)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    public string RenderMerge(IBlock block, IEntity parent, string field, string textTemplate, IEditService edit)
    {
        var l = Log.Fn<string>($"{nameof(parent)}: {parent.EntityId}, {nameof(field)}: '{field}'");
        // do basic checking
        if (!InlineCbDetector.IsMatch(textTemplate))
            return l.Return(textTemplate, "no inner content");

        var result = new StringBuilder();
        var charProgress = 0;

        var matches = InlineCbDetector.Matches(textTemplate);
        if (matches.Count == 0)
            return l.Return(textTemplate, "no inline block placeholders found");

        l.A($"Found {matches.Count} inner content placeholders");

        //var items = parent.Children(field);
        var items = parent.Children(field);

        foreach (Match curMatch in matches)
            l.Do($"Match at text pos: {curMatch.Index}", l2 =>
            {
                // Get characters before the first match
                if (curMatch.Index > charProgress)
                    result.Append(textTemplate.Substring(charProgress, curMatch.Index - charProgress));
                charProgress = curMatch.Index + curMatch.Length;

                // get the infos we need to retrieve the value, get it. 
                var marker = curMatch.Value.Replace("\'", "\"");

                if (marker.IndexOf("sxc=\"sxc-content-block\"", StringComparison.Ordinal) == 0)
                    return "marker is incomplete, won't process";

                var guidMatch = GuidExtractor.Match(marker);
                var likelyGuid = guidMatch.Groups[1].Value;

                // check if guid is valid
                if (!Guid.TryParse(likelyGuid, out var guid))
                    return "Marker can't be converted to guid, won't process";

                var subItem = items.FirstOrDefault(i => i.EntityGuid == guid);

                var contents = simpleRenderer.RenderWithEditContext(block, parent, subItem, field, guid, edit);

                result.Append(contents);
                return "done";
            });

        // attach the rest of the text (after the last match)
        result.Append(textTemplate.Substring(charProgress));

        // Ready to finish, but first, ensure repeating if desired
        var finalResult = result.ToString();
        return l.Return(finalResult, "ok");
    }
}