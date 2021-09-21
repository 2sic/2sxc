using System;
using System.IO;
using ToSic.Eav.Run;
using ToSic.Sxc.Code;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Razor12<TModel>
    {

        public string CreateInstancePath
        {
            get => _createInstancePath ?? Path;
            set => _createInstancePath = value;
        }
        private string _createInstancePath;

        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <returns></returns>
        public dynamic CreateInstance(string virtualPath,
            string noParamOrder = ToSic.Eav.Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var directory = System.IO.Path.GetDirectoryName(Path);
            if (directory == null) throw new Exception("Current directory seems to be null");
            var path = System.IO.Path.Combine(directory, virtualPath);
            VerifyFileExists(path);
            
            return path.EndsWith(CodeCompiler.CsFileExtension)
                ? _DynCodeRoot.CreateInstance(path, noParamOrder, name, null, throwOnError)
                : CreateInstanceCshtml(path);
        }

        private dynamic CreateInstanceCshtml(string path)
        {
            throw new NotSupportedException("CreateInstance with .cshtml files is not supported in Oqtane. We're still trying to find a solution, but ATM it doesn't work. Use a .cs file instead. ");
        }

        protected void VerifyFileExists(string path)
        {
            var pathFinder = GetService<IServerPaths>();
            var finalPath = pathFinder.FullAppPath(path);
            if (!File.Exists(finalPath))
                throw new FileNotFoundException("The shared file does not exist.", path);
        }
    }
}
