using ToSic.Lib.Logging;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        #region SharedCode Compiler

        /// <inheritdoc />
        public virtual dynamic CreateInstance(string virtualPath,
            string noParamOrder = Eav.Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var wrap = Log.Fn<object>($"{virtualPath}, {name}, {relativePath}, {throwOnError}");
            Eav.Parameters.Protect(noParamOrder, $"{nameof(name)},{nameof(throwOnError)}");

            // Compile
            var compiler = Services.CodeCompilerLazy.Value;
            var instance = compiler.InstantiateClass(virtualPath, name, relativePath, throwOnError);

            // if it supports all our known context properties, attach them
            if (instance is INeedsDynamicCodeRoot needsRoot) needsRoot.ConnectToRoot(this);

            return wrap.Return(instance, (instance != null).ToString());
        }

        /// <inheritdoc />
        public string CreateInstancePath { get; set; }

        #endregion
    }
}
