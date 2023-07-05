using System.Collections.Generic;
using ToSic.Eav.Code.Help;
using static ToSic.Sxc.Code.Help.ObsoleteHelp;

namespace ToSic.Sxc.Code.Help
{
    public class CodeHelpDbV16
    {
        private static readonly CodeHelp ListNotExist16 =
            HelpNotExistsPro("List", "MyItems", "AsItems(MyData.Get())"); // TODO: NAMING NOT FINAL

        private static readonly CodeHelp ListObsolete16 =
            new CodeHelp(ListNotExist16, detect: "does not contain a definition for 'List'");

        private static readonly CodeHelp ListObsolete16MisCompiledAsGenericList = new CodeHelp(ListNotExist16,
            detect:
            @"error CS0305: Using the generic type 'System.Collections.Generic.List<T>' requires 1 type arguments");

        private static readonly CodeHelp ListContentNotExist16 = HelpNotExistsPro("ListContent", "MyHeader");

        private static readonly CodeHelp ListPresentationNotExist16 = HelpNotExistsPro("ListPresentation", "MyHeader.Presentation");

        private static readonly CodeHelp ContentNotExist16 = HelpNotExistsPro("Content", "MyItem");

        private static readonly CodeHelp ContentNotExist16Duplicate = HelpNotExistsPro("Content", "You may prefer to use Razor14");

        private static readonly CodeHelp HeaderNotExist16 = HelpNotExistsPro("Header", "MyHeader");

        private static readonly CodeHelp CreateSourceNotExist16 = HelpNotExistsPro("CreateSource", "Kit.Data.CreateSource(...)");
        private static readonly CodeHelp CreateInstanceShouldBeGetCode = HelpNotExistsPro("CreateInstance", "GetCode(...)");

        private static readonly CodeHelp ResourcesNotExist16 = HelpNotExistsPro("Resources", "App.Resources", "AllResources");

        private static readonly CodeHelp ResourcesNotExist16B = new CodeHelp(ResourcesNotExist16,
            detect: @"does not exist in the namespace 'Resources' (are you missing an assembly reference?)");

        private static readonly CodeHelp SettingsNotExist16 = HelpNotExistsPro("Settings", "App.Settings", "AllSettings");

        private static readonly CodeHelp PresentationNotExist16 = HelpNotExistsPro("Presentation", "MyItem.Presentation");

        private static readonly CodeHelp DataNotExist = HelpNotExistsPro("Data", "MyData");

        private static readonly CodeHelp EditNotExist = HelpNotExistsPro("Edit",
            ("Kit.Toolbar.Default()...", "to build a standard toolbar"),
            ("Kit.Toolbar.Empty()...", "to start with an empty toolbar"),
            ("MyUser.IsContentAdmin", "to find out if edit is enabled"),
            ("Kit.Edit", "to really use the Edit object (not often needed, as the replacements are better)"));

        private static readonly CodeHelp AsAdamNotExist = HelpNotExistsPro(("AsAdam", "AsAdam isn't needed any more, since there is an easier syntax."), ("object.Folder(\"FieldName\")", "Use the Folder(...) method on an Item"));

        internal static List<CodeHelp> Compile16 = new List<CodeHelp>
        {
            // use `Convert`
            CodeHelpDbV12.SystemConvertIncorrectUse,

            // Use Dnn
            CodeHelpDbV12.DnnObjectNotInHybrid,

            // use `CreateSource(name)
            CreateSourceNotExist16,

            CreateInstanceShouldBeGetCode,

            // Not handled - can't because the AsDynamic accepts IEntity which works in Razor14
            // dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
            // dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
            // IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
            // dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => Obsolete10.AsDynamicKvp();

            // Access .List
            ListNotExist16,

            ListObsolete16,
            ListObsolete16MisCompiledAsGenericList,

            // Access ListContent
            ListContentNotExist16,
            ListPresentationNotExist16,

            // Presentation
            PresentationNotExist16,

            // Content and Header - replaced with MyItem / MyHeader
            ContentNotExist16,
            //ContentNotExist16Duplicate,
            HeaderNotExist16,

            // Skipped, as can't be detected - they are all IEnumerable...
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataStream stream) => Obsolete10.AsDynamicForList();
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataSource source) => Obsolete10.AsDynamicForList();
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => Obsolete10.AsDynamicForList();

            // Settings / Resources
            SettingsNotExist16,
            ResourcesNotExist16,
            ResourcesNotExist16B,

            EditNotExist,
            AsAdamNotExist,
            DataNotExist,
        };

    }
}
