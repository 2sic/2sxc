using System.Collections;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Web.Sys.Html;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

/// <summary>
/// INTERNAL: Toolbar Builder implementation.
/// </summary>
/// <remarks>
/// We cannot make this internal :(
/// Reason is that as soon as we run something like `Kit.Toolbar.Empty(Content)` in a razor file,
/// the result is dynamic - so the compiler evaluates the final object at runtime.
/// If the ToolbarBuilder is internal, things start to fail.
/// like AsTag() will fail, saying that RawHtmlString doesn't have that
/// So for now :( it must remain public.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial record ToolbarBuilder: HybridHtmlString, IEnumerable<string>, IToolbarBuilder, INeedsExecutionContext
{

    #region Constructors and Init

    public class Dependencies(
        LazySvc<ToolbarButtonDecoratorHelper> toolbarButtonHelper,
        LazySvc<IAppsCatalog> appsCatalog)
        : DependenciesBase(connect: [toolbarButtonHelper, appsCatalog])
    {
        internal LazySvc<ToolbarButtonDecoratorHelper> ToolbarButtonHelper { get; } = toolbarButtonHelper;
        public LazySvc<IAppsCatalog> AppsCatalog { get; } = appsCatalog;
    }

    /// <summary>
    /// Public constructor for DI
    /// </summary>
    /// <param name="services"></param>
    public ToolbarBuilder(Dependencies services) =>
        Services = services.ConnectServices(Log);

    protected Dependencies Services { get; init; }

    public ILog Log { get; } = new Log(SxcLogName + ".TlbBld");
    

    public void ConnectToRoot(IExecutionContext? exCtx)
    {
        if (exCtx == null)
            return;
        ExCtx = exCtx;
        CurrentAppIdentity = exCtx.GetApp().PureIdentity();
        Services.ToolbarButtonHelper.Value.MainAppIdentity = CurrentAppIdentity;
    }

    private IAppIdentity? CurrentAppIdentity { get; set; }

    private IExecutionContext ExCtx { get; set; } = null!;

    #endregion

    #region Object state, init only for cloning

    internal ToolbarBuilderConfiguration Configuration { get; init; } = new();

    [field: AllowNull, MaybeNull]
    private ToolbarBuilderUtilities Utils => field ??= new();

    internal List<ToolbarRuleBase> Rules { get; init; } = [];

    #endregion


    internal IToolbarBuilder Toolbar(
        string toolbarTemplate,
        object? target = default,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = default,
        object? parameters = default,
        object? prefill = default
    )
    {
        var updated = this.AddInternal([new ToolbarRuleToolbar(toolbarTemplate, ui: PrepareUi(ui))]);
        // If anything is not null, then we must specify it
        return new[] { target, parameters, prefill, tweak }.Any(x => x != null)
            ? updated.Parameters(target, tweak: tweak, parameters: parameters, prefill: prefill)
            : updated;
    }


    private T? FindRule<T>() where T : class => Rules.FirstOrDefault(r => r is T) as T;


    #region Enumerators

    [PrivateApi]
    public IEnumerator<string> GetEnumerator()
    {
        var rulesToDeliver = Rules;

        // **Special**
        // Previously standalone toolbars also hovered based on their wrapper DIV.
        // But this isn't actually useful anymore - normally hover is done with a non-standalone toolbar.
        // But we cannot change the JS defaults, because that would affect old toolbars
        // So any standalone toolbar created using the tag-builder will automatically add a settings
        // to not-hover by default. 
        // The rule must be added to the top of the list, so that any other settings will take precedence,
        // Including UI rules added to the toolbar itself
        if (Configuration.HtmlMode == ToolbarHtmlModes.Standalone)
        {
            var standaloneSettings = new ToolbarRuleSettings(show: "always", hover: "none");
            rulesToDeliver = [standaloneSettings, .. Rules];
        }

        return rulesToDeliver.Select(r => r.ToString()).GetEnumerator();
    }

    [PrivateApi]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

}