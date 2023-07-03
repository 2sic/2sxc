using System;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data.AsConverter
{
    public partial class AsConverterService
    {

        public ITypedRead AsTypedPure(object original)
        {
            var l = Log.Fn<ITypedRead>();
            switch (original)
            {
                case null:
                    return l.ReturnNull();
                case string _:
                    throw l.Done(new ArgumentException("Tried to convert using AsTyped(...) but got a string." +
                                                       "If you want to convert a JSON to a readable object, use Kit.Json.ToTyped(...) instead"));
                case ICanBeEntity _:
                    throw l.Done(new ArgumentException("Tried to convert using AsTyped(...) but got an entity-like object." +
                                                       "If you want to convert an Entity-Like object, use AsItem(...) instead"));

                // This must come after ICanBeEntity
                case ITypedRead alreadyTyped:
                    return l.Return(alreadyTyped, "already typed");

                default:
                    // Check value types - note that it won't catch strings, but these were handled above
                    if (original.GetType().IsValueType)
                        throw l.Done(new ArgumentException(
                            $"Tried to convert using AsTyped but got value type: '{original.GetType()}'."));

                    var result = DynamicHelpers.WrapIfPossible(original, true, true, false);
                    if (result is ITypedRead resTyped) return l.Return(resTyped, "converted to dyn-read");

                    throw l.Done(new ArgumentException($"Can't wrap/convert the original '{original.GetType()}'"));
            }
        }

    }
}
