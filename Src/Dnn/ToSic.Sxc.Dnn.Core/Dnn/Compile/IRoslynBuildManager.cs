using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;

namespace ToSic.Sxc.Dnn.Compile;

public interface IRoslynBuildManager
{
    AssemblyResult GetCompiledAssembly(CodeFileInfo codeFileInfo, string className, HotBuildSpec spec);

    /// <summary>
    /// Manage template compilations, cache the assembly and returns the generated type.
    /// </summary>
    /// <param name="codeFileInfo"></param>
    /// <param name="spec"></param>
    /// <returns>The generated type for razor cshtml.</returns>
    Type GetCompiledType(CodeFileInfo codeFileInfo, HotBuildSpec spec);
}