using System.Collections;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Edit.Toolbar;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class FilterValueProcessor : UrlValueProcess
{

    public override NameObjectSet Process(NameObjectSet set)
    {
        // Basic cases where we don't change anything
        if (set?.Value == null || set.Value is string || set.Value.IsNumeric()) return set;

        // If the value is an entity / dynamic-entity, return it as an id
        if (set.Value is ICanBeEntity entity)
            return new(set, value: entity.Entity.EntityId);

        // Check array / list of items to filter for
        // Make sure that if they have IDs or Entity-like objects they will be reduced to their ID
        if (set.Value is IEnumerable enumerable)
        {
            var ids = enumerable.Cast<object>().Select(o =>
                {
                    if (o is string str) return str;
                    if (o.IsNumeric()) return o.ToString();
                    if (o is ICanBeEntity oEnt) return oEnt.Entity.EntityId.ToString();
                    return null;
                })
                .Where(v => v != null)
                .ToArray();
            return new(set, value: ids);
        }

        // Fallback
        return set;
    }
        
}