using System.Collections.Immutable;

namespace ToSic.Sxc.Edit.Toolbar;

internal interface ITweakButtonInternal
{
    /// <summary>
    /// List of changes to apply to the UI parameter
    /// </summary>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)] 
    IImmutableList<object> UiMerge { get; }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    IImmutableList<object> ParamsMerge { get; }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    bool? _condition { get; }

    /// <summary>
    /// Named tweaks for situations where the tweak may be needed for multiple buttons,
    /// each with a different tweak.
    /// </summary>
    public IImmutableDictionary<string, Func<ITweakButton, ITweakButton>> Named { get; }
}