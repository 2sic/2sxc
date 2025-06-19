﻿using ToSic.Eav.Data.Entities.Sys.Wrappers;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Sys.Docs;
using ToSic.Sxc.Data.Sys.Factory;

#if !NETFRAMEWORK
#pragma warning disable CS0108, CS0114
#pragma warning disable CS0109
#endif

namespace ToSic.Sxc.Data;

/// <summary>
/// This is an older _dynamic_ wrapper for IEntity objects. It provides _dynamic_ access to underlying properties.
/// </summary>
/// <remarks>
/// It provides nicer access to underlying properties
/// and automatically handles things like multi-language etc.
/// The underlying IEntity <see cref="IEntity"/> is in the Entity property.
/// 
/// <blockquote>
/// This is an older way to work with entities and not recommended anymore.
/// You should use a newer base class such as `RazorTyped`, there Dynamic objects are not used anymore.
/// </blockquote>
/// 
/// Normally you will use it without caring about these internals. <br/>
/// Please check @HowTo.DynamicCode.DynamicEntity
/// 
/// </remarks>
[PublicApi]
public partial interface IDynamicEntity:
    IEntityWrapper, 
    ISxcDynamicObject, 
    ICanDebug
{
    /// <summary>
    /// Get a Field-object of a property of this entity, to use with services like the <see cref="Services.IImageService"/> which also need more information like the metadata.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <remarks>
    /// History: Added in v13.10
    /// </remarks>
    IField? Field(string name);

    /// <summary>
    /// The type name of the current entity. This provides the nice name like "Person" and not the technical internal StaticName
    /// </summary>
    /// <remarks>
    /// * Added in v13
    /// * Changed type name to `IMetadata` from `IDynamicMetadata` in 16.02; same type, different type name
    /// </remarks>
    ITypedMetadata Metadata { get; }

    #region Publishing / Draft Information

    /// <summary>
    /// Get the draft item of this item if this is a content-item which is published, and has a draft.
    /// </summary>
    /// <returns>Returns a dynamic entity if there is a draft, or null if there is no draft.</returns>
    dynamic? GetDraft();

    /// <summary>
    /// Get the published item of this item if this is a content-item which is draft, and has a published.
    /// </summary>
    /// <returns>Returns a dynamic entity if there is a draft, or null if there is no draft.</returns>
    dynamic? GetPublished();

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
    List<IDynamicEntity?> Parents(string? type = null, string? field = null);

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
    List<IDynamicEntity?> Children(string? field = null, string? type = null);

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
    dynamic? Presentation { get; }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    // ReSharper disable once InconsistentNaming
    ICodeDataFactory Cdf {get; }


    /* IMPORTANT: These are just fake properties for documentation - Keep in Sync between IDynamicEntity and IDynamicStack */

    /// <inheritdoc cref="IDynamicAnythingDocs.AnyProperty"/>
    public dynamic? AnyProperty { get; }

}