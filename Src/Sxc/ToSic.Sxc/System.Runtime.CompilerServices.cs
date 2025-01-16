// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{

    // enable C# 9 init-only properties
    // https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined
    internal static class IsExternalInit;


    #region required feature (for records / init)

    public class RequiredMemberAttribute : Attribute;

    public class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string name)
        {
        }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [System.AttributeUsage(System.AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class SetsRequiredMembersAttribute : Attribute;
}

#endregion