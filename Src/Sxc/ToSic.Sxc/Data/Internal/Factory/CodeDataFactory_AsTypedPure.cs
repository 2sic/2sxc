using System.Collections;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{
    private const string NameOfAsTyped = nameof(IDynamicCode16.AsTyped) + "(...)";

    public ITyped AsTyped(object data, bool required = false, bool? propsRequired = default, string detailsMessage = default)
    {
        var l = Log.Fn<ITyped>();

        if (AsTypedPreflightReturnNull(data, NameOfAsTyped, required, detailsMessage))
            return l.ReturnNull();

        if (data is ITyped alreadyTyped)
            return l.Return(alreadyTyped, "already typed");

        var result = codeDataWrapper.Value.ChildNonJsonWrapIfPossible(data: data, wrapNonAnon: true,
            WrapperSettings.Typed(children: true, realObjectsToo: false, propsRequired: propsRequired ?? true));
        if (result is ITyped resTyped)
            return l.Return(resTyped, "converted to dyn-read");

        throw l.Done(new ArgumentException($"Can't wrap/convert the original '{data.GetType()}'"));
    }

    private const string NameOfAsTypedList = nameof(IDynamicCode16.AsTypedList) + "(...)";
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder, bool? required = false, bool? propsRequired = default)
    {
        var l = Log.Fn<IEnumerable<ITyped>>();

        if (AsTypedPreflightReturnNull(list, NameOfAsTypedList, required == true))
            return l.ReturnNull();

        if (list is IEnumerable<ITyped> alreadyTyped)
            return l.Return(alreadyTyped, "already typed");

        if (list is not IEnumerable enumerable)
            throw new ArgumentException($"The object provided to {NameOfAsTypedList} is not enumerable/array so it can't be converted.", nameof(list));

        var itemsRequired = required != false;
        var result = enumerable
            .Cast<object>()
            .Select((o, i) => AsTyped(o, itemsRequired, propsRequired, $"index: {i}"))
            .ToList();

        return result;
    }

    private bool AsTypedPreflightReturnNull(object original, string methodName, bool required, string detailsMessage = default)
    {
        var l = Log.Fn<bool>();
        switch (original)
        {
            case null:
                return required 
                    ? throw l.Done(new ArgumentException($"Tried to convert using {methodName} but got null, with {nameof(required)} = {required}. {detailsMessage}"))
                    : l.ReturnTrue();
            case string _:
                throw l.Done(new ArgumentException($"Tried to convert using {methodName} but got a string." +
                                                   $"If you want to convert a string JSON to an easier-to-use object, use Kit.Json.ToTyped(...) instead. {detailsMessage}"));
            case ICanBeEntity _:
                throw l.Done(new ArgumentException(
                    $"Tried to convert using {methodName} but got an entity-like object." +
                    $"If you want to convert an Entity-Like object, use AsItem(...) instead. {detailsMessage}"));
            default:
                // Check value types - note that it won't catch strings, but these were handled above
                if (original.GetType().IsValueType)
                    throw l.Done(new ArgumentException(
                        $"Tried to convert using {methodName} but got value type: '{original.GetType()}'. This can't be converted. {detailsMessage}"));

                return l.ReturnFalse();
        }
    }

}