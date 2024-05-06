#if NETFRAMEWORK
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Apps;
using ToSic.Sxc.DataSources.Internal.Compatibility;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent
{
    /// <remarks>
    /// Deprecation notice: this is an old interface in the "bad" SexyContent namespace.
    /// We'll probably keep it forever, but don't add any more features. Instead, put it on the new interface
    /// This will force developers to use the new interface without breaking compatibility
    /// </remarks>
    [Obsolete("Avoid this - it's in an old namespace. Use the ToSic.Sxc.Web.IDynamicCode")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IAppAndDataHelpers
    {
        IApp App { get; }
        IBlockDataSource Data { get; }

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
        dynamic AsDynamic(object dynamicEntity);

        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        IEntity AsEntity(object dynamicEntity);

        #region Create Data Sources
        /// <summary>
        /// Create a source with initial stream to attach...
        /// </summary>
        /// <returns></returns>
        T CreateSource<T>(IDataStream source) where T : IDataSource;
        IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine configurationProvider = null);
        T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource;
        #endregion

    }
}
#endif