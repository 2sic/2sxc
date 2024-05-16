using System.Dynamic;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Data.Internal.Dynamic;
using ToSic.Sxc.Data.Internal.Typed;

namespace ToSic.Sxc.Data.Internal.Wrapper;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeDataWrapper(
    LazySvc<CodeDataFactory> cdf,
    Generator<WrapObjectTyped> wrapTypeGenerator,
    Generator<WrapObjectTypedItem> wrapItemGenerator)
    : ServiceBase("Sxc.DWrpFk", connect: [wrapTypeGenerator, wrapItemGenerator, cdf])
{
    internal WrapDictionaryDynamic<TKey, TValue> FromDictionary<TKey, TValue>(IDictionary<TKey, TValue> original) =>
        new(original, this);

    public WrapObjectDynamic FromObject(object data, WrapperSettings settings)
    {
        var preWrap = new PreWrapObject(data, settings, this);
        return new(preWrap, this);
    }

    public ITyped TypedFromObject(object data, WrapperSettings settings)
    {
        var preWrap = new PreWrapObject(data, settings, this);
        return wrapTypeGenerator.New().Setup(preWrap);
    }
    public ITypedItem TypedItemFromObject(object data, WrapperSettings settings, ILazyLike<CodeDataFactory> cdf1 = default)
    {
        var preWrap = new PreWrapObject(data, settings, this);
        return wrapItemGenerator.New().Setup(cdf1 ?? cdf, this, preWrap);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="wrapNonAnon">if true and the contents isn't already a dynamic object, it will also wrap real objects; otherwise only anonymous</param>
    /// <param name="settings"></param>
    /// <returns></returns>
    [PrivateApi]
    internal object ChildNonJsonWrapIfPossible(object data, bool wrapNonAnon, WrapperSettings settings)
    {
        // If null or simple value, use that
        if (data is null) return null;

        if (data is string || data.GetType().IsValueType) return data;

        // Guids & DateTimes are objects, but very simple, and should be returned for normal use
        if (data is Guid || data is DateTime) return data;

        // 2023-08-18 2dm DISABLED - I'm pretty sure that json should never be a source of this
        // as I believe it's only used in non-json wrappers
        // Check if the result is a JSON object which should support navigation again
        //var result = new CodeJsonWrapper(this, settings).IfJsonGetValueOrJacket(data);
        var result = data;

        // Check if the original or result already supports navigation... - which is the case if it's a DynamicJacket now
        switch (result)
        {
            case IPropertyLookup _:
            case ISxcDynamicObject _:
            case DynamicObject _:
                return result;
        }

        // if (result is IDictionary dicResult) return DynamicReadDictionary(dicResult);

        // Simple arrays don't benefit from re-wrapping. 
        // If the calling code needs to do something with them, it should iterate it and then rewrap with AsDynamic
        if (result.GetType().IsArray) return result;

        // Otherwise it's a complex object, which should be re-wrapped for navigation
        var wrap = wrapNonAnon || data.IsAnonymous();
        return wrap
            ? settings.WrapToDynamic 
                ? FromObject(data, settings) as object
                : TypedFromObject(data, settings)
            : data;
    }
}