using System.Dynamic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data;

/// <summary>
/// Case insensitive dynamic read-object for JSON. <br/>
/// Used in various cases where you start with JSON and want to
/// provide the contents to custom code without having to mess with
/// JS/C# code style differences. <br/>
/// You will usually do things like `AsDynamic(jsonString).FirstName` etc.
/// </summary>
[PrivateApi("was Internal-API till v17 - just use the objects from AsDynamic(...), don't use this directly")]
[JsonConverter(typeof(DynamicJsonConverter))]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class DynamicJacket: DynamicJacketBase<JsonObject>
{
    /// <inheritdoc />
    [PrivateApi]
    internal DynamicJacket(CodeJsonWrapper wrapper, PreWrapJsonObject preWrap) : base(wrapper, preWrap.GetContents())
    {
        PreWrapJson = preWrap;
    }

    private PreWrapJsonObject PreWrapJson { get; }

    internal override IPreWrap PreWrap => PreWrapJson;

    #region Basic Jacket Properties

    public override bool IsList => false;

    /// <summary>
    /// Count array items or object properties
    /// </summary>
    public override int Count => UnwrappedContents.Count;

    #endregion

    /// <inheritdoc />



    /// <summary>
    /// Enable enumeration. Will return the keys, not the values. <br/>
    /// Use the [key] accessor to get the values as <see cref="DynamicJacket"/> or <see cref="DynamicJacketList"/>
    /// </summary>
    /// <returns>the string names of the keys</returns>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public override IEnumerator<object> GetEnumerator() => UnwrappedContents.Select(p => p.Key).GetEnumerator();


    /// <summary>
    /// Access the properties of this object.
    /// </summary>
    /// <remarks>
    /// Note that <strong>this</strong> accessor is case insensitive
    /// </remarks>
    /// <param name="key">the key, case-insensitive</param>
    /// <returns>A value (string, int etc.), <see cref="DynamicJacket"/> or <see cref="DynamicJacketList"/></returns>
    public object this[string key] => PreWrap.TryGetWrap(key).Result;

    // 2023-08-17 2dm - completely removed this, I can't imagine it actually being used anywhere.
    ///// <summary>
    ///// Access the properties of this object.
    ///// </summary>
    ///// <param name="key">the key</param>
    ///// <param name="caseSensitive">true if case-sensitive, false if not</param>
    ///// <returns>A value (string, int etc.), <see cref="DynamicJacket"/> or <see cref="DynamicJacketList"/></returns>
    //[PrivateApi("2023-08-07 2dm made private now, before it was public. It's very exotic, so I don't think it's used anywhere, consider removing this later on")]
    //public object this[string key, bool caseSensitive]
    //    => PreWrap.FindValueOrNull(key, caseSensitive 
    //        ? Ordinal
    //        : InvariantCultureIgnoreCase, null);


    #region Private TryGetMember

    /// <summary>
    /// Performs a case-insensitive value look-up
    /// </summary>
    /// <param name="binder">.net binder object</param>
    /// <param name="result">usually a <see cref="DynamicJacket"/>, <see cref="DynamicJacketList"/> or null</param>
    /// <returns>always returns true, to avoid errors</returns>
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        result = PreWrap.TryGetWrap(binder.Name).Result;
        // always say it was found to prevent runtime errors
        return true;
    }

    #endregion

    /// <inheritdoc />
    public override object this[int index] => (_propertyArray ??= UnwrappedContents.Select(p => p.Value).ToArray())[index];
    private JsonNode[] _propertyArray;

}