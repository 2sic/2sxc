using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using IEntity = ToSic.Eav.Data.IEntity;

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
        IEntityWrapper, IDynamicEntityBase, ISxcDynamicObject
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
        new object EntityTitle { get; }


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
        IMetadataOf Metadata { get; }

        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityGet and IDynamicStack */
        /// <summary>
        /// Get a value of the entity. Usually you will prefer the quick access like
        /// @content.FirstName - which will give you the same things as content.Get("FirstName").
        /// There are two cases to use this:
        /// - when you dynamically assemble the field name in your code, like when using App.Resources or similar use cases.
        /// - to access a field which has a conflicting name with this object, like Get("Parents")
        /// </summary>
        /// <param name="name"></param>
        /// <returns>An object which can be either a string, number, boolean or List&lt;IDynamicEntity&gt;, depending on the field type. Will return null if the field was not found. </returns>
        new dynamic Get(string name);


        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityGet and IDynamicStack */
        /// <summary>
        /// Get a property using the string name. Only needed in special situations, as most cases can use the object.name directly
        /// </summary>
        /// <param name="name">the property name. </param>
        /// <param name="noParamOrder">
        /// This should enforce the convention that all following parameters (which are optional) must explicitly use the parameter name.
        /// So <code>Get("FirstName", "en")</code> won't work, you must use <code>Get("FirstName", language: "en")</code> and similar
        /// </param>
        /// <param name="language">Optional language code - like "de-ch" to prioritize that language</param>
        /// <param name="convertLinks">Optionally turn off if links like file:72 are looked up to a real link. Default is true.</param>
        /// <param name="debug">Set true to see more details in [Insights](xref:NetCode.Debug.Insights.Index) how the value was retrieved.</param>
        /// <returns>a dynamically typed result, can be string, bool, etc.</returns>
        new dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string noParamOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true,
            bool? debug = null
        );



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


        /// <summary>
        /// Many templates show demo data. If the template code must know if it's the demo item or
        /// real data, use IsDemoItem.
        /// </summary>
        /// <returns>
        /// True if this is the item configured in the view-settings, false if not.
        /// </returns>
        /// <remarks>New in 10.07</remarks>
        bool IsDemoItem { get; }

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
        DynamicEntityDependencies _Dependencies { get; }

        /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityGet and IDynamicStack */
        /// <summary>
        /// Activate debugging, so that you'll see details in [Insights](xref:NetCode.Debug.Insights.Index) how the value was retrieved.
        /// </summary>
        /// <param name="debug"></param>
        void SetDebug(bool debug);

    }
}
