using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Razor;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;

namespace ToSic.Sxc.Web;

/// <summary>
/// The base page type for razor pages
/// It's the foundation for RazorPage and the old SexyContent page
/// It only contains internal wiring stuff, so not to be published
/// </summary>
[PrivateApi("internal class only!")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class RazorComponentBase : WebPageBase, IRazor, IHasCodeLog, IHasLog, IDnnRazorCompatibility, ICompatibilityLevel
{
    #region Constructor / Setup

    /// <summary>
    /// Special helper to move all Razor logic into a separate class.
    /// For architecture of Composition over Inheritance.
    /// </summary>
    [PrivateApi]
    internal DnnRazorHelper RzrHlp => _rzrHlp ??= new DnnRazorHelper().Init(this);
    private DnnRazorHelper _rzrHlp;

    /// <summary>
    /// Internal access to the underlying RenderPage.
    /// This is needed by the render helper to provide the default behavior
    /// if we are not using Roslyn
    /// </summary>
    internal HelperResult BaseRenderPage(string path, object data = null)
    {
        var l = (this as IHasLog).Log.Fn<HelperResult>($"{nameof(path)}: '{path}', {nameof(data)}: {data != null}");
        // TODO: VERIFY / handle conversion from 'object' to 'params object[]'
        return data == null
            ? l.Return(base.RenderPage(path), "default render, no data")
            : l.Return(base.RenderPage(path, data), "default render with data");
    }

    /// <inheritdoc />
    [PrivateApi]
    public ICodeApiService _CodeApiSvc { get; private set; }

    /// <inheritdoc />
    [PrivateApi]
    public void ConnectToRoot(ICodeApiService codeRoot)
    {
        RzrHlp.ConnectToRoot(codeRoot);
        _CodeApiSvc = codeRoot;
    }

    /// <summary>
    /// Override the base class ConfigurePage, and additionally update internal objects so sub-pages work just like the master
    /// </summary>
    /// <param name="parentPage"></param>
    [PrivateApi]
    protected override void ConfigurePage(WebPageBase parentPage)
    {
        base.ConfigurePage(parentPage);
        RzrHlp.ConfigurePage(parentPage, VirtualPath);
    }

    /// <summary>
    /// Must be set on each derived class
    /// </summary>
    [PrivateApi]
    public abstract int CompatibilityLevel { get; }

    #endregion

    #region Secret Stuff like IHasLog or Compile Helpers

    /// <summary>
    /// EXPLICIT Log implementation (to ensure that new IHasLog.Log interface is implemented)
    /// </summary>
    [PrivateApi] ILog IHasLog.Log => RzrHlp.Log;

    /// <inheritdoc />
    public string Path => VirtualPath;


    #endregion

    #region Core Properties which should appear in docs

    /// <inheritdoc />
    public virtual ICodeLog Log => RzrHlp.CodeLog;

    /// <inheritdoc />
    public virtual IHtmlHelper Html => RzrHlp.Html;

    #endregion
}