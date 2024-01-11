using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Dnn.Compile;

public interface IRoslynBuildManager
{
    AssemblyResult GetCompiledAssembly(string relativePath, string className, HotBuildSpec spec);

    /// <summary>
    /// Manage template compilations, cache the assembly and returns the generated type.
    /// </summary>
    /// <param name="templatePath">Relative path to template file.</param>
    /// <param name="spec"></param>
    /// <returns>The generated type for razor cshtml.</returns>
    Type GetCompiledType(string templatePath, HotBuildSpec spec);
}