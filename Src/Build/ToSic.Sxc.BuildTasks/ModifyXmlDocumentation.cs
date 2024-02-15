using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ToSic.Sxc.BuildTasks
{
    public class ModifyXmlDocumentation : Task
    {
        [Required]
        public string XmlDocumentationPath { get; set; }

        public override bool Execute()
        {
            // Launches a dialog asking if you want to attach a debugger
            //System.Diagnostics.Debugger.Launch();

            try
            {
                var content = File.ReadAllText(XmlDocumentationPath);

                var modifiedContent = ReplaceXrefsWithLinks(content);

                modifiedContent = AddDocsLinkToTypesAndProperties(modifiedContent);

                File.WriteAllText(XmlDocumentationPath, modifiedContent);
                Log.LogMessage(MessageImportance.High, $"XML documentation modified successfully at: {XmlDocumentationPath}");
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }

            return true;
        }

        private static string ReplaceXrefsWithLinks(string content)
        {
            // regex pattern to handle method signatures with parentheses
            var pattern = @"\[(.*?)\]\(xref:((?:[^()]+|\((?:[^()]+|\([^()]*\))*\))*)\)";

            // Replacement method defined as a local function
            string Replacement(Match m)
            {
                var label = m.Groups[1].Value;
                var reference = m.Groups[2].Value;
                // Use label if provided, otherwise use reference as label
                var linkText = !string.IsNullOrEmpty(label) ? label : reference;
                return $@"<a href=""https://go.2sxc.org/xref17?xref={reference}"">{linkText}</a>";
            }

            var modifiedContent = Regex.Replace(content, pattern, new MatchEvaluator(Replacement));
            return modifiedContent;
        }

        private string AddDocsLinkToTypesAndProperties(string content)
        {
            var xdoc = XDocument.Parse(content);
            foreach (var member in xdoc.Descendants("member"))
            {
                try
                {
                    var memberName = member.Attribute("name")?.Value;

                    if (memberName == null
                        || !(memberName.StartsWith("T:") || memberName.StartsWith("P:"))
                        || memberName.Contains(".Internal.")
                        || memberName.Contains(".Integration.")) continue;

                    var summary = member.Element("summary");
                    if (summary == null) continue;

                    var summaryContent = summary.Value;
                    if (summaryContent.Contains("📖")) continue;

                    var reference = memberName.Substring(2);
                    // if it's a generic type, we need to remove the generic part and append *
                    //if (reference.Contains("`")) reference = reference.Substring(0,reference.LastIndexOf('`')) + "*";

                    var label = reference.Substring(reference.LastIndexOf('.') + 1);

                    summary.Add(new XElement("para", new XElement("a", new XAttribute("href", $"https://go.2sxc.org/xref17?xref={reference}"), "📖 " + label)));
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex);
                }
            }
            return xdoc.ToString();
        }
    }
}