using System.Collections;
using System.Dynamic;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Data;
using ToSic.Sxc.Data.Internal.Dynamic;

namespace ToSic.Sxc.Data.Internal.Stack;

// Must be pbulic so that `Resources.Get...` work
[PrivateApi("Keep implementation hidden, only publish interface")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DynamicStack: DynamicObject,
    IWrapper<IPropertyStack>,
    IDynamicStack,
    IHasPropLookup,
    ISxcDynamicObject,
    IEnumerable, // note: not sure why it supports this, but it has been this way for a long time
    ICanDebug
{
    #region Constructor and Helpers (Composition)

    public DynamicStack(string name, Internal.CodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources)
    {
        Cdf = cdf;
        var stack = new PropertyStack().Init(name, sources);
        _stack = stack;
        PropertyLookup = new PropLookupStack(stack, () => Debug);
    }
    // ReSharper disable once InconsistentNaming
    [PrivateApi] public Internal.CodeDataFactory Cdf { get; }
    private readonly IPropertyStack _stack;
    private const bool Strict = false;

    [PrivateApi]
    public IPropertyLookup PropertyLookup { get; }

    [PrivateApi]
    internal GetAndConvertHelper GetHelper => _getHelper ??= new(this, Cdf, Strict, childrenShouldBeDynamic: true, canDebug: this);
    private GetAndConvertHelper _getHelper;

    [PrivateApi]
    internal SubDataFactory SubDataFactory => _subData ??= new(Cdf, Strict, canDebug: this);
    private SubDataFactory _subData;

    /// <inheritdoc />
    public bool Debug { get; set; }

    #endregion



    public override bool TryGetMember(GetMemberBinder binder, out object result) 
        => CodeDynHelper.TryGetMemberAndRespectStrict(GetHelper, binder, out result);


    /// <inheritdoc />
    [PrivateApi]
    public IPropertyStack GetContents() => _stack;

    /// <inheritdoc />
    [PrivateApi("was public till v16.02, but since I'm not sure if it is really used, we decided to hide it again since it's probably not an important API")]
    public dynamic GetSource(string name)
    {
        var source = _stack.GetSource(name)
                     // If not found, create a fake one
                     ?? Cdf.FakeEntity(Cdf.BlockOrNull?.AppId);

        return SourceToDynamicEntity(source);
    }

    /// <inheritdoc />
    [PrivateApi("Never published in docs")]
    public dynamic GetStack(params string[] names)
    {
        var l = GetHelper.LogOrNull.Fn<object>();
        var newStack = _stack.GetStack(GetHelper.LogOrNull, names);
        var newDynStack = new DynamicStack("New", Cdf, newStack.Sources);
        return l.Return(newDynStack);
    }

    private IDynamicEntity SourceToDynamicEntity(IPropertyLookup source)
    {
        if (source == null) return null;
        if (source is IDynamicEntity dynEnt) return dynEnt;
        if (source is ICanBeEntity canBe) return SubDataFactory.SubDynEntityOrNull(canBe.Entity);
        //if (source is IEntity ent) return SubDataFactory.SubDynEntityOrNull(ent);
        return null;
    }


    [PrivateApi("not implemented")]
    public override bool TrySetMember(SetMemberBinder binder, object value)
        => throw new NotSupportedException($"Setting a value on {nameof(IDynamicStack)} is not supported");

    #region Get / Get<T>

    public dynamic Get(string name) => GetHelper.Get(name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public dynamic Get(string name, NoParamOrder noParamOrder = default, string language = null, bool convertLinks = true, bool? debug = null)
        => GetHelper.Get(name, noParamOrder, language, convertLinks, debug);

    public TValue Get<TValue>(string name)
        => GetHelper.Get<TValue>(name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default)
        => GetHelper.Get(name, noParamOrder, fallback);

    #endregion

    #region IEnumerable<IDynamicEntity>

    private List<IDynamicEntity> List => _list ??= _stack.Sources
        .Select(src => SourceToDynamicEntity(src.Value))
        .Where(e => e != null)
        .ToList();

    private List<IDynamicEntity> _list;

    public IEnumerator<IDynamicEntity> GetEnumerator() => List.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Any*** properties just for documentation

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public dynamic AnyProperty => null;

    #endregion

}