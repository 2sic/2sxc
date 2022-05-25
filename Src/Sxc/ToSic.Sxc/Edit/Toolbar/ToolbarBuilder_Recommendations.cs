using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private List<string> GetMetadataTypeNames(object target, string contentTypes)
        {
            var types = contentTypes?.Split(',').Select(s => s.Trim()).ToArray() ?? Array.Empty<string>();
            if (!types.Any())
                types = TryToFindRecommendations(target);

            var finalTypes = new List<string>();
            foreach (var type in types)
                if (type == "*") finalTypes.AddRange(TryToFindRecommendations(target));
                else finalTypes.Add(type);
            return finalTypes;
        }

        private string[] TryToFindRecommendations(object target)
        {
            var l = Log.Fn<string[]>();
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (target == null) 
                return l.Return(Array.Empty<string>(), "null");

            if (target is IHasMetadata withMetadata)
                target = withMetadata.Metadata;

            if (!(target is IMetadataOf mdOf))
                return l.Return(Array.Empty<string>(), "not metadata");

            var recommendations = mdOf?.Target?.Recommendations ?? Array.Empty<string>();
            
            return l.ReturnAsOk(recommendations);
        }
    }
}
