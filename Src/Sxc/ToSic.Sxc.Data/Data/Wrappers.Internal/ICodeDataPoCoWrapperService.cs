using ToSic.Lib.DI;

namespace ToSic.Sxc.Data.Internal.Wrapper;

/// <summary>
/// Service to create dynamic or typed objects from non Entity objects.
/// </summary>
public interface ICodeDataPoCoWrapperService
{
    /// <summary>
    /// Note: returns a WrapObjectDynamic, but not typed to make it easier to move objects in code.
    /// It's mainly used in production code as `dynamic` so the internal type doesn't matter.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    /*WrapObjectDynamic*/
    object DynamicFromObject(object data, WrapperSettings settings);
    ITyped TypedFromObject(object data, WrapperSettings settings);
    ITypedItem TypedItemFromObject(object data, WrapperSettings settings, ILazyLike<ICodeDataFactory> cdf1 = default);

    public /*DynamicFromDictionary<TKey, TValue>*/object FromDictionary<TKey, TValue>(IDictionary<TKey, TValue> original);

    internal object ChildNonJsonWrapIfPossible(object data, bool wrapNonAnon, WrapperSettings settings);
}