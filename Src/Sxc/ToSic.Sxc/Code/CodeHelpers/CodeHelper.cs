using ToSic.Lib.Logging;

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

        /// <inheritdoc />
        public object CreateInstance(string virtualPath,
            string noParamOrder = Eav.Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) => Log.Func(() =>
        {
            // usually we don't have a relative path, so we use the preset path from when this class was instantiated
            var createPath = (_parent as ICreateInstance)?.CreateInstancePath;
            relativePath = relativePath ?? createPath;
            var instance = _DynCodeRoot?.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);
            return instance;
        });

        #endregion


    }
}
