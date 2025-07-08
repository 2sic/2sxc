using ToSic.Razor.Markup;

namespace ToSic.Sxc.Edit.Toolbar.Sys;

internal abstract class ItemToolbarBase(string logName) : ServiceBase($"{SxcLogName}.{logName}")
{
    public abstract string ToolbarAsTag { get; }

    /// <summary>
    /// Generate the attributes to add to a toolbar tag 
    /// </summary>
    /// <returns></returns>
    public abstract string ToolbarAsAttributes();

    public RawHtmlString Render(bool inTag) => new(inTag ? ToolbarAsAttributes() : ToolbarAsTag);
}