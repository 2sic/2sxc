using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.InPageEditingSystem;

namespace ToSic.Sxc.Blocks.Renderers
{
    internal class InTextContentBlocks
    {
        // RegEx formulas
        static readonly Regex InlineCbDetector = new Regex("<hr[^>]+sxc[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static readonly Regex  GuidExtractor = new Regex("guid=\\\"([^\\\"]*)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        internal static string Render(DynamicEntity parent, string entityField, string textTemplate)
        {
            // do basic checking
            if (!InlineCbDetector.IsMatch(textTemplate))
                return textTemplate;

            var result = new StringBuilder();
            var charProgress = 0;

            var matches = InlineCbDetector.Matches(textTemplate);
            if (matches.Count == 0)
                return textTemplate;

            // create edit-object which is necessary for context attributes
            var edit = new InPageEditingHelper(parent._Dependencies.BlockOrNull);

            foreach (Match curMatch in matches)
            {
                // Get characters before the first match
                if (curMatch.Index > charProgress)
                    result.Append(textTemplate.Substring(charProgress, curMatch.Index - charProgress));
                charProgress = curMatch.Index + curMatch.Length;

                // get the infos we need to retrieve the value, get it. 
                var marker = curMatch.Value.Replace("\'", "\"");

                if (marker.IndexOf("sxc=\"sxc-content-block\"", StringComparison.Ordinal) == 0)
                    continue;

                var guidMatch = GuidExtractor.Match(marker);
                var likelyGuid = guidMatch.Groups[1].Value;

                // check if guid is valid
                if (!Guid.TryParse(likelyGuid, out var guid))
                    continue;

                DynamicEntity subitem = null;
                var itms = parent.Children(entityField);//, out var objFound);
                var found = itms.Any();

                if (found)
                {
                    //var itms = objFound as List<IDynamicEntity>;
                    if (itms.Count > 0)
                        subitem = itms.FirstOrDefault(i => i.EntityGuid == guid) as DynamicEntity;
                }

                result.Append(Simple.RenderWithEditContext(parent, subitem, entityField,  guid, edit));
            }

            // attach the rest of the text (after the last match)
            result.Append(textTemplate.Substring(charProgress));

            // Ready to finish, but first, ensure repeating if desired
            var finalResult = result.ToString();
            return finalResult;
        }
    }
}