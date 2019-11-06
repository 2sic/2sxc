using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.Eav.ValueProviders;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Interfaces
{
    /// <summary>
    /// Dynamic code files like Razor or WebApis.
    /// Supports many properties like App, etc. to ensure that the dynamic code has everything you need. <br />
    /// Also provides many Conversions between <see cref="IEntity"/> and <see cref="IDynamicEntity"/>.
    /// Important for dynamic code files like Razor or WebApi. Note that there are many overloads to ensure that AsDynamic and AsEntity "just work" even if you give them the original data. 
    /// </summary>
    [PublicApi]
#pragma warning disable 618
    public interface IDynamicCode: SexyContent.IAppAndDataHelpers, ISharedCodeBuilder // inherit from old namespace to ensure compatibility
#pragma warning restore 618
    {
        #region Content and Header
        /// <summary>
        /// The content object of the current razor view - IF the current view has content.
        /// If the view is a list, it will return the first item. 
        /// Will be null otherwise.
        /// To tell if it's the demo/default item, use <see cref="IDynamicEntity.IsDemoItem"/>.
        /// </summary>
        /// <returns>A <see cref="IDynamicEntity"/> object with the current content - or null.</returns>
        dynamic Content { get; }

        /// <summary>
        /// The header object of the current razor view, if it's a list and has a header object.
        /// If it's a list and doesn't have a header (and no default), it will return null.
        /// To tell if it's the demo/default item, use <see cref="IDynamicEntity.IsDemoItem"/>.
        /// </summary>
        /// <returns>A <see cref="IDynamicEntity"/> object with the current content.</returns>
        /// <remarks>
        /// Introduced in 2sxc 10.10 - previously it was called ListContent, now deprecated.
        /// </remarks>
        dynamic Header { get; }

        #endregion

        #region AsAdam

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        IAdamFolder AsAdam(IDynamicEntity entity, string fieldName);

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        IAdamFolder AsAdam(IEntity entity, string fieldName);

        #endregion

        #region Linking

        /// <summary>
        /// Link helper object to create the correct links
        /// </summary>
        /// <returns>
        /// A <see cref="ILinkHelper"/> object.
        /// </returns>
        ILinkHelper Link { get; }

        #endregion

        #region Edit

        /// <summary>
        /// Helper commands to enable in-page editing functionality
        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
        /// </summary>
        /// <returns>
        /// An <see cref="IInPageEditingSystem"/> object.
        /// </returns>
        IInPageEditingSystem Edit { get; }
        #endregion


        #region AsDynamic and AsEntity

        /// <summary>
        /// Wraps an entity into a <see cref="IDynamicEntity"/>
        /// </summary>
        /// <param name="entity">the original object</param>
        /// <returns>a dynamic object for easier coding</returns>
        new dynamic AsDynamic(IEntity entity);


        /// <summary>
        /// Convert a dynamic entity and return itself again. This is so coders don't have to worry if the original object was an <see cref="IEntity"/> or a <see cref="IDynamicEntity"/> in the first place. 
        /// </summary>
        /// <param name="dynamicEntity">the original object</param>
        /// <returns>a dynamic object for easier coding</returns>
        new dynamic AsDynamic(dynamic dynamicEntity);


        /// <summary>
        /// Converts a dictionary-style list of many <see cref="IEntity"/> objects into a key-value pair of <see cref="IDynamicEntity"/> objects. 
        /// </summary>
        /// <param name="entityKeyValuePair"></param>
        /// <returns></returns>
        new dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair);


        /// <summary>
        /// Converts a list of entities from a <see cref="IDataSource"/> into a list of <see cref="IDynamicEntity"/> objects. 
        /// </summary>
        /// <param name="stream">the stream containing <see cref="IEntity"/> items</param>
        /// <returns>a list of <see cref="IDynamicEntity"/> objects</returns>
        new IEnumerable<dynamic> AsDynamic(IDataStream stream);


        /// <summary>
        /// Converts a list of <see cref="IEntity"/> objects into a list of <see cref="IDynamicEntity"/> objects. 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns>a list of <see cref="IDynamicEntity"/> objects</returns>
        new IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities);



        /// <summary>
        /// Unwraps a dynamic entity back into the underlying <see cref="IEntity"/>
        /// </summary>
        /// <param name="dynamicEntity">the wrapped IEntity</param>
        /// <returns>A normal IEntity</returns>
        new IEntity AsEntity(dynamic dynamicEntity);

        // todo: there should be more overloads for AsEntity, but I assume we never needed it, so we don't have them
        #endregion

        #region Create Data Sources
        /// <summary>
        /// Create a <see cref="IDataSource"/> which will process data from the given stream.
        /// </summary>
        /// <param name="inStream">The stream which will be the default In of the new data-source.</param>
        /// <typeparam name="T">A data-source type - must be inherited from IDataSource</typeparam>
        /// <returns>A typed DataSource object</returns>
        new T CreateSource<T>(IDataStream inStream) where T: IDataSource;

        /// <summary>
        /// Create a <see cref="IDataSource"/> which will process data from the given stream.
        /// </summary>
        /// <returns>A typed DataSource object</returns>
        [Obsolete("Please use the CreateSource<T> overload instead.")]
        [PrivateApi]
        new IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IValueCollectionProvider configurationProvider = null);

        /// <summary>
        /// Create a <see cref="IDataSource"/> which will process data from the given stream.
        /// </summary>
        /// <param name="inSource">The data source which will be the default In of the new data-source.</param>
        /// <param name="configurationProvider">An alternate configuration provider for the DataSource</param>
        /// <typeparam name="T">A data-source type - must be inherited from IDataSource</typeparam>
        /// <returns>A typed DataSource object</returns>
        new T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null) where T : IDataSource;
        #endregion


    }
}
