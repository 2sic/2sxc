﻿using System;
using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Oqt.Server.Code.Internal;

[PrivateApi]
internal class CodeCompilerNetCore(
    IServiceProvider serviceProvider,
    LazySvc<IServerPaths> serverPaths,
    Generator<Compiler> compiler)
    : CodeCompiler(serviceProvider, connect: [serverPaths, compiler])
{
    protected override (Type Type, string ErrorMessage) GetCsHtmlType(string virtualPath)
        => throw new("Runtime Compile of .cshtml is Not Implemented in .net standard / core");

    public override AssemblyResult GetAssembly(string virtualPath, string className, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>(
            $"{nameof(virtualPath)}: '{virtualPath}'; {nameof(className)}: '{className}'; {spec}", timer: true);
        var fullContentPath = serverPaths.Value.FullContentPath(virtualPath.Backslash());
        var fullPath = NormalizeFullFilePath(fullContentPath);
        l.A($"New paths: '{fullContentPath}', '{fullPath}'");
        try
        {
            return l.ReturnAsOk(compiler.New().Compile(fullPath, className, spec));
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage =
                $"Error: Can't compile '{className}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. " +
                ex.Message;
            return l.ReturnAsError(new AssemblyResult(errorMessages: errorMessage), "error");
        }
    }

    /// <summary>
    /// Normalize full file path, so it is without redirections like "../" in "dir1/dir2/../file.cs"
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    private static string NormalizeFullFilePath(string fullPath) => new FileInfo(fullPath).FullName;
}