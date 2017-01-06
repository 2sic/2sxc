using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.SexyContent.Edit.InPageEditingSystem;

namespace ToSic.SexyContent.ContentBlocks.Renderers
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
            var edit = new InPageEditingHelper(parent.SxcInstance);

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
                Guid guid;
                if (!Guid.TryParse(likelyGuid, out guid))
                    continue;

                object objFound;
                DynamicEntity subitem = null;
                var found = parent.TryGetMember(entityField, out objFound);

                if (found)
                {
                    var itms = (IList<DynamicEntity>)objFound;
                    if (itms.Count > 0)
                        subitem = itms.FirstOrDefault(i => i.EntityGuid == guid);
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