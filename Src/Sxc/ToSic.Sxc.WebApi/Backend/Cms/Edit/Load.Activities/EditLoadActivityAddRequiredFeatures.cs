using ToSic.Eav.Data.Processing;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityAddRequiredFeatures(IUiContextBuilder contextBuilder): ServiceBase("UoW.AddCtx", connect: [contextBuilder]),
    IWork<EditLoadDto, EditLoadDto>
{
    public async Task<Package<EditLoadDto>> Handle(WorkContext actionCtx, Package<EditLoadDto> package)
    {
        var l = Log.Fn<Package<EditLoadDto>>();

        // Determine required features for the UI WIP 18.02
        var inheritedFields = actionCtx.Get<List<IContentType>>(EditLoadContextConstants.UsedTypes)
            .SelectMany(t => t.Attributes
                .Where(a => a.SysSettings?.InheritMetadata == true)
                .Select(a => new { a.Name, Type = t }))
            .ToList();

        if (!inheritedFields.Any())
            return l.Return(package, "none found");

        package = package with
        {
            Data = package.Data with
            {
                RequiredFeatures = new()
                {
                    {
                        BuiltInFeatures.ContentTypeFieldsReuseDefinitions.NameId,
                        inheritedFields.Select(f => $"Used in fields: {f.Type.Name}.{f.Name}").ToArray()
                    },
                }
            },
        };

        return l.Return(package, "added some req features");
    }
}
