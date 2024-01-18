using ToSic.Sxc.Data.Internal.Docs;

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
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    dynamic GetSource(string name);

    [PrivateApi("Never published in docs")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    dynamic GetStack(params string[] names);

    #region Get and Get<T>

    /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
    /// <inheritdoc cref="DynamicEntityDocs.Get(string)"/>
    new dynamic Get(string name);

    /* IMPORTANT: KEEP THIS DEFINITION AND DOCS IN SYNC BETWEEN IDynamicEntity, IDynamicEntityBase and IDynamicStack */
    /// <inheritdoc cref="DynamicEntityDocs.Get(string, NoParamOrder, string, bool, bool?)"/>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    dynamic Get(string name, NoParamOrder noParamOrder = default, string language = null, bool convertLinks = true, bool? debug = null);

    /// <inheritdoc cref="DynamicEntityDocs.Get{TValue}(string)"/>
    TValue Get<TValue>(string name);

    /// <inheritdoc cref="DynamicEntityDocs.Get{TValue}(string, NoParamOrder, TValue)"/>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default);

    #endregion

    #region Any** IMPORTANT: These are just fake properties for documentation - Keep in Sync between IDynamicEntity and IDynamicStack

    /// <inheritdoc cref="IDynamicAnythingDocs.AnyProperty"/>
    public dynamic AnyProperty { get; }

    #endregion

}