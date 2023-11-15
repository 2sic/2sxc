using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Html5;

namespace ToSic.Sxc.Dnn.Run.Capabilities
{
    internal class RoslynCompilerCapability
    {
        internal static bool CheckCsharpLangVersion(string version) => CsharpLangVersions.Any(v => v.StartsWith(version));

        private const string RoslynFileName = "csc.exe";

        // Uses a double-check locking pattern to ensure thread safety and performance.
        private static string[] CsharpLangVersions
        {
            get
            {
                if (_csharpLangVersions == null)
                    lock (LangVersionLock)
                        if (_csharpLangVersions == null)
                            _csharpLangVersions = GetCsharpLangVersions();
                return _csharpLangVersions;
            }
        }
        // Volatile keyword ensures that the most up-to-date value is always read from memory, which is crucial for the correct functioning of the double-check locking pattern.
        private static volatile string[] _csharpLangVersions;
        private static readonly object LangVersionLock = new object();

        // Run csc.exe to get list of language versions supported by the csharp compiler.
        private static string[] GetCsharpLangVersions()
        {
            var roslynFolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/bin/roslyn/");
            if (string.IsNullOrEmpty(roslynFolderPath)) return Array.Empty<string>();

            try
            {
                var roslynCompilerPath = Path.Combine(roslynFolderPath, RoslynFileName);
                if (!File.Exists(roslynCompilerPath)) return Array.Empty<string>();

                // Run external applications from an ASP.NET application with in application directory,
                // with limited permissions and security restrictions of the ASP.NET application pool
                var startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = roslynFolderPath,
                    FileName = roslynCompilerPath,
                    Arguments = "-langversion:?",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process == null) return Array.Empty<string>();

                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    return output.SplitNewLine();
                }
            }
            catch
            {
                // ignored
                return Array.Empty<string>();
            }
        }
    }
}
