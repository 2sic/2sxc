using System;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Code;

public partial class DynamicCodeRoot
{
    [PrivateApi]
    public void AttachApp(IApp app)
    {
        if (app is App typedApp) typedApp.SetupAsConverter(Cdf);
        App = app;

        FigureCurrentEdition();
    }

    [PrivateApi]
    [Obsolete("Warning - avoid using this on the DynamicCode Root - always use the one on the AsC")]
    public int CompatibilityLevel => Cdf.CompatibilityLevel;

    [PrivateApi] public IBlock Block { get; private set; }


    private void FigureCurrentEdition()
    {
        // Figure out the current edition - if none, stop here
        var polymorph = Services.Polymorphism.Init(App.AppState.List);
        // New 2023-03-20 - if the view comes with a preset edition, it's an ajax-preview which should be respected
        Edition = Block?.View?.Edition.NullIfNoValue() ?? polymorph.Edition();
    }
    private string Edition = default;
}