﻿using ToSic.Sxc.Code.Sys.HotBuild;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext
{
    #region SharedCode Compiler

    /// <inheritdoc />
    public object? CreateInstance(string virtualPath,
        NoParamOrder noParamOrder = default,
        string? name = null,
        string? relativePath = null,
        bool throwOnError = true)
    {
        var l = Log.Fn<object>($"{virtualPath}, {name}, {relativePath}, {throwOnError}");

        // Compile
        var compiler = Services.CodeCompilerLazy.Value;
        var spec = new HotBuildSpec(App.AppId, _edition, App.Name);
        var instance = compiler.InstantiateClass(virtualPath, spec, className: name, relativePath: relativePath, throwOnError: throwOnError);

        if (instance == null)
            return l.ReturnNull("null / not found / error");

        var typeName = instance.GetType().FullName;

        // if it supports all our known context properties, attach them
        if (instance is INeedsExecutionContext needsRoot)
        {
            l.A($"will attach root / Kit to {typeName}");
            needsRoot.ConnectToRoot(this);
        }
        else
            l.A($"no root / Kit for {typeName}");

        return l.Return(instance, instance.ToString());
    }

    /// <inheritdoc />
    public string CreateInstancePath { get; set; } = null!; // must be set as the context becomes clear

    #endregion
}