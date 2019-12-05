using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Web
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

        /// <summary>
        /// A fully prepared <see cref="IApp"/> object letting you access all the data and queries in the current app. 
        /// </summary>
        /// <returns>The current app</returns>
        new IApp App { get; }

        /// <summary>
        /// The data prepared for the current Code. Usually user data which was manually added to the instance, but can also be a query.
        /// </summary>
        /// <returns>
        /// An <see cref="IBlockDataSource"/> which is as <see cref="IDataSource"/>.</returns>
        new IBlockDataSource Data { get; }

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
        IFolder AsAdam(IDynamicEntity entity, string fieldName);

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        IFolder AsAdam(IEntity entity, string fieldName);

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

        //#region Configure a just generated object

        //[PrivateApi("WIP")]
        //void ConfigurePage(IDynamicCode parentPage);


        //#endregion

        #region AsDynamic for Strings

        /// <summary>
        /// Take a json and provide it as a dynamic object to the code
        /// </summary>
        /// <remarks>
        /// New in 2sxc 10.20
        /// </remarks>
        /// <param name="json">the original json string</param>
        /// <param name="fallback">
        /// Alternate string to use, if the original json can't parse.
        /// Can also be null or the word "error" if you would prefer an error to be thrown.</param>
        /// <returns>A dynamic object representing the original json.
        /// If it can't be parsed, it will parse the fallback, which by default is an empty empty dynamic object.
        /// If you provide null for the fallback, then you will get null back.
        /// </returns>
        /// <remarks>Added in 2sxc 10.20.06</remarks>
        dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson);

        #endregion 

        #region AsDynamic for Entities

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


        // 2019-11-28 2dm removed from primary IDynamicCode interface, now only in legacyinterface
        ///// <summary>
        ///// Converts a dictionary-style list of many <see cref="IEntity"/> objects into a key-value pair of <see cref="IDynamicEntity"/> objects. 
        ///// </summary>
        ///// <param name="entityKeyValuePair"></param>
        ///// <returns></returns>
        //new dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair);


        /// <summary>
        /// Converts a list of entities from a <see cref="IDataStream"/> into a list of <see cref="IDynamicEntity"/> objects. 
        /// </summary>
        /// <param name="stream">the stream containing <see cref="IEntity"/> items</param>
        /// <returns>a list of <see cref="IDynamicEntity"/> objects</returns>
        new IEnumerable<dynamic> AsDynamic(IDataStream stream);

        /// <summary>
        /// Converts a list of entities from a <see cref="IDataSource"/> into a list of <see cref="IDynamicEntity"/> objects. <br/>
        /// It will use the "Default" stream to do this. For any other stream, use the AsDynamic(YourDataSource["streamname"]) syntax
        /// </summary>
        /// <param name="source">the DataSource containing <see cref="IEntity"/> items</param>
        /// <remarks>Added in 2sxc 10.20.06</remarks>
        /// <returns>a list of <see cref="IDynamicEntity"/> objects</returns>
        new IEnumerable<dynamic> AsDynamic(IDataSource source);

        /// <summary>
        /// Converts a list of <see cref="IEntity"/> objects into a list of <see cref="IDynamicEntity"/> objects. 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns>a list of <see cref="IDynamicEntity"/> objects</returns>
        new IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities);

        #endregion

        #region AsEntity

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
        /// <param name="inSource">The data source which will be the default In of the new data-source.</param>
        /// <param name="configurationProvider">An alternate configuration provider for the DataSource</param>
        /// <typeparam name="T">A data-source type - must be inherited from IDataSource</typeparam>
        /// <returns>A typed DataSource object</returns>
        new T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null) where T : IDataSource;
        #endregion


    }
}
