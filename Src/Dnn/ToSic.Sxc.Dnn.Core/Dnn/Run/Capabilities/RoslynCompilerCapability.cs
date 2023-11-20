using System;
using System.Linq;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Dnn.Run.Capabilities
{
    internal class RoslynCompilerCapability
    {
        internal static bool CheckCsharpLangVersion(int version) => CsharpLangVersions.Contains(value: version);

        // Uses a double-check locking pattern to ensure thread safety and performance.
        private static int[] CsharpLangVersions
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
        private static volatile int[] _csharpLangVersions = null;
        private static readonly object LangVersionLock = new object();

        // DETECT based on installed stuff (DLLs, available APIs?)
        // Goal is that it can tell if the newer CodeDom library has been installed or not
        // I'll then use it to build a config in the App, so the app can warn if a feature is missing
        private static int[] GetCsharpLangVersions()
        {
            var csharpLangVersions = new CSharpAssemblyHandling().GetLanguageVersions();
            if (string.IsNullOrEmpty(csharpLangVersions)) return Array.Empty<int>();
            return csharpLangVersions.Split(',').Select(int.Parse).ToArray();
        }
    }
}
