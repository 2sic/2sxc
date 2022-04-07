using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.Sxc.Images
{
    internal class RecipeHelpers
    {
        internal static IDictionary<string, string> ToStringDicOrNull(IDictionary<string, object> origOrNull)
            => origOrNull?.ToDictionary(p => p.Key, p => p.Value?.ToString());

        internal static ReadOnlyDictionary<string, string> MergeDics(ReadOnlyDictionary<string, string> parentOrNull, IDictionary<string, string> myOrNull)
        {
            if (myOrNull == null) return parentOrNull ?? new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
            if (parentOrNull == null || !parentOrNull.Any())
                return new ReadOnlyDictionary<string, string>(myOrNull);

            var newMaster = new Dictionary<string, string>(parentOrNull);

            foreach (var pair in myOrNull)
                if (pair.Value == null) newMaster.Remove(pair.Key);
                else newMaster[pair.Key] = pair.Value;

            return new ReadOnlyDictionary<string, string>(newMaster);
        }

    }
}
