using ToSic.Lib.Services;

namespace ToSic.Sxc.Edit.Toolbar;

internal abstract class ItemToolbarBase(string logName) : ServiceBase($"{SxcLogName}.{logName}")
{
    public const string ToolbarAttributeName = "sxc-toolbar";
    public const string JsonToolbarNodeName = "toolbar";
    public const string JsonSettingsNodeName = "settings";

    protected const string ToolbarTagPlaceholder = "{contents}";
    protected const string ToolbarTagTemplate = "<ul class=\"sc-menu\" {contents} ></ul>";

    public abstract string ToolbarAsTag { get; }

    protected abstract string ToolbarJson { get; }

    /// <summary>
    /// Generate the attributes to add to a toolbar tag 
    /// </summary>
    /// <returns></returns>
    public abstract string ToolbarAsAttributes();
}