#if NETFRAMEWORK


// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Design-time compatibility shim for newer editor-generated Razor classes on .NET Framework.
/// </summary>
/// <remarks>
/// DNN runtime rendering still uses legacy <c>System.Web.Razor</c> code generation, which emits classic
/// <c>Execute</c>/<c>Write</c>/<c>WriteLiteral</c> calls through <c>WebPageExecutingBase</c>.
/// The members below only exist so VS Code / Roslyn design-time generated classes can bind against
/// <see cref="RazorTyped"/> during analysis of <c>.cshtml</c> files.
/// They must not become part of the production execution path.
/// If any of them is invoked at runtime, it means the runtime Razor compiler/code generator has drifted into
/// an unsupported shape or a non-generated subclass relied on this shim instead of providing real behavior.
/// In that case a fail-fast exception is safer than silently rendering wrong HTML.
/// </remarks>
public abstract partial class RazorTyped
{
    /// <summary>
    /// Newer design-time generated Razor classes may override <c>ExecuteAsync</c>.
    /// The real DNN runtime path should continue to use legacy <c>Execute</c>-based code generation.
    /// </summary>
    public virtual Task ExecuteAsync()
        => throw CreateUnexpectedRuntimeInvocation(nameof(ExecuteAsync));

    /// <summary>
    /// Bridges classic Razor's abstract <c>Execute</c> contract to the design-time <c>ExecuteAsync</c> pattern.
    /// If this base implementation is ever reached in production, fail fast with a clear message.
    /// </summary>
    public override void Execute() => ExecuteAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Placeholder symbol for newer design-time generated attribute writer calls.
    /// Legacy DNN runtime code generation should keep using <c>WriteAttribute</c> instead.
    /// </summary>
    protected virtual void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        => throw CreateUnexpectedRuntimeInvocation(nameof(BeginWriteAttribute));

    /// <summary>
    /// Placeholder symbol for newer design-time generated attribute writer calls.
    /// Legacy DNN runtime code generation should keep using <c>WriteAttribute</c> instead.
    /// </summary>
    protected virtual void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        => throw CreateUnexpectedRuntimeInvocation(nameof(WriteAttributeValue));

    /// <summary>
    /// Placeholder symbol for newer design-time generated attribute writer calls.
    /// Legacy DNN runtime code generation should keep using <c>WriteAttribute</c> instead.
    /// </summary>
    protected virtual void EndWriteAttribute()
        => throw CreateUnexpectedRuntimeInvocation(nameof(EndWriteAttribute));

    private static InvalidOperationException CreateUnexpectedRuntimeInvocation(string memberName)
        => new(
            $"The design-time compatibility member '{nameof(RazorTyped)}.{memberName}' was invoked at runtime. " +
            "This shim exists only to satisfy newer editor-generated Razor code for VS Code IntelliSense on .NET Framework. " +
            "The DNN runtime should continue using legacy System.Web.Razor code generation (Execute/Write/WriteLiteral and WriteAttribute). " +
            "If this exception occurs, the runtime Razor compiler/code generator has started emitting unsupported design-time members " +
            "or a non-generated subclass relied on the compat shim instead of implementing real runtime behavior.");
}
#endif
