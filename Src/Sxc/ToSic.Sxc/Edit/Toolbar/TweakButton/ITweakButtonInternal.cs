using System.Collections.Immutable;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Edit.Toolbar;

internal interface ITweakButtonInternal
{
    /// <summary>
    /// List of changes to apply to the UI parameter
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    IImmutableList<object> UiMerge { get; }

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IImmutableList<object> ParamsMerge { get; }
}