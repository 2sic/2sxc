using System.Collections;
using System.Linq;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    public class HasKeysHelper
    {
        public static bool IsEmpty(ITyped item, string name, string noParamOrder, bool? blankIs)
        {
            var value = item.Get(name, noParamOrder, required: false);
            return IsEmpty(value, blankIs);
        }

        public static bool IsNotEmpty(ITyped item, string name, string noParamOrder, bool? blankIs)
        {
            var value = item.Get(name, noParamOrder, required: false);
            return IsNotEmpty(value, blankIs);
        }

        public static bool IsEmpty(object value, bool? blankIs)
        {
            // Since we'll reverse the final result, we must ensure that blankIs is pre-reversed as well
            blankIs = !(blankIs ?? true);
            return !IsNotEmpty(value, blankIs);
        }

        public static bool IsNotEmpty(object value, bool? blankIs)
        {
            if (value == null) return false;

            if (value is string strVal)
            {
                // It's a non-null string, let's check other things
                // null or true means blank strings (inkl. whitespace etc.) are seen as empty
                // So Text.Has returns true if it has non-blank content
                if (blankIs is null || blankIs == false) return Text.Has(strVal);
                // blankIs == true, so even blank strings return true
                return true;

            }
            if (value is IEnumerable typedList)
                return typedList.Cast<object>().Any();

            return true;
        }

    }
}
