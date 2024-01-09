// ReSharper disable InconsistentNaming
namespace ToSic.Sxc.Web.JsContext;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsApi
{
    public string platform { get; set; }
    public int page { get; set; }
    public string root { get; set; }
    public string api { get; set; }
    public string appApi { get; set; }
    public string uiRoot { get; set; }
    public string rvtHeader { get; set; }
    public string rvt { get; set; }
    public string dialogQuery { get; set; }

    /// <summary>
    /// Debug information while we're developing the on-module info
    /// </summary>
    public string source => "module JsApi";
}