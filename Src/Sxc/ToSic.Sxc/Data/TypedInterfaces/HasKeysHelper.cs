using System.Collections;
using System.Linq;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Data
{
    public class HasKeysHelper
    {
        public static bool ContainsData(ITyped item, string name, string noParamOrder, bool? blankIs)
        {
            var value = item.Get(name, required: false);
            return HasKeysHelper.ContainsData(value, blankIs);
        }

        public static bool ContainsData(object value, bool? blankIs)
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
