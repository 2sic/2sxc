using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Sys.CodeApi;

namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// The CodeHelper for custom code.
/// Provides the GetCode and CreateInstance methods.
///
/// It inherits from the helper base so in Custom Code it will also provide the log,
/// but when used in APIs it only provides the GetCode and CreateInstance methods.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeHelper() : CodeHelperBase("Sxc.CdHlp")
{
    #region Setup

    public CodeHelper Init(IGetCodePath parent)
    {
        _parent = parent;
        return this;
    }

    private IGetCodePath _parent = null!;

    #endregion

    #region GetCode / CreateInstance

    public object? GetCode(string path, NoParamOrder noParamOrder = default, string? className = default) 
        => CreateInstance(path, name: className);

    /// <inheritdoc />
    public object? CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string? name = null, string? relativePath = null, bool throwOnError = true)
    {
        var l = Log.Fn<object?>();

        // Prevent GetCode / CreateInstance from being used inside AppCode
        CodeRunThrowIfParentIsInsideAppCode(_parent);

        // usually we don't have a relative path, so we use the preset path from when this class was instantiated
        relativePath ??= _parent?.CreateInstancePath;
        object? instance = ExCtxOrNull?.GetDynamicApi()?.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);
        return l.Return(instance);
    }

    public static void CodeRunThrowIfParentIsInsideAppCode(object parent)
    {
        if (HotBuildConstants.ObjectIsFromAppCode(parent))
            throw new("Can't use GetCode in objects from inside AppCode as it's a very dynamic operation.");
    }
    
    #endregion
}