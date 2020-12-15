namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        #region SharedCode Compiler

        /// <inheritdoc />
        public virtual dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var wrap = Log.Call<dynamic>($"{virtualPath}, {name}, {relativePath}, {throwOnError}");
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "CreateInstance",
                $"{nameof(name)},{nameof(throwOnError)}");

            // Compile
            var instance = new CodeCompiler(_serviceProvider, Log)
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
