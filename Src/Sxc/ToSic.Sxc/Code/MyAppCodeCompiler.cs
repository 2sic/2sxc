using System;
using System.IO;
using System.Reflection;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code
{
    public abstract class MyAppCodeCompiler(/*IServiceProvider serviceProvider*/) : ServiceBase("Sxc.MyApCd") // CodeCompiler(serviceProvider)
    {
        public const string CsFiles = ".cs";
        public const bool UseSubfolders = false;
        public const string MyAppCodeDll = "MyApp.Code.dll";

        protected internal abstract AssemblyResult GetAppCode(string relativePath, int appId = 0);

        protected (string[] SourceFiles, AssemblyResult ErrorResult) GetSourceFilesOrError(string fullPath)
        {
            var l = Log.Fn<(string[], AssemblyResult)>(timer: true);

            var sourceFiles = Directory.GetFiles(fullPath, $"*{CsFiles}", UseSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            // Log all files
            foreach (var sourceFile in sourceFiles) l.A(sourceFile);

            // Validate are there any C# files
            // TODO: if no files exist, it shouldn't be an error, because it could be that it's just not here yet
            return sourceFiles.Length == 0 
                ? l.ReturnAsError((sourceFiles, new AssemblyResult(errorMessages: $"Error: given path '{fullPath}' doesn't contain any {CsFiles} files"))) : 
                l.ReturnAsOk((sourceFiles, null));
        }

        /// <summary>
        /// Generates a random name for a dll file and ensures it does not already exist in the "2sxc.bin" folder.
        /// </summary>
        /// <returns>The generated random name.</returns>
        protected virtual string GetAppCodeDllName(string folderPath, int appId)
        {
            var l = Log.Fn<string>($"{nameof(folderPath)}: '{folderPath}'; {nameof(appId)}: {appId}", timer: true);
            string randomNameWithoutExtension;
            do
            {
                randomNameWithoutExtension = $"App-{appId:0000}-{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
            }
            while (File.Exists(Path.Combine(folderPath, $"{randomNameWithoutExtension}.dll")));
            return l.ReturnAsOk(randomNameWithoutExtension);
        }


        /// <summary>
        /// Normalize full file or folder path, so it is without redirections like "../" in "dir1/dir2/../file.cs"
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        protected static string NormalizeFullPath(string fullPath) => new FileInfo(fullPath).FullName;


        protected bool LogAllTypes(Assembly assembly)
        {
            var l = Log.Fn<bool>(assembly?.FullName);
            if (assembly == null) return l.ReturnFalse("no assembly");

            foreach (var type in assembly.GetTypes()) l.A(type.FullName);

            return l.ReturnTrue();
        }
    }
}
