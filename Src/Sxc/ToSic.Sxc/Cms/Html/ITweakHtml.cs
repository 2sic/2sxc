using System;
using ToSic.Lib.Coding;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Cms.Html
{
    /// <summary>
    /// Tweak API to reconfigure HTML generation in the <see cref="ICmsService"/> class.
    /// </summary>
    /// <remarks>Added in v17</remarks>
    public interface ITweakHtml
    {
        /// <summary>
        /// Simple value tweak, to inject a different value for use instead of the original.
        /// </summary>
        /// <param name="replace">replacement value to use instead</param>
        /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="step">optional step, such as 'before' or 'after' - default is 'before'</param>
        /// <returns></returns>
        ITweakHtml Value(string replace, NoParamOrder protector = default, string step = default);

        /// <summary>
        /// Simple value tweak, to inject a different value for use instead of the original.
        /// </summary>
        /// <param name="func">function to generate a replacement value</param>
        /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="step">optional step, such as 'before' or 'after' - default is 'before'</param>
        /// <returns></returns>
        ITweakHtml Value(Func<string> func, NoParamOrder protector = default, string step = default);

        /// <summary>
        /// Simple value tweak, to inject a different value for use instead of the original.
        /// </summary>
        /// <param name="func">function to generate a replacement value, receiving the previous value inside a <see cref="ITweakValue{TValue}"/> </param>
        /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="step">optional step, such as 'before' or 'after' - default is 'before'</param>
        /// <returns></returns>
        ITweakHtml Value(Func<ITweakValue<string>, string> func, NoParamOrder protector = default, string step = default);
    }
}
