using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using System.Web.Hosting;
using ToSic.Sxc.Code.Help;

namespace ToSic.Sxc.Engines
{
    public class DnnRazorSourceAnalyzer: ServiceBase
    {
        public DnnRazorSourceAnalyzer() : base("Dnn.RzrSrc")
        {
        }

        public CodeFileTypes FindType(string virtualPath)
        {
            var l = Log.Fn<CodeFileTypes>($"{nameof(virtualPath)}: '{virtualPath}'");

            if (virtualPath.IsEmptyOrWs()) 
                return l.Return(CodeFileTypes.FileNotFound, "no path");

            var path = HostingEnvironment.MapPath(virtualPath);
            if (path == null || path.IsEmptyOrWs())
                return l.Return(CodeFileTypes.FileNotFound, "no path");

            if (!File.Exists(path))
                return l.Return(CodeFileTypes.FileNotFound, "file not found");

            var contents = File.ReadAllText(path);

            if (contents.Length < 10)
                return l.Return(CodeFileTypes.Unknown, "file too short");

            var inheritsMatch = Regex.Match(contents, @"@inherits\s+(?<BaseName>[\w\.]+)", RegexOptions.Multiline);

            if (!inheritsMatch.Success)
                return l.Return(CodeFileTypes.Unknown, "no namespace found");

            var ns = inheritsMatch.Groups["BaseName"].Value;
            if (ns.IsEmptyOrWs())
                return l.Return(CodeFileTypes.Unknown);

            return RazorMap.TryGetValue(ns, out var razorType) 
                ? l.ReturnAndLog(razorType) 
                : l.Return(CodeFileTypes.Other, $"namespace '{ns}' can't be found");
        }

        private static readonly Dictionary<string, CodeFileTypes> RazorMap = new Dictionary<string, CodeFileTypes>
        {
            { "Custom.Hybrid.Razor12", CodeFileTypes.V12 },
            { "Custom.Hybrid.Razor14", CodeFileTypes.V14 },
            { "Custom.Hybrid.Razor16", CodeFileTypes.V16 }
        };
    }
}
