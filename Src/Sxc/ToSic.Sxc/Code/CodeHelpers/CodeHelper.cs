using ToSic.Lib.Logging;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code.CodeHelpers
{
    public class CodeHelper: CodeHelperBase
    {
        #region Setup

        public CodeHelper() : base("Sxc.CdHlp")
        {

        }

        public CodeHelper Init(DynamicCodeBase parent)
        {
            _parent = parent;
            return this;
        }

        private DynamicCodeBase _parent;

        #endregion

        #region CreateInstance

        public object GetCode(string path, string noParamOrder = Protector, string className = default)
        {
            Protect(noParamOrder, nameof(className));
            return CreateInstance(path, name: className);
        }

        /// <inheritdoc />
        public object CreateInstance(string virtualPath,
            string noParamOrder = Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) => Log.Func(() =>
        {
            // usually we don't have a relative path, so we use the preset path from when this class was instantiated
            var createPath = (_parent as IGetCodePath)?.CreateInstancePath;
            relativePath = relativePath ?? createPath;
            var instance = _DynCodeRoot?.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);
            return instance;
        });

        #endregion


    }
}
