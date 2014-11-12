using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent
{
    interface IFrontApi
    {
        App App { get; }
        ViewDataSource Data { get; }
        DnnHelper Dnn { get; }

        /// <summary>
        /// Transform a IEntity to a DynamicEntity as dynamic object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        dynamic AsDynamic(IEntity entity);

        /// <summary>
        /// Makes sure a dynamicEntity could be wrapped in AsDynamic()
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        dynamic AsDynamic(dynamic dynamicEntity);

        /// <summary>
        /// Returns the value of a KeyValuePair as DynamicEntity
        /// </summary>
        /// <param name="entityKeyValuePair"></param>
        /// <returns></returns>
        dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair);

        /// <summary>
        /// In case AsDynamic is used with Data["name"]
        /// </summary>
        /// <returns></returns>
        IEnumerable<dynamic> AsDynamic(IDataStream stream);

        /// <summary>
        /// In case AsDynamic is used with Data["name"].List
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        IEnumerable<dynamic> AsDynamic(IDictionary<int, IEntity> list);

        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        IEntity AsEntity(dynamic dynamicEntity);

        /// <summary>
        /// Returns a list of DynamicEntities
        /// </summary>
        /// <param name="entities">List of entities</param>
        /// <returns></returns>
        IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities);

        /// <summary>
        /// Create a source with initial stream to attach...
        /// </summary>
        /// <returns></returns>
        T CreateSource<T>(IDataStream inStream);
        ToSic.Eav.DataSources.IDataSource CreateSource(string typeName = "", ToSic.Eav.DataSources.IDataSource inSource = null, IConfigurationProvider configurationProvider = null);
        T CreateSource<T>(ToSic.Eav.DataSources.IDataSource inSource = null, IConfigurationProvider configurationProvider = null);

    }
}