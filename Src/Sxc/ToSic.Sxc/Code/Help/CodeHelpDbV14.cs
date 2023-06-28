using System.Collections.Generic;
using ToSic.Eav.Code.Help;
using static ToSic.Sxc.Code.Help.CodeHelpDbV12;

namespace ToSic.Sxc.Code.Help
{
    internal class CodeHelpDbV14
    {
        internal static List<CodeHelp> Compile14 = new List<CodeHelp>
        {
            // use `Convert`
            SystemConvertIncorrectUse,

            // Use Dnn
            DnnObjectNotInHybrid,

            // use `CreateSource(name)
            CreateSourceStringObsolete,

            // Not handled - can't because the AsDynamic accepts IEntity which works in Razor14
            // dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
            // dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
            // IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
            // dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => Obsolete10.AsDynamicKvp();

            // Access .List
            ListNotExist12,

            ListObsolete12,
            ListObsolete12MisCompiledAsGenericList,

            // Access ListContent
            ListContentNotExist12,
            ListPresentationNotExist12,

            // Presentation
            PresentationNotExist12,

            // Skipped, as can't be detected - they are all IEnumerable...
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataStream stream) => Obsolete10.AsDynamicForList();
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataSource source) => Obsolete10.AsDynamicForList();
            //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => Obsolete10.AsDynamicForList();

        };
    }
}
