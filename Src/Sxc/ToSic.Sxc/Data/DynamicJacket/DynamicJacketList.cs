using System.Dynamic;
using System.Text.Json.Nodes;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data;

/// <summary>
/// This is a DynamicJacket for JSON arrays. You can enumerate through it. 
/// </summary>
[PrivateApi("was Internal-API till v17 - just use the objects from AsDynamic, don't use this directly")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class DynamicJacketList : DynamicJacketBase<JsonArray>, IReadOnlyList<object>
{
    /// <inheritdoc />
    internal DynamicJacketList(CodeJsonWrapper wrapper, PreWrapJsonArray preWrap) : base(wrapper, preWrap.GetContents())
    {
        PreWrapList = preWrap;
    }
    private PreWrapJsonArray PreWrapList { get; }

    internal override IPreWrap PreWrap => PreWrapList;

    #region Basic Jacket Properties

    /// <inheritdoc />
    public override bool IsList => true;

    /// <summary>
    /// Count array items or object properties
    /// </summary>
    public override int Count => UnwrappedContents.Count;

    #endregion



    [PrivateApi]
    public override IEnumerator<object> GetEnumerator() 
        => UnwrappedContents.Select(o => Wrapper.IfJsonGetValueOrJacket(o)).GetEnumerator();

    [PrivateApi]
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        result = null;
        return true;
    }


    /// <summary>
    /// Access the items in this object - but only if the underlying object is an array. 
    /// </summary>
    /// <param name="index">array index</param>
    /// <returns>the item or an error if not found</returns>
    public override object this[int index] => Wrapper.IfJsonGetValueOrJacket(UnwrappedContents[index]);
}