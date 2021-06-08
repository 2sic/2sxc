namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        #region SharedCode Compiler

        /// <inheritdoc />
        public virtual dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var wrap = Log.Call<dynamic>($"{virtualPath}, {name}, {relativePath}, {throwOnError}");
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "CreateInstance",
                $"{nameof(name)},{nameof(throwOnError)}");

            // Compile
            var compiler = Deps.CodeCompilerLazy.IsValueCreated
                ? Deps.CodeCompilerLazy.Value
                : Deps.CodeCompilerLazy.Value.Init(Log);
            var instance = compiler
                //new CodeCompiler(_serviceProvider)
                //.Init(Log)
                .InstantiateClass(virtualPath, name, relativePath, throwOnError);

            // if it supports all our known context properties, attach them
            if (instance is ICoupledDynamicCode isShared) isShared.DynamicCodeCoupling(this);

            return wrap((instance != null).ToString(), instance);
        }

        /// <inheritdoc />
        public string CreateInstancePath { get; set; }

        #endregion
    }
}
