using System.Collections.Immutable;

namespace ToSic.Sxc.Edit.Toolbar;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ITweakButtonInternal
{
    /// <summary>
    /// List of changes to apply to the UI parameter
    /// </summary>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)] 
    IImmutableList<object?> UiMerge { get; }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    IImmutableList<object?> ParamsMerge { get; }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    bool? _condition { get; }

    /// <summary>
    /// Named tweaks for situations where the tweak may be needed for multiple buttons,
    /// each with a different tweak.
    /// </summary>
    public IImmutableDictionary<string, Func<ITweakButton, ITweakButton>> Named { get; }

    /// <summary>
    /// Special internal add-rule, which is typically on the main toolbar,
    /// but will then only be applied to buttons with exactly this name...?
    ///
    /// Used in image responsive to add notes to the buttons,
    /// but different notes for e.g. Copyright etc.
    ///
    /// Not fully documented or standardized.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tweak"></param>
    /// <returns></returns>
    ITweakButton AddNamed(string name, Func<ITweakButton, ITweakButton> tweak);
}