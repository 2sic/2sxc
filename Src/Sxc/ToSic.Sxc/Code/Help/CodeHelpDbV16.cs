using System.Collections.Generic;
using ToSic.Eav.Code.Help;
using static ToSic.Sxc.Code.Help.ObsoleteHelp;

namespace ToSic.Sxc.Code.Help
{
    public class CodeHelpDbV16
    {
        internal static CodeHelp ListNotExist16 =
            CreateNotExistCodeHelp("List", false, "MyItems", "AsItems(MyData.Get())"); // TODO: NAMING NOT FINAL

        internal static CodeHelp ListObsolete16 =
            new CodeHelp(ListNotExist16, detect: "does not contain a definition for 'List'");

        internal static CodeHelp ListObsolete16MisCompiledAsGenericList = new CodeHelp(ListNotExist16,
            detect:
            @"error CS0305: Using the generic type 'System.Collections.Generic.List<T>' requires 1 type arguments");

        internal static CodeHelp ListContentNotExist16 = CreateNotExistCodeHelp("ListContent", false, "MyHeader");

        internal static CodeHelp ListPresentationNotExist16 =
            CreateNotExistCodeHelp("ListPresentation", false, "MyHeader.Presentation");

        internal static CodeHelp ContentNotExist16 = CreateNotExistCodeHelp("Content", false, "MyItem");
        internal static CodeHelp HeaderNotExist16 = CreateNotExistCodeHelp("Header", false, "MyHeader");

        internal static CodeHelp SettingsNotExist16 =
            CreateNotExistCodeHelp("Settings", false, "App.Settings", "AllSettings");

        internal static CodeHelp ResourcesNotExist16 =
            CreateNotExistCodeHelp("Resources", false, "App.Resources", "AllResources");

        internal static CodeHelp PresentationNotExist16 =
            CreateNotExistCodeHelp("Presentation", false, "MyItem.Presentation");

        internal static List<CodeHelp> Compile16 = new List<CodeHelp>
        {
            // use `Convert`
            CodeHelpDbV12.SystemConvertIncorrectUse,

            // Use Dnn
            CodeHelpDbV12.DnnObjectNotInHybrid,

            // use `CreateSource(name)
            CodeHelpDbV12.CreateSourceStringObsolete,

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
            HeaderNotExist16,

            // Skipped, as can't be detected - they are all IEnumerable...
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataStream stream) => Obsolete10.AsDynamicForList();
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataSource source) => Obsolete10.AsDynamicForList();
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => Obsolete10.AsDynamicForList();

            // Settings / Resources
            SettingsNotExist16,
            ResourcesNotExist16,
        };

    }
}
