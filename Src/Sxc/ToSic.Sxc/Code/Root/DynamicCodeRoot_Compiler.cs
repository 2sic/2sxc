using ToSic.Lib.Coding;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Code;

public partial class DynamicCodeRoot
{
    #region SharedCode Compiler

    /// <inheritdoc />
    public dynamic CreateInstance(string virtualPath,
        NoParamOrder noParamOrder = default,
        string name = null,
        string relativePath = null,
        bool throwOnError = true)
    {
        var l = Log.Fn<object>($"{virtualPath}, {name}, {relativePath}, {throwOnError}");

        // Compile
        var compiler = Services.CodeCompilerLazy.Value;
        var instance = compiler.InstantiateClass(virtualPath, App.AppId, className: name, relativePath: relativePath, throwOnError: throwOnError);

        // if it supports all our known context properties, attach them
        if (instance is INeedsDynamicCodeRoot needsRoot) needsRoot.ConnectToRoot(this);

        return l.Return(instance, (instance != null).ToString());
    }

    /// <inheritdoc />
    public string CreateInstancePath { get; set; }

    #endregion
}