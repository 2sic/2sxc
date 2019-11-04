using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Interfaces;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Interfaces
{
    /// <summary>
    /// This is a wrapper for IEntity objects. It provides nicer access to underlying properties
    /// and automatically handles things like multi-language etc.
    /// The underlying IEntity <see cref="IEntity"/> is in the Entity property.
    /// </summary>
    [PublicApi]
    public interface IDynamicEntity: SexyContent.Interfaces.IDynamicEntity
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
        /// <returns>
        /// The same Id as you would get from .Entity.EntityId
        /// </returns>
        new int EntityId { get; }

        /// <summary>
        /// The guid of the underlying entity.
        /// </summary>
        /// <returns>
        /// The same Guid as you would get from .Entity.EntityGuid
        /// </returns>
        new Guid EntityGuid { get; }

        /// <summary>
        /// The title of this item. This is always available no matter what the underlying field for the title is. 
        /// </summary>
        /// <returns>
        /// The title of the underlying entity
        /// </returns>
        new object EntityTitle { get; }

        /// <summary>
        /// Get a value of the entity. Usually you will NOT use this, because the default access
        /// is usually content.FirstName - which will give you the same things as content.Get("FirstName").
        /// The only reason to use this is when you dynamically assemble the field name in your code, like
        /// when using App.Resources or similar use cases. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>An object which can be either a string, number, boolean or List&lt;IDynamicEntity&gt;, depending on the field type. Will return null if the field was not found. </returns>
        new dynamic Get(string name);

        /// <summary>
        /// Get the draft item of this item.  
        /// </summary>
        /// <returns>Returns a dynamic entity if there is a draft, or null if there is no draft.</returns>
        new dynamic GetDraft();

        /// <summary>
        /// Get the published item of this item.  
        /// </summary>
        /// <returns>Returns a dynamic entity if there is a draft, or null if there is no draft.</returns>
        new dynamic GetPublished();

        /// <summary>
        /// Many templates show demo data. If the template code must know if it's the demo item or
        /// real data, use IsDemoItem.
        /// Added in 2sxc 10.07.
        /// </summary>
        /// <returns>
        /// True if this is the item configured in the view-settings, false if not.
        /// </returns>
        new bool IsDemoItem { get; }

        /// <summary>
        /// A dynamic list of entities which point to this item. Important for LINQ style querying or just
        /// working with various lists. Note that for getting child items of this item you should just use the properties, like
        /// content.Authors. 
        /// Please check the tutorials on 2sxc.org/dnn-tutorials/ for more info. 
        /// </summary>
        /// <param name="type">Optional type filter - would only return items of this type. </param>
        /// <param name="field">Optional field filter - would only return items that point to the current item in a specific field name.</param>
        /// <returns>A list of all items pointing here (filtered), converted to DynamicEntity for convenience.</returns>
        new List<IDynamicEntity> Parents(string type = null, string field = null);

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
    }
}
