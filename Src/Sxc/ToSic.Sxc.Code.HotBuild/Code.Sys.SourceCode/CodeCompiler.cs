﻿using ToSic.Eav.Sys;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Sys.HotBuild;
using static System.StringComparison;

namespace ToSic.Sxc.Code.Sys.SourceCode;

/// <summary>
/// Code / Class Compiler.
/// </summary>
/// <param name="serviceProvider"></param>
/// <remarks>
/// This is abstract, as each platform (Dnn/Oqtane) has its own implementation for specific parts of this.
///
/// We also have a minimal IClassCompiler interface, for use in the CodeApiService.
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class CodeCompiler(IServiceProvider serviceProvider, object[]? connect = default) : ServiceBase("Sys.CsCmpl", connect: connect /* never! serviceProvider */), IClassCompiler
{
    public object? InstantiateClass(string virtualPath, HotBuildSpec spec, string? className = null, string? relativePath = null, bool throwOnError = true)
    {
        var l = Log.Fn<object>($"{virtualPath}; {spec}; {nameof(className)}:{className}; {nameof(relativePath)}:{relativePath}; {nameof(throwOnError)}: {throwOnError}");

        // Perform various checks on the path values
        var hasErrorMessage = CheckIfPathsOkAndCleanUp(ref virtualPath, relativePath);
        if (hasErrorMessage != null)
        {
            l.A($"Error: {hasErrorMessage}");
            l.ReturnNull("failed");
            if (throwOnError)
                throw new(hasErrorMessage);
            return null; // special, l.Return was already called
        }

        var pathLowerCase = virtualPath.ToLowerInvariant();
        var isCs = pathLowerCase.EndsWith(SourceCodeConstants.CsFileExtension);
        var isCshtml = pathLowerCase.EndsWith(SourceCodeConstants.CsHtmlFileExtension);

        Type? compiledType = null;
        string? errorMessages;
        if (isCshtml && string.IsNullOrEmpty(className))
            (compiledType, errorMessages) = GetCsHtmlType(virtualPath);
        // compile .cs files
        else if (isCs || isCshtml)
            (compiledType, errorMessages) = GetTypeOrErrorMessages(virtualPath, className, throwOnError, spec);
        else
            errorMessages = $"Error: given path '{Path.GetFileName(virtualPath)}' doesn't point to a .cs or .cshtml";

        if (errorMessages != null)
        {
            l.A($"{errorMessages}; throw error: {throwOnError}");
            l.ReturnNull("failed");
            if (throwOnError) throw new(errorMessages);
            return null;
        }

        var instance = serviceProvider.Build<object>(compiledType!, Log);
        AttachRelativePath(virtualPath, instance);

        return l.Return(instance, $"found: {instance != null!}");
    }

    public (Type? Type, string? ErrorMessages) GetTypeOrErrorMessages(string relativePath, string? className, bool throwOnError, HotBuildSpec spec)
    {
        var l = Log.Fn<(Type? Type, string? ErrorMessages)>($"{nameof(relativePath)}: '{relativePath}'; {nameof(className)} '{className}'; {nameof(throwOnError)}: {throwOnError}; {spec}");

        // if no name provided, use the name which is the same as the file name
        className ??= Path.GetFileNameWithoutExtension(relativePath) ?? EavConstants.NullNameId;

        var assResult = GetAssembly(relativePath, className, spec);
        var assembly = assResult.Assembly;
        var errorMessages = assResult.ErrorMessages;

        if (errorMessages != null)
            return l.Return((null, errorMessages), "error messages");

        if (assembly == null)
            return l.Return((null, "assembly is null"), "no assembly");

        var possibleErrorMessage = $"Error: Didn't find type '{className}' in {Path.GetFileName(relativePath)}. Maybe the class name doesn't match the file name. ";
        Type? compiledType = null;
        try
        {
            compiledType = assembly.GetType(className, false, true);
            if (compiledType == null)
            {
                var types = assembly.GetTypes();
                compiledType = types.FirstOrDefault(t => t.Name.EqualsInsensitive(className));
            }

            if (compiledType == null && throwOnError)
                assembly.GetType(className, true, true);
        }
        catch (Exception ex)
        {
            l.A(possibleErrorMessage);
            if (throwOnError) throw new TypeLoadException(possibleErrorMessage, ex);
        }

        if (compiledType == null)
            errorMessages = possibleErrorMessage;

        return l.Return((compiledType, errorMessages), errorMessages == null ? "ok" : "errors");
    }

    public abstract AssemblyResult GetAssembly(string relativePath, string className, HotBuildSpec spec);


    protected abstract (Type Type, string? ErrorMessage) GetCsHtmlType(string relativePath);


    /// <summary>
    /// Check the path and perform various corrections
    /// </summary>
    /// <param name="virtualPath">primary path to use</param>
    /// <param name="relativePath">optional second path to which the primary one would be attached to</param>
    /// <returns>null if all is ok, or an error message if not</returns>
    private string? CheckIfPathsOkAndCleanUp(ref string virtualPath, string? relativePath)
    {
        var l = Log.Fn<string>($"{nameof(virtualPath)}: '{virtualPath}', {nameof(relativePath)}: '{relativePath}'");
        if (string.IsNullOrWhiteSpace(virtualPath))
            return l.ReturnAndLog("no path/name provided");

        // if path relative, merge with shared code path
        virtualPath = virtualPath.ForwardSlash();
        if (!virtualPath.StartsWith("/"))
        {
            l.A($"Trying to resolve relative path: '{virtualPath}' using '{relativePath}'");
            if (relativePath == null)
                return l.ReturnAndLog("Unexpected null value on relativePath");

            // if necessary, add trailing slash
            relativePath = relativePath.SuffixSlash();
            virtualPath = Path.Combine(relativePath, virtualPath).ToAbsolutePathForwardSlash();
            l.A($"final virtual path: '{virtualPath}'");
        }

        if (virtualPath.IndexOf(":", InvariantCultureIgnoreCase) > -1)
            return l.ReturnAndLog($"Tried to get .cs file, but found '{virtualPath}' containing ':', (not allowed)");

        return l.ReturnNull("all ok");
    }


    private bool AttachRelativePath(string virtualPath, object instance)
    {
        var l = Log.Fn<bool>($"{nameof(virtualPath)}: {virtualPath}");

        if (instance is not IGetCodePath codeForwarding)
            return l.ReturnFalse("didn't attach");

        // in case it supports shared code again, give it the relative path
        codeForwarding.CreateInstancePath = Path.GetDirectoryName(virtualPath)!;
        return l.ReturnTrue("attached");
    }
}