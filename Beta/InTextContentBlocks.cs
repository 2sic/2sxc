using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.SexyContent.Edit.InPageEditingSystem;

namespace ToSic.SexyContent.Beta
{
    public class InTextContentBlocks
    {
        private readonly IInPageEditingSystem _edit;

        // RegEx formulas
        static readonly Regex InlineCbDetector = new Regex("<hr[^>]+sxc[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        static readonly Regex  GuidExtractor = new Regex("guid=\\\"([^\\\"]*)\\\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public InTextContentBlocks(IInPageEditingSystem edt)
        {
            _edit = edt;
        }

        private string InTextBlock(dynamic parent, string entityField, Guid guid, DynamicEntity subItem)
        {
            var area = "<div class='sc-content-block-list show-placeholder single-item' " + _edit.ContextAttributes(parent, field: entityField, newGuid: guid) + ">"
                + (subItem?.Render().ToString() ?? "")
                + "</div>";
            return area;
        }

        public string RenderWithInnerContent(DynamicEntity content, string sourceText, string entityField)
        {
            // do basic checking
            if (!InlineCbDetector.IsMatch(sourceText))
                return sourceText;

            var result = new StringBuilder();
            var charProgress = 0;

            var matches = InlineCbDetector.Matches(sourceText);
            if (matches.Count == 0)
                return sourceText;

            foreach (Match curMatch in matches)
            {
                // Get characters before the first match
                if (curMatch.Index > charProgress)
                    result.Append(sourceText.Substring(charProgress, curMatch.Index - charProgress));
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
                var found = content.TryGetMember(entityField, out objFound);

                if (found)
                {
                    var itms = (IList<DynamicEntity>)objFound;
                    if (itms.Count > 0)
                        subitem = itms.FirstOrDefault(i => i.EntityGuid == guid);
                }

                result.Append(InTextBlock(content, entityField, guid, subitem));
            }

            // attach the rest of the text (after the last match)
            result.Append(sourceText.Substring(charProgress));

            // Ready to finish, but first, ensure repeating if desired
            var finalResult = result.ToString();
            return finalResult;
        }
    }
}