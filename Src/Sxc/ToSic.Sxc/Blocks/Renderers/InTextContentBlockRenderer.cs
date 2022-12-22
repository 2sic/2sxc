using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Blocks.Renderers
{
    public class InTextContentBlockRenderer: ServiceBase
    {
        private readonly SimpleRenderer _simpleRenderer;

        public InTextContentBlockRenderer(SimpleRenderer simpleRenderer):base(Constants.SxcLogName + ".RndTxt")
        {
            ConnectServices(
                _simpleRenderer = simpleRenderer
            );
        }

        // RegEx formulas
        static readonly Regex InlineCbDetector = new Regex("<hr[^>]+sxc[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static readonly Regex  GuidExtractor = new Regex("guid=\\\"([^\\\"]*)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        public string RenderMerge(DynamicEntity parent, string field, string textTemplate, IEditService edit)
        {
            var l = Log.Fn<string>($"{nameof(parent)}: {parent?.EntityId}, {nameof(field)}: '{field}'");
            // do basic checking
            if (!InlineCbDetector.IsMatch(textTemplate))
                return l.Return(textTemplate, "no inner content");

            var result = new StringBuilder();
            var charProgress = 0;

            var matches = InlineCbDetector.Matches(textTemplate);
            if (matches.Count == 0)
                return l.Return(textTemplate, "no inline block placeholders found");

            l.A($"Found {matches.Count} inner content placeholders");

            var items = parent.Children(field);

            foreach (Match curMatch in matches)
            {
                var l2 = l.Fn($"Match at text pos: {curMatch.Index}");
                // Get characters before the first match
                if (curMatch.Index > charProgress)
                    result.Append(textTemplate.Substring(charProgress, curMatch.Index - charProgress));
                charProgress = curMatch.Index + curMatch.Length;

                // get the infos we need to retrieve the value, get it. 
                var marker = curMatch.Value.Replace("\'", "\"");

                if (marker.IndexOf("sxc=\"sxc-content-block\"", StringComparison.Ordinal) == 0)
                {
                    l2.Done("marker is incomplete, won't process");
                    continue;
                }

                var guidMatch = GuidExtractor.Match(marker);
                var likelyGuid = guidMatch.Groups[1].Value;

                // check if guid is valid
                if (!Guid.TryParse(likelyGuid, out var guid))
                {
                    l2.Done("Marker can't be converted to guid, won't process");
                    continue;
                }

                //var found = items.Any();

                var subItem = items.Any() // found && items.Count > 0
                    ? items.FirstOrDefault(i => i.EntityGuid == guid) as DynamicEntity
                    : null;

                var contents = _simpleRenderer.RenderWithEditContext(parent, subItem, field, guid, edit);

                result.Append(contents);
                l2.Done();
            }

            // attach the rest of the text (after the last match)
            result.Append(textTemplate.Substring(charProgress));

            // Ready to finish, but first, ensure repeating if desired
            var finalResult = result.ToString();
            return l.Return(finalResult, "ok");
        }
    }
}