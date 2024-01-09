using ToSic.Lib.Coding;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeHelper: CodeHelperBase
{
    #region Setup

    public CodeHelper() : base("Sxc.CdHlp")
    {

    }

    public CodeHelper Init(CustomCodeBase parent)
    {
        _parent = parent;
        return this;
    }

    private CustomCodeBase _parent;

    #endregion

    #region CreateInstance

    public object GetCode(string path, NoParamOrder noParamOrder = default, string className = default) 
        => CreateInstance(path, name: className);

    /// <inheritdoc />
    public object CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true)
    {
        var l = Log.Fn<object>();
        // usually we don't have a relative path, so we use the preset path from when this class was instantiated
        var createPath = (_parent as IGetCodePath)?.CreateInstancePath;
        relativePath ??= createPath;
        object instance = _DynCodeRoot?.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);
        return l.Return(instance);
    }

    #endregion


}