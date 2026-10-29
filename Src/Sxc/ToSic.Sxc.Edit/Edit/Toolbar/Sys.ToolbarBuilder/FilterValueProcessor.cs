using System.Collections;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class FilterValueProcessor : IUrlParameterInGroupOperation
{

    public UrlParameter? Process(UrlParameter? set)
    {
        // Basic cases where we don't change anything
        if (set?.Value == null || set.Value is string || set.Value.IsNumeric())
            return set;

        // If the value is an entity / dynamic-entity, return it as an id
        if (set.Value is ICanBeEntity entity)
            return set with { Value = entity.Entity.EntityId };

        // Check array / list of items to filter for
        // Make sure that if they have IDs or Entity-like objects they will be reduced to their ID
        if (set.Value is not IEnumerable enumerable)
            return set; // Fallback

        var ids = enumerable
            .Cast<object>()
            .Select(o =>
            {
                if (o is string str) return str;
                if (o.IsNumeric()) return o.ToString();
                if (o is ICanBeEntity oEnt) return oEnt.Entity.EntityId.ToString();
                return null;
            })
            .OfType<string>()
            .ToArray();
        return set with { Value = ids };
    }
}