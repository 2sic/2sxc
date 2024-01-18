namespace ToSic.Sxc.Services.Tweaks;

/// <summary>
/// Tweak API to reconfigure a value pre-processing in a service / method call.
///
/// Whatever code you write, always assume that this interface can be replaced with another name
/// which will then provide more tweaks. So never use the interface-name in your code.
/// </summary>
/// <remarks>Added in v17</remarks>
[PublicApi]
public interface ITweakInput<TInput>
{
    /// <summary>
    /// Simple value tweak, to inject a different value for use instead of the original.
    /// </summary>
    /// <param name="replace">replacement value to use instead</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="step">optional step, such as 'before' or 'after' - default is 'before'</param>
    /// <returns></returns>
    ITweakInput<TInput> Input(TInput replace, NoParamOrder protector = default);

    /// <summary>
    /// Simple value tweak, to inject a different value for use instead of the original.
    /// </summary>
    /// <param name="func">function to generate a replacement value</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="step">optional step, such as 'before' or 'after' - default is 'before'</param>
    /// <returns></returns>
    ITweakInput<TInput> Input(Func<TInput> func, NoParamOrder protector = default);

    /// <summary>
    /// Simple value tweak, to inject a different value for use instead of the original.
    /// </summary>
    /// <param name="func">function to generate a replacement value, but first providing the initial value</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="step">optional step, such as 'before' or 'after' - default is 'before'</param>
    /// <returns></returns>
    ITweakInput<TInput> Input(Func<TInput, TInput> func, NoParamOrder protector = default);

    ///// <summary>
    ///// DO NOT USE YET - BETA
    ///// Simple value tweak, to inject a different value for use instead of the original.
    ///// </summary>
    ///// <param name="func">function to generate a replacement value, receiving the previous value inside a <see cref="ITweakValue{TValue}"/> </param>
    ///// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    ///// <param name="step">optional step, such as 'before' or 'after' - default is 'before'</param>
    ///// <returns></returns>
    //[PrivateApi("not yet final")]
    //ITweakHtml Process(Func<ITweakValue<string>, string> func, NoParamOrder protector = default);//, string step = default);
}