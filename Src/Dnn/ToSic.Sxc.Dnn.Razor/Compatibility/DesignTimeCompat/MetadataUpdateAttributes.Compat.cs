#if NETFRAMEWORK


// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Symbol-only compatibility attribute for newer Razor design-time / hot-reload metadata emitted by tooling.
    /// </summary>
    /// <remarks>
    /// This does not enable Hot Reload on DNN / .NET Framework.
    /// It only allows generated design-time code to resolve the expected symbol while editing <c>.cshtml</c> files.
    /// The type is public because the design-time helper project that compiles the generated Razor code is a different assembly.
    /// Keep this shim .NET Framework-only to avoid colliding with the real BCL type on modern runtimes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class CreateNewOnMetadataUpdateAttribute : Attribute
    {
    }
}


// ReSharper disable once CheckNamespace
namespace System.Reflection.Metadata
{
    /// <summary>
    /// Symbol-only compatibility attribute for metadata update handlers referenced by newer tooling.
    /// </summary>
    /// <remarks>
    /// This shim carries only the type shape required for design-time compilation.
    /// It does not provide active runtime Hot Reload behavior in DNN / .NET Framework.
    /// The type is public because generated design-time Razor code is compiled outside this assembly.
    /// Keep this shim .NET Framework-only to avoid colliding with the real BCL type on modern runtimes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class MetadataUpdateHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes the compatibility attribute with the handler type expected by generated design-time code.
        /// </summary>
        public MetadataUpdateHandlerAttribute(Type handlerType)
        {
            HandlerType = handlerType;
        }

        /// <summary>
        /// Gets the handler type referenced by generated design-time metadata update wiring.
        /// </summary>
        public Type HandlerType { get; }
    }
}


#endif
