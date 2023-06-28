using System.Collections.Generic;
using ToSic.Eav.Code.Help;
using ToSic.Lib.Documentation;
using static ToSic.Sxc.Code.Help.Obsolete10;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor14: IHasCodeHelp
    {
        [PrivateApi]
        List<CodeHelp> IHasCodeHelp.ErrorHelpers => new List<CodeHelp>
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
