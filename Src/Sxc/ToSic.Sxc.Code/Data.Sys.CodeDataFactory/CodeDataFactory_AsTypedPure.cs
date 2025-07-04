﻿using System.Collections;
using ToSic.Sxc.Data.Sys.Wrappers;

namespace ToSic.Sxc.Data.Sys.CodeDataFactory;

partial class CodeDataFactory
{
    private const string NameOfAsTyped = /*nameof(IDynamicCode16.AsTyped)*/ "AsTyped" + "(...)";

    public ITyped? AsTyped(object data, Factory.ConvertItemSettings settings, string? detailsMessage = default)
    {
        var l = Log.Fn<ITyped>();

        if (AsTypedPreflightReturnNull(data, NameOfAsTyped, settings.FirstIsRequired, detailsMessage))
            return l.ReturnNull();

        if (data is ITyped alreadyTyped)
            return l.Return(alreadyTyped, "already typed");

        var result = codeDataWrapper.Value.ChildNonJsonWrapIfPossible(data: data, wrapNonAnon: true,
            WrapperSettings.Typed(children: true, realObjectsToo: false, propsRequired: settings.ItemIsStrict));
        if (result is ITyped resTyped)
            return l.Return(resTyped, "converted to dyn-read");

        throw l.Done(new ArgumentException($"Can't wrap/convert the original '{data.GetType()}'"));
    }

    private const string NameOfAsTypedList = /*nameof(IDynamicCode16.AsTypedList)*/ "AsTypedList" + "(...)";
    public IEnumerable<ITyped>? AsTypedList(object list, Factory.ConvertItemSettings settings)
    {
        var l = Log.Fn<IEnumerable<ITyped>>();

        if (AsTypedPreflightReturnNull(list, NameOfAsTypedList, settings.FirstIsRequired))
            return l.ReturnNull();

        if (list is IEnumerable<ITyped> alreadyTyped)
            return l.Return(alreadyTyped, "already typed");

        if (list is not IEnumerable enumerable)
            throw new ArgumentException($"The object provided to {NameOfAsTypedList} is not enumerable/array so it can't be converted.", nameof(list));

        //var subSettings = new ConvertItemSettings { FirstIsRequired = required, ItemIsStrict = propsRequired ?? true };
        var result = enumerable
            .Cast<object>()
            .Select((o, i) => AsTyped(o, settings, $"index: {i}")!)
            .Where(o => o != null) // filter out nulls, as this is the default behavior
            .ToList();

        return result;
    }

    private bool AsTypedPreflightReturnNull(object original, string methodName, bool required, string? detailsMessage = default)
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