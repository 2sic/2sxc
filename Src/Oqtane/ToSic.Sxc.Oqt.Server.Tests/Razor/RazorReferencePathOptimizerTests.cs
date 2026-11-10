using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using ToSic.Sxc.Razor;

namespace ToSic.Sxc.Oqt.Razor;

public class RazorReferencePathOptimizerTests
{
    [Fact]
    public void PreferCompileReferences_KeepsHighestVersion_WhenDuplicateAssemblyHasDifferentVersions()
    {
        // Arrange
        const string rootPath = @"C:\Oqtane.Server\Microsoft.AspNetCore.Authorization.dll";
        const string refsPath = @"C:\Oqtane.Server\refs\Microsoft.AspNetCore.Authorization.dll";
        var getAssemblyName = AssemblyNames(
            (rootPath, Assembly("Microsoft.AspNetCore.Authorization", "9.0.0.0")),
            (refsPath, Assembly("Microsoft.AspNetCore.Authorization", "10.0.0.0")));

        // Act
        var result = RazorReferencePathOptimizer.PreferCompileReferences([rootPath, refsPath], getAssemblyName);

        // Assert
        Equal([refsPath], result);
    }

    [Fact]
    public void PreferCompileReferences_PrefersRefsPath_WhenDuplicateAssemblyHasSameVersion()
    {
        // Arrange
        const string rootPath = @"C:\Oqtane.Server\Microsoft.Data.SqlClient.dll";
        const string refsPath = @"C:\Oqtane.Server\refs\Microsoft.Data.SqlClient.dll";
        var getAssemblyName = AssemblyNames(
            (rootPath, Assembly("Microsoft.Data.SqlClient", "6.0.0.0")),
            (refsPath, Assembly("Microsoft.Data.SqlClient", "6.0.0.0")));

        // Act
        var result = RazorReferencePathOptimizer.PreferCompileReferences([rootPath, refsPath], getAssemblyName);

        // Assert
        Equal([refsPath], result);
    }

    [Fact]
    public void PreferCompileReferences_KeepsUnknownPaths_WhenAssemblyNameCannotBeRead()
    {
        // Arrange
        const string firstPath = @"C:\Oqtane.Server\custom-a.dll";
        const string secondPath = @"C:\Oqtane.Server\custom-b.dll";

        // Act
        var result = RazorReferencePathOptimizer.PreferCompileReferences([firstPath, secondPath], _ => null);

        // Assert
        Equal([firstPath, secondPath], result);
    }

    [Fact]
    public void PreferCompileReferences_SkipsNativeDlls_WhenMixedWithManagedAssemblies()
    {
        var managedPath = typeof(RazorReferencePathOptimizerTests).Assembly.Location;
        var nativePath = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(nativePath, NativePeImage(managedPath));

            var result = RazorReferencePathOptimizer.PreferCompileReferences([nativePath, managedPath]);

            Equal([managedPath], result);
        }
        finally
        {
            File.Delete(nativePath);
        }
    }

    [Fact]
    public void PreferCompileReferences_KeepsInvalidPeFiles_ForCompilerDiagnostics()
    {
        var invalidPath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(invalidPath, "not a PE image");

            var result = RazorReferencePathOptimizer.PreferCompileReferences([invalidPath]);

            Equal([invalidPath], result);
        }
        finally
        {
            File.Delete(invalidPath);
        }
    }

    [Fact]
    public void PreferCompileReferences_SkipsMissingPaths()
    {
        // Arrange
        const string rootPath = @"C:\Oqtane.Server\Microsoft.AspNetCore.Authorization.dll";
        const string refsPath = @"C:\Oqtane.Server\refs\Microsoft.AspNetCore.Authorization.dll";
        var getAssemblyName = AssemblyNames(
            (rootPath, Assembly("Microsoft.AspNetCore.Authorization", "9.0.0.0")),
            (refsPath, Assembly("Microsoft.AspNetCore.Authorization", "10.0.0.0")));

        // Act
        var result = RazorReferencePathOptimizer.PreferCompileReferences(
            [rootPath, refsPath],
            path => !path.Equals(rootPath, StringComparison.OrdinalIgnoreCase),
            getAssemblyName);

        // Assert
        Equal([refsPath], result);
    }

    [Fact]
    public void PreferCompileReferences_SkipsSystemPrivateAssemblies_WhenSystemRuntimeRefIsAvailable()
    {
        // Arrange
        const string privateCoreLibPath = @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\10.0.0\System.Private.CoreLib.dll";
        const string privateXmlPath = @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\10.0.0\System.Private.Xml.dll";
        const string systemRuntimeRefPath = @"C:\Oqtane.Server\refs\System.Runtime.dll";
        const string xmlReaderWriterRefPath = @"C:\Oqtane.Server\refs\System.Xml.ReaderWriter.dll";
        var getAssemblyName = AssemblyNames(
            (privateCoreLibPath, Assembly("System.Private.CoreLib", "10.0.0.0")),
            (privateXmlPath, Assembly("System.Private.Xml", "10.0.0.0")),
            (systemRuntimeRefPath, Assembly("System.Runtime", "10.0.0.0")),
            (xmlReaderWriterRefPath, Assembly("System.Xml.ReaderWriter", "10.0.0.0")));

        // Act
        var result = RazorReferencePathOptimizer.PreferCompileReferences(
            [privateCoreLibPath, privateXmlPath, systemRuntimeRefPath, xmlReaderWriterRefPath],
            _ => true,
            getAssemblyName);

        // Assert
        Equal([systemRuntimeRefPath, xmlReaderWriterRefPath], result);
    }

    private static byte[] NativePeImage(string managedPath)
    {
        var image = File.ReadAllBytes(managedPath);
        using var reader = new PEReader(new MemoryStream(image));

        // The CLR directory is the penultimate 8-byte data directory in a managed PE header.
        // Removing it gives us a native PE fixture without a platform-specific binary dependency.
        const int dataDirectorySize = 8;
        var clrDirectoryOffset = reader.PEHeaders.PEHeaderStartOffset
                                 + reader.PEHeaders.CoffHeader.SizeOfOptionalHeader - 2 * dataDirectorySize;
        Array.Clear(image, clrDirectoryOffset, dataDirectorySize);
        return image;
    }

    private static Func<string, AssemblyName?> AssemblyNames(params (string Path, AssemblyName AssemblyName)[] assemblies)
    {
        var map = assemblies.ToDictionary(assembly => assembly.Path, assembly => assembly.AssemblyName, StringComparer.OrdinalIgnoreCase);
        return path => map.TryGetValue(path, out var assemblyName) ? assemblyName : null;
    }

    private static AssemblyName Assembly(string name, string version)
        => new(name) { Version = Version.Parse(version) };
}
