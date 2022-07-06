using ToSic.Eav.Logging;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal abstract class ItemToolbarBase: HasLog
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

        //protected abstract IHybridHtmlString AttributeSettings();
        protected ItemToolbarBase(string logName) : base($"{Constants.SxcLogName}.{logName}")
        {
        }
    }
}
