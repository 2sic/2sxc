using ToSic.Eav.CodeChanges;

namespace ToSic.Sxc.Code
{
    public class DynamicCode16Warnings
    {
        public static ICodeChangeInfo AvoidSettingsResources = CodeChangeInfo.Warn("no-settings-resources-on-code16",
            message: "Don't use Settings or Resources - use App.Settings/App.Resources or SettingsStack / ResourcesStack");

        public static ICodeChangeInfo NoDataMyContent = CodeChangeInfo.Warn("no-data-my-content-code16",
            message: "Don't use Data.MyContent - use new MyItem or MyItems");
        public static ICodeChangeInfo NoDataMyHeader = CodeChangeInfo.Warn("no-data-my-header-code16",
            message: "Don't use Data.MyHeader - use new MyHeader");
        public static ICodeChangeInfo NoDataMyData = CodeChangeInfo.Warn("no-data-my-data-code16",
            message: "Don't use Data.MyData - use new TODO!");
    }
}
