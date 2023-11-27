using System.Collections.Generic;
using System;
using ToSic.Eav.Data;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data.Docs;

namespace ToSic.Sxc.Data;

/// <summary>
/// This is a dynamic object which contains multiple dynamic objects (Sources).
/// It will try to find a value inside each source in the order the Sources are managed. 
/// </summary>
/// <remarks>New in 12.02</remarks>
[PublicApi]
public interface IDynamicStack: ISxcDynamicObject, ICanDebug, ICanGetByName
{
    /// <summary>
    /// Get a source object which is used in the stack. Returned as a dynamic object. 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>A dynamic object like a <see cref="IDynamicEntity"/> or similar. If not found, it will return a source which just-works, but doesn't have data. </returns>
    /// <remarks>
    /// Added in 2sxc 12.03
    /// </remarks>
    [PrivateApi("was public till v16.02, but since I'm not sure if it is really used, we decided to hide it again since it's probably not an important API")]
    dynamic GetSource(string name);

    [PrivateApi("Never published in docs")]
    dynamic GetStack(params string[] names);

    #region Get and Get<T>

    /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
    /// <inheritdoc cref="DynamicEntityDocs.Get(string)"/>
    new dynamic Get(string name);

    /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
    /// <inheritdoc cref="DynamicEntityDocs.Get(string, string, string, bool, bool?)"/>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    dynamic Get(string name, NoParamOrder noParamOrder = default, string language = null, bool convertLinks = true, bool? debug = null);

    /// <inheritdoc cref="DynamicEntityDocs.Get{TValue}(string)"/>
    TValue Get<TValue>(string name);

    /// <inheritdoc cref="DynamicEntityDocs.Get{TValue}(string, string, TValue)"/>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default);

    #endregion

    #region Any** IMPORTANT: These are just fake properties for documentation - Keep in Sync between IDynamicEntity and IDynamicStack

    /// <inheritdoc cref="IDynamicAnythingDocs.AnyBooleanProperty"/>
    bool AnyBooleanProperty { get; }

    /// <inheritdoc cref="IDynamicAnythingDocs.AnyDateTimeProperty"/>
    DateTime AnyDateTimeProperty { get; }

    /// <inheritdoc cref="IDynamicAnythingDocs.AnyChildrenProperty"/>
    IEnumerable<IDynamicEntity> AnyChildrenProperty { get; }

    // 2023-08-12 2dm - removed this, as we don't officially have the Json Type any more
    //string AnyJsonProperty { get; }

    /// <inheritdoc cref="IDynamicAnythingDocs.AnyLinkOrFileProperty"/>
    string AnyLinkOrFileProperty { get; }

    /// <inheritdoc cref="IDynamicAnythingDocs.AnyNumberProperty"/>
    decimal AnyNumberProperty { get; }

    /// <inheritdoc cref="IDynamicAnythingDocs.AnyStringProperty"/>
    string AnyStringProperty { get; }

    // 2023-08-11 2dm - disable this, believe the instructions were a bit wrong
    ///// <summary>
    ///// If this DynamicEntity carries a list of items (for example a `BlogPost.Tags` which behaves as the first Tag, but also carries all the tags in it)
    ///// Then you can also use DynamicCode to directly navigate to a sub-item, like `Blogs.Tags.WebDesign`. 
    ///// </summary>
    ///// <remarks>New in 12.03</remarks>
    //IEnumerable<DynamicEntity> AnyTitleOfAnEntityInTheList { get; }

    #endregion

}