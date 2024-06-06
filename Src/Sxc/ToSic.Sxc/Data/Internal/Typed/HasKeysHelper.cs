using System.Collections;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data.Internal.Typed;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HasKeysHelper
{
    public static bool IsEmpty(ITyped item, string name, NoParamOrder noParamOrder, bool? blankIs)
    {
        var value = item.Get(name, noParamOrder, required: false);
        return IsEmpty(value, blankIs);
    }

    public static bool IsNotEmpty(ITyped item, string name, NoParamOrder noParamOrder, bool? blankIs)
    {
        var value = item.Get(name, noParamOrder, required: false);
        return IsNotEmpty(value, blankIs);
    }

    public static bool IsEmpty(object value, bool? blankIsEmpty)
    {
        // Since we'll reverse the final result, we must ensure that blankIs is pre-reversed as well
        blankIsEmpty = !(blankIsEmpty ?? true);
        return !IsNotEmpty(value, blankIsEmpty);
    }

    public static bool IsNotEmpty(object value, bool? blankIsEmpty) =>
        value switch
        {
            null => false,
            // It's a non-null string, let's check other things
            // null or true means blank strings (inkl. whitespace etc.) are seen as empty
            // So Text.Has returns true if it has non-blank content
            // old: if (blankIs is null || blankIs == false) return Text.Has(strVal);
            string strVal when blankIsEmpty != true => Text.Has(strVal),
            // blankIs == true, so even blank strings return true
            string => true,
            IEnumerable typedList => typedList.Cast<object>().Any(),
            _ => true
        };
}