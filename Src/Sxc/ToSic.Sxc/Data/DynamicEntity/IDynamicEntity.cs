using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using IEntity = ToSic.Eav.Data.IEntity;
#pragma warning disable CS0108, CS0114
#if !NETFRAMEWORK
#pragma warning disable CS0109
#endif

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a wrapper for IEntity objects. It provides nicer access to underlying properties
    /// and automatically handles things like multi-language etc.
    /// The underlying IEntity <see cref="IEntity"/> is in the Entity property. 
    /// <blockquote>
    /// Normally you will use it without caring about these internals. <br/>
    /// Please check @HowTo.DynamicCode.DynamicEntity
    /// </blockquote>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public partial interface IDynamicEntity:
#if NETFRAMEWORK
        SexyContent.Interfaces.IDynamicEntity,
#endif
        IEntityWrapper, IDynamicEntityBase, ISxcDynamicObject, ICanDebug
        // , ITypedItem - it can't inherit from this, because otherwise some commands like Parents() with no parameters or just one would be ambiguous
    {
        /// <summary>
        /// The underlying entity which provides all the data for the DynamicEntity
        /// </summary>
        /// <returns>
        /// An Entity object.
        /// </returns>
        new IEntity Entity { get; }

        /// <summary>
        /// The ID of the underlying entity.
        /// Use it for edit-functionality or just to have a unique number for this item.
        /// </summary>
        /// <remarks>If the entity doesn't exist, it will return 0</remarks>
        new int EntityId { get; }

        /// <summary>
        /// The guid of the underlying entity.
        /// </summary>
        /// <remarks>If the entity doesn't exist, it will return an empty guid</remarks>
        new Guid EntityGuid { get; }

        /// <summary>
        /// The title of this item. This is always available no matter what the underlying field for the title is. 
        /// </summary>
        /// <returns>
        /// The title of the underlying entity.
        /// In rare cases where no title-field is known, it can be null.
        /// It can also be null if there is no underlying entity. 
        /// </returns>
        /// <remarks>This returns a string which is usually what's expected. In previous versions (before v15) 2sxc it returned an object.</remarks>
        new string EntityTitle { get; }

        /// <summary>
        /// Get a Field-object of a property of this entity, to use with services like the <see cref="Services.IImageService"/> which also need more information like the metadata.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>
        /// History: Added in v13.10
        /// </remarks>
        IDynamicField Field(string name);


        /// <summary>
        /// The type name of the current entity. This provides the nice name like "Person" and not the technical internal StaticName
        /// </summary>
        string EntityType { get; }


        /// <summary>
        /// The type name of the current entity. This provides the nice name like "Person" and not the technical internal StaticName
        /// </summary>
        /// <remarks>
        /// Added in v13
        /// </remarks>
        IDynamicMetadata Metadata { get; }



        #region Publishing / Draft Information

        /// <summary>
        /// Get the draft item of this item if this is a content-item which is published, and has a draft.
        /// </summary>
        /// <returns>Returns a dynamic entity if there is a draft, or null if there is no draft.</returns>
        new dynamic GetDraft();

        /// <summary>
        /// Get the published item of this item if this is a content-item which is draft, and has a published.
        /// </summary>
        /// <returns>Returns a dynamic entity if there is a draft, or null if there is no draft.</returns>
        new dynamic GetPublished();

        // This property would also work on the normal dynamic interface, but we want them to appear in the documentation so we're adding them
        /// <summary>
        /// Tells us if this data item is published or still draft. Default is true.
        /// </summary>
        bool IsPublished { get; }

        #endregion


        #region parents / children

        /// <summary>
        /// A dynamic list of entities which point to this item. Important for LINQ style querying or just
        /// working with various lists. Note that for getting child items of this item you
        /// can just use the properties, like content.Authors. <br/>
        /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
        /// </summary>
        /// <param name="type">Optional type filter - would only return items of this type. </param>
        /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
        /// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
        /// <remarks>New in 9.42 - note also that the parameter-order is reversed to the Children()</remarks>
        List<IDynamicEntity> Parents(string type = null, string field = null);

        /// <summary>
        /// A dynamic list of sub-items. Important for LINQ style querying or just
        /// working with various lists. Note that for getting child items of this item you
        /// can just use the properties, like content.Authors. <br/>
        /// But using Children("Authors", typeName) gives you the ability to restrict to a type.  <br/>
        /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
        /// </summary>
        /// <param name="type">Optional type filter - would only return items of this type. </param>
        /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
        /// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
        /// <remarks>New in 10.21.00 - note also that the parameter-order is reversed to the Parents()</remarks>
        List<IDynamicEntity> Children(string field = null, string type = null);
        #endregion 

        /// <summary>
        /// Contains presentation settings for an item - if they exist.
        /// This is a functionality of the CMS, where an instance of an item can be configured to show in a specific way.
        /// Normally it's used when something like an address has various show-settings (like how the map should visualize this address).
        /// The presentation-info is therefor not-null IF <br/>
        /// - the content <em>belongs</em> to this module instance <br/>
        /// - the view-configuration of this module is configured to have presentation items <br />
        /// - there is either a default presentation configured in the view, or the user has manually edited the presentation settings
        /// </summary>
        /// <returns>
        /// An <see cref="IDynamicEntity"/> with the presentation item (or the demo-presentation), otherwise null.
        /// </returns>
        dynamic Presentation { get; }

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        DynamicEntity.MyServices _Services { get; }

        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
        ///// <summary>
        ///// Activate debugging, so that you'll see details in [Insights](xref:NetCode.Debug.Insights.Index) how the value was retrieved.
        ///// </summary>
        ///// <param name="debug"></param>
        //void SetDebug(bool debug);

    }
}
