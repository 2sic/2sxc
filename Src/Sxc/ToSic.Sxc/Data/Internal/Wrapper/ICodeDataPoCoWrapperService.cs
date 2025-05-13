using ToSic.Lib.DI;
using ToSic.Sxc.Data.Internal.Dynamic;

namespace ToSic.Sxc.Data.Internal.Wrapper;

/// <summary>
/// Service to create dynamic or typed objects from non Entity objects.
/// </summary>
public interface ICodeDataPoCoWrapperService
{
    WrapObjectDynamic DynamicFromObject(object data, WrapperSettings settings);
    ITyped TypedFromObject(object data, WrapperSettings settings);
    ITypedItem TypedItemFromObject(object data, WrapperSettings settings, ILazyLike<ICodeDataFactory> cdf1 = default);

    internal DynamicFromDictionary<TKey, TValue> FromDictionary<TKey, TValue>(IDictionary<TKey, TValue> original);

    internal object ChildNonJsonWrapIfPossible(object data, bool wrapNonAnon, WrapperSettings settings);
}