using ToSic.Razor.Markup;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Edit.Toolbar;

/// <summary>
/// The toolbar builder helps you create Toolbar configurations for the UI.
/// Note that it has a fluid API, and each method/use returns a fresh object with the updated configuration.
/// </summary>
/// <remarks>
/// Your code cannot construct this object by itself, as it usually needs additional information.
/// To get a `ToolbarBuilder`, use the <see cref="Services.IToolbarService"/>.
///
/// * uses the [](xref:NetCode.Conventions.Functional)
/// 
/// History
/// * Added in 2sxc 13, just minimal API
/// * massively enhanced in v14.04
/// * most commands extended with [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) in v15.07
/// </remarks>
[PublicApi]
public partial interface IToolbarBuilder: IRawHtmlString, IHasLog, INeedsCodeApiService
{
    //[PrivateApi("internal use only")]
    //IToolbarBuilder Toolbar(
    //    string toolbarTemplate,
    //    object target = default,
    //    NoParamOrder noParamOrder = default,
    //    Func<ITweakButton, ITweakButton> tweak = default,
    //    object ui = default,
    //    object parameters = default,
    //    object prefill = default
    //);

}