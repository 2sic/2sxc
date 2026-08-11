using System.Reflection;
using ToSic.Sxc.Dnn.Razor.Sys;

namespace ToSic.Sxc.Dnn;

public class TemplateCacheServiceTests
{
    [Fact]
    public void ComputeAppCodeHash_NullAssembly_ReturnsStableNonEmptyHash()
    {
        var computeMethod = typeof(TemplateCacheService)
            .GetMethod("ComputeAppCodeHash", BindingFlags.NonPublic | BindingFlags.Static);

        NotNull(computeMethod);

        var nullHash1 = (string)computeMethod!.Invoke(null, [null])!;
        var nullHash2 = (string)computeMethod.Invoke(null, [null])!;
        var assemblyHash = (string)computeMethod.Invoke(null, [Assembly.GetExecutingAssembly()])!;

        False(string.IsNullOrWhiteSpace(nullHash1));
        Equal(nullHash1, nullHash2);
        NotEqual(nullHash1, assemblyHash);
    }
}
