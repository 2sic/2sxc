using System;
using System.IO;
using System.Text.RegularExpressions;
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
    }
}