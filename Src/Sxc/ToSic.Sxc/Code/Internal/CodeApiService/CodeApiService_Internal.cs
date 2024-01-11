using System;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.Code.Internal;

public partial class CodeApiService : ICodeApiServiceInternal
{
    [PrivateApi]
    public void AttachApp(IApp app)
    {
        if (app is App typedApp) typedApp.SetupAsConverter(_Cdf);
        App = app;

        _edition = PolymorphConfigReader.UseViewEditionLazyGetEdition(_Block?.View, () => Services.Polymorphism.Init(App.AppState.List));
        //_edition = _Block?.View?.Edition.NullIfNoValue() // if Block-View comes with a preset edition, it's an ajax-preview which should be respected
        //          ?? Services.Polymorphism.Init(App.AppState.List).Edition(); // Figure out edition using data
    }

    private string _edition;

    [PrivateApi]
    [Obsolete("Warning - avoid using this on the DynamicCode Root - always use the one on the AsC")]
    public int CompatibilityLevel => _Cdf.CompatibilityLevel;

    [PrivateApi] public IBlock _Block { get; private set; }

}