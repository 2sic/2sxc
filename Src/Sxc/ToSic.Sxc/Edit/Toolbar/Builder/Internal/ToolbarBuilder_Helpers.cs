using System.Runtime.CompilerServices;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial class ToolbarBuilder
{
    private string PrepareUi(object ui, object uiMerge = default, string uiMergePrefix = default, IEnumerable<object> tweaks = default) 
        => Utils.PrepareUi(ui, uiMerge, uiMergePrefix, Configuration?.Group, tweaks: tweaks);

    private ITweakButton RunTweaksOrErrorIfCombined(
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        ITweakButton initial = default,
        object ui = default,
        object parameters = default, object prefill = default, object filter = default, [CallerMemberName] string methodName = null)
    {
        var tweaks = tweak?.Invoke(initial ?? new TweakButton());
        ErrorIfTweakCombined(tweaks, ui, parameters, prefill, filter, methodName);
        return tweaks;
    }

    private void ErrorIfTweakCombined(ITweakButton tweak, object ui, object parameters, object prefill, object filter, string methodName)
    {
        // No tweak, nothing to check
        if (tweak == null) return;
        // If tweak exist, skip if nothing else was provided
        if ((parameters ?? ui ?? prefill ?? filter) == null) return;

        // Figure out what name to show and alternative options
        (string Name, string Alternatives) err = ui != null
            ? (nameof(ui), $".{nameof(ITweakButton.Ui)}(...), {nameof(ITweakButton.Show)}() or other methods")
            : parameters != null
                ? (nameof(parameters), $".{nameof(ITweakButton.Parameters)}(...) or other methods")
                : prefill != null
                    ? (nameof(prefill), $".{nameof(ITweakButton.Prefill)}(...)")
                    : filter != null
                        ? (nameof(filter), $".{nameof(ITweakButton.Filter)}(...)")
                        : ("unknown", "unknown problem, pls contact us");

        throw new ArgumentException(
            $"You called .{methodName}(...) using the '{nameof(tweak)}:' parameter. " +
            $"You also supplied a '{err.Name}:'. This is not allowed. When you use 'tweak', use {err.Alternatives} instead. See https://go.2sxc.org/tweak-buttons");
    }
}