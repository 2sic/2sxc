﻿using System.Collections;
using System.Dynamic;
using ToSic.Eav.Data.PropertyStack.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Lib.Wrappers;
using ToSic.Sxc.Data.Sys.Dynamic;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Stack;

namespace ToSic.Sxc.Data.Sys.DynamicStack;

// Must be pbulic so that `Resources.Get...` work
[PrivateApi("Keep implementation hidden, only publish interface")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DynamicStack: DynamicObject,
    IWrapper<IPropertyStack>,
    IDynamicStack,
    IHasPropLookup,
    ISxcDynamicObject,
    IEnumerable, // note: not sure why it supports this, but it has been this way for a long time
    ICanDebug
{
    #region Constructor and Helpers (Composition)

    public DynamicStack(string name, ICodeDataFactory cdf, IReadOnlyCollection<KeyValuePair<string, IPropertyLookup>> sources)
    {
        Cdf = cdf;
        var stack = new PropertyStack().Init(name, sources);
        _stack = stack;
        PropertyLookup = new PropLookupStack(stack, () => Debug);
    }
    // ReSharper disable once InconsistentNaming
    [PrivateApi] public ICodeDataFactory Cdf { get; }
    private readonly IPropertyStack _stack;
    private const bool Strict = false;

    [PrivateApi]
    public IPropertyLookup PropertyLookup { get; }

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal GetAndConvertHelper GetHelper => field ??= new(this, Cdf, Strict, childrenShouldBeDynamic: true, canDebug: this);

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal SubDataFactory SubDataFactory => field ??= new(Cdf, Strict, canDebug: this);

    /// <inheritdoc />
    public bool Debug { get; set; }

    #endregion



    public override bool TryGetMember(GetMemberBinder binder, out object? result) 
        => CodeDynHelper.TryGetMemberAndRespectStrict(GetHelper, binder, out result);


    /// <inheritdoc />
    [PrivateApi]
    public IPropertyStack GetContents() => _stack;

    /// <inheritdoc />
    [PrivateApi("was public till v16.02, but since I'm not sure if it is really used, we decided to hide it again since it's probably not an important API")]
    public dynamic? GetSource(string name)
    {
        var source = _stack.GetSource(name)
                     // If not found, create a fake one
                     ?? Cdf.FakeEntity(((ICodeDataFactoryDeepWip)Cdf).AppIdOrZero);

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

    private IDynamicEntity? SourceToDynamicEntity(IPropertyLookup? source)
    {
        if (source == null)
            return null;
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (source is IDynamicEntity dynEnt)
            return dynEnt;
        if (source is ICanBeEntity canBe)
            return SubDataFactory.SubDynEntityOrNull(canBe.Entity);
        //if (source is IEntity ent) return SubDataFactory.SubDynEntityOrNull(ent);
        return null;
    }


    [PrivateApi("not implemented")]
    public override bool TrySetMember(SetMemberBinder binder, object? value)
        => throw new NotSupportedException($"Setting a value on {nameof(IDynamicStack)} is not supported");

    #region Get / Get<T>

    public dynamic? Get(string name) => GetHelper.Get(name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public dynamic? Get(string name, NoParamOrder noParamOrder = default, string? language = null, bool convertLinks = true, bool? debug = null)
        => GetHelper.Get(name, noParamOrder, language, convertLinks, debug);

    public TValue? Get<TValue>(string name)
        => GetHelper.Get<TValue>(name);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TValue? Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue? fallback = default)
        => GetHelper.Get(name, noParamOrder, fallback);

    #endregion

    #region IEnumerable<IDynamicEntity>

    [field: AllowNull, MaybeNull]
    private List<IDynamicEntity> List => field ??= _stack.Sources
        .Select(src => SourceToDynamicEntity(src.Value)!)
        .Where(e => e != null!)
        .ToList();

    public IEnumerator<IDynamicEntity> GetEnumerator() => List.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Any*** properties just for documentation

    [ShowApiWhenReleased(ShowApiMode.Never)]
    public dynamic AnyProperty => null!;

    #endregion

}