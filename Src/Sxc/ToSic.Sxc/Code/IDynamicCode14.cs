using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;
// Disable warnings that properties should be marked as new
// Because we need them here as additional definition because of Razor problems with inherited interfaces
#pragma warning disable CS0108, CS0114

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Interface for Dynamic Code with enhancements after v12. It extends <see cref="IDynamicCode"/>
    /// 
    /// Dynamic Code is the API for files like Razor or WebApis.
    /// Supports many properties like App, etc. to ensure that the dynamic code has everything you need. <br />
    /// Also provides many Conversions between <see cref="IEntity"/> and <see cref="IDynamicEntity"/>.
    /// Important for dynamic code files like Razor or WebApi. Note that there are many overloads to ensure that AsDynamic and AsEntity "just work" even if you give them the original data.
    /// </summary>
    [PrivateApi("WIP v14.02")]
    public interface IDynamicCode14<out TModel, out TKit> : IDynamicCode<TModel, TKit>
        where TModel : class
        where TKit : Kit
    {
        #region IDynamicCode Repeats - keep this in sync
        // **************************************************
        // WARNING
        // **************************************************
        // Razor has a small problem with interfaces inheriting interfaces. 
        // If an object is of an interface which inherits another interface
        // then Razor will not find methods of the root interface and give errors like
        // Microsoft.CSharp.RuntimeBinder.RuntimeBinderException: 'ToSic.Sxc.Code.IDynamicCode12' does not contain a definition for 'AsList' at CallSite.Target
        //
        // Because of this, we repeat the ENTIRE definition for IDynamicCode here
        // Make sure they remain in-sync
        // **************************************************


        /// <summary>
        /// Get a service from the EAV / 2sxc Dependency Injection. 
        /// </summary>
        /// <typeparam name="TService">Interface (preferred) or Class which is needed</typeparam>
        /// <returns>An object of the type or interface requested</returns>
        /// <remarks>Added in 2sxc 11.11</remarks>
        TService GetService<TService>();

        /// <summary>
        /// A fully prepared <see cref="IApp"/> object letting you access all the data and queries in the current app. 
        /// </summary>
        /// <returns>The current app</returns>
        IApp App { get; }

        /// <summary>
        /// The data prepared for the current Code. Usually user data which was manually added to the instance, but can also be a query.
        /// </summary>
        /// <returns>
        /// An <see cref="IBlockDataSource"/> which is as <see cref="IDataSource"/>.</returns>
        IBlockDataSource Data { get; }

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
        /// A <see cref="ILinkService"/> object.
        /// </returns>
        ILinkService Link { get; }

        #endregion

        #region Edit

        /// <summary>
        /// Helper commands to enable in-page editing functionality
        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
        /// </summary>
        /// <returns>
        /// An <see cref="IEditService"/> object.
        /// </returns>
        IEditService Edit { get; }
        #endregion

        #region AsDynamic for Strings

        /// <summary>
        /// Take a json and provide it as a dynamic object to the code
        /// </summary>
        /// <remarks>Added in 2sxc 10.22.00</remarks>
        /// <param name="json">the original json string</param>
        /// <param name="fallback">
        /// Alternate string to use, if the original json can't parse.
        /// Can also be null or the word "error" if you would prefer an error to be thrown.</param>
        /// <returns>A dynamic object representing the original json.
        /// If it can't be parsed, it will parse the fallback, which by default is an empty empty dynamic object.
        /// If you provide null for the fallback, then you will get null back.
        /// </returns>
        dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson);

        #endregion 

        #region AsDynamic for Entities

        /// <summary>
        /// Wraps an entity into a <see cref="IDynamicEntity"/>
        /// </summary>
        /// <param name="entity">the original object</param>
        /// <returns>a dynamic object for easier coding</returns>
        dynamic AsDynamic(IEntity entity);


        /// <summary>
        /// Convert a dynamic entity and return itself again. This is so coders don't have to worry if the original object was an <see cref="IEntity"/> or a <see cref="IDynamicEntity"/> in the first place. 
        /// </summary>
        /// <param name="dynamicEntity">the original object</param>
        /// <returns>a dynamic object for easier coding</returns>
        dynamic AsDynamic(object dynamicEntity);


        #endregion

        #region AsEntity

        /// <summary>
        /// Unwraps a dynamic entity back into the underlying <see cref="IEntity"/>
        /// </summary>
        /// <param name="dynamicEntity">the wrapped IEntity</param>
        /// <returns>A normal IEntity</returns>
        IEntity AsEntity(object dynamicEntity);

        #endregion

        #region AsList

        /// <summary>
        /// Converts a list of <see cref="IEntity"/> objects into a list of <see cref="IDynamicEntity"/> objects. 
        /// </summary>
        /// <param name="list">typically a List/IEnumerable of Entities or DynamicEntities. <br/>
        /// Can also be a <see cref="IDataSource"/> in which case it uses the default stream. </param>
        /// <remarks>Added in 2sxc 10.21.00</remarks>
        /// <returns>a list of <see cref="IDynamicEntity"/> objects</returns>
        IEnumerable<dynamic> AsList(object list);

        #endregion


        #region Create Data Sources
        /// <summary>
        /// Create a <see cref="IDataSource"/> which will process data from the given stream.
        /// </summary>
        /// <param name="inStream">The stream which will be the default In of the new data-source.</param>
        /// <typeparam name="T">A data-source type - must be inherited from IDataSource</typeparam>
        /// <returns>A typed DataSource object</returns>
        T CreateSource<T>(IDataStream inStream) where T : IDataSource;


        /// <summary>
        /// Create a <see cref="IDataSource"/> which will process data from the given stream.
        /// </summary>
        /// <param name="inSource">The data source which will be the default In of the new data-source.</param>
        /// <param name="configurationProvider">An alternate configuration provider for the DataSource</param>
        /// <typeparam name="T">A data-source type - must be inherited from IDataSource</typeparam>
        /// <returns>A typed DataSource object</returns>
        T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null) where T : IDataSource;
        #endregion

        #region Context

        /// <summary>
        /// The CmsContext tells you about the environment, like what page and module we're running in.
        /// It's supposed to replace the Dnn object in Razor and WebAPI code.
        /// </summary>
        /// <remarks>
        /// New in v11.11
        /// </remarks>
        ICmsContext CmsContext { get; }

        #endregion



        #endregion



        #region Stuff added by DynamicCode12

        /// <summary>
        /// Convert one or many Entities and Dynamic entities into an <see cref="IDynamicStack"/>
        /// </summary>
        /// <param name="entities">one or more source object</param>
        /// <returns>a dynamic object for easier coding</returns>
        /// <remarks>
        /// New in 12.05
        /// </remarks>
        dynamic AsDynamic(params object[] entities);


        #region Convert-Service - removed in V14!

        #endregion

        #region Resources and Settings

        /// <summary>
        /// Resources for this Scenario. This is a dynamic object based on the <see cref="IDynamicStack"/>.
        ///
        /// It will combine both the Resources of the View and the App. The View-Resources will have priority. In future it may also include some global Resources. 
        /// 
        /// 🪒 Use in Razor: `@Resources.CtaButtonLabel`
        /// </summary>
        /// <remarks>New in 12.03</remarks>
        dynamic Resources { get; }

        /// <summary>
        /// Settings for this Scenario. This is a dynamic object based on the <see cref="IDynamicStack"/>.
        /// 
        /// It will combine both the Settings of the View and the App. The View-Settings will have priority. In future it may also include some global Settings. 
        /// 
        /// 🪒 Use in Razor: `@Settings.ItemsPerRow`
        /// </summary>
        /// <remarks>New in 12.03</remarks>
        dynamic Settings { get; }

        #endregion


        #region DevTools

        [PrivateApi("Still WIP")]
        IDevTools DevTools { get; }

        #endregion

        #endregion
    }
}
