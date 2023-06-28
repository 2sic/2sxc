using ToSic.Eav.Code.Help;
using static ToSic.Sxc.Code.Help.ObsoleteHelp;

namespace ToSic.Sxc.Code.Help
{
    public static class Obsolete16
    {
        internal static CodeHelp ListNotExist16 = CreateNotExistCodeHelp("List", false, "MyItems", "AsItems(MyData.Get())"); // TODO: NAMING NOT FINAL
        internal static CodeHelp ListObsolete16 = new CodeHelp(ListNotExist16, detect: "does not contain a definition for 'List'");

        internal static CodeHelp ListObsolete16MisCompiledAsGenericList = new CodeHelp(ListNotExist16,
            detect: @"error CS0305: Using the generic type 'System.Collections.Generic.List<T>' requires 1 type arguments");

        internal static CodeHelp ListContentNotExist16 = CreateNotExistCodeHelp("ListContent", false, "MyHeader");

        internal static CodeHelp ListPresentationNotExist16 = CreateNotExistCodeHelp("ListPresentation", false, "MyHeader.Presentation");

        internal static CodeHelp ContentNotExist16 = CreateNotExistCodeHelp("Content", false, "MyItem");
        internal static CodeHelp HeaderNotExist16 = CreateNotExistCodeHelp("Header", false, "MyHeader");
        internal static CodeHelp SettingsNotExist16 = CreateNotExistCodeHelp("Settings", false, "App.Settings", "AllSettings");
        internal static CodeHelp ResourcesNotExist16 = CreateNotExistCodeHelp("Resources", false, "App.Resources", "AllResources");

        internal static CodeHelp PresentationNotExist16 = CreateNotExistCodeHelp("Presentation", false, "MyItem.Presentation");
    }
}
