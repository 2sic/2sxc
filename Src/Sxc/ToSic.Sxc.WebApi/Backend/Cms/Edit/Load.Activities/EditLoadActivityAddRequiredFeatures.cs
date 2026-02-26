using ToSic.Eav.Data.Processing;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityAddRequiredFeatures(IUiContextBuilder contextBuilder): ServiceBase("UoW.AddCtx", connect: [contextBuilder]),
    ILowCodeAction<EditLoadDto, EditLoadDto>
{
    public async Task<ActionData<EditLoadDto>> Run(LowCodeActionContext actionCtx, ActionData<EditLoadDto> result)
    {
        var l = Log.Fn<ActionData<EditLoadDto>>();

        // Determine required features for the UI WIP 18.02
        var inheritedFields = actionCtx.Get<List<IContentType>>(EditLoadContextConstants.UsedTypes)
            .SelectMany(t => t.Attributes
                .Where(a => a.SysSettings?.InheritMetadata == true)
                .Select(a => new { a.Name, Type = t }))
            .ToList();

        if (!inheritedFields.Any())
            return l.Return(result, "none found");

        result = result with
        {
            Data = result.Data with
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

        return l.Return(result, "added some req features");
    }
}
