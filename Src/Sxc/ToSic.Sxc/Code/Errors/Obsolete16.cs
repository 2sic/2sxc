using static ToSic.Sxc.Code.Errors.ObsoleteHelp;

namespace ToSic.Sxc.Code.Errors
{
    public static class Obsolete16
    {
        internal static CodeHelp ListNotExist16 = CreateNotExistCodeHelp("List", "MyItems", "AsItems(MyData.Get())"); // TODO: NAMING NOT FINAL
        internal static CodeHelp ListObsolete16 = new CodeHelp(ListNotExist16, detect: "does not contain a definition for 'List'");

        internal static CodeHelp ListObsolete16MisCompiledAsGenericList = new CodeHelp(ListNotExist16,
            detect: @"error CS0305: Using the generic type 'System.Collections.Generic.List<T>' requires 1 type arguments");

        internal static CodeHelp ListContentNotExist16 = CreateNotExistCodeHelp("ListContent", "MyHeader");

        internal static CodeHelp ListPresentationNotExist16 = CreateNotExistCodeHelp("ListPresentation", "MyHeader.Presentation");

        internal static CodeHelp ContentNotExist16 = CreateNotExistCodeHelp("Content", "MyItem");
        internal static CodeHelp HeaderNotExist16 = CreateNotExistCodeHelp("Header", "MyHeader");
        internal static CodeHelp SettingsNotExist16 = CreateNotExistCodeHelp("Settings", "App.Settings", "AllSettings");
        internal static CodeHelp ResourcesNotExist16 = CreateNotExistCodeHelp("Resources", "App.Resources", "AllResources");

        internal static CodeHelp PresentationNotExist16 = CreateNotExistCodeHelp("Presentation", "MyItem.Presentation");
    }
}
