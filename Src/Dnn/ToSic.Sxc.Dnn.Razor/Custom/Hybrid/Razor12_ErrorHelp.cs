using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code.Errors;
using static ToSic.Sxc.Code.Errors.Obsolete10;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    // Important Note
    // The new hybrid implementation doesn't actually need this
    // But if we add these overloads in an inherited class, they will be preferred to the real working ones
    // which would result in errors on AsDynamic(some-object) even though it should just work
    public partial class Razor12: IHasCodeErrorHelp
    {
        [PrivateApi]
        List<CodeHelp> IHasCodeErrorHelp.ErrorHelpers => new List<CodeHelp>
        {
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