using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.WebApi.Sys.Entities;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityAddNecessaryInputTypes(GenWorkPlus<WorkInputTypes> inputTypes) : ServiceBase("UoW.InpTyp")
{
    public async Task<ActionData<EditLoadDto>> Run(LowCodeActionContext mainCtx, ActionData<EditLoadDto> result) =>
        result with
        {
            Data = result.Data with
            {
                InputTypes = GetNecessaryInputTypes(result.Data.ContentTypes, mainCtx.Get<IAppWorkCtxPlus>(EditLoadContextConstants.AppCtxWork)),
            },
        };

    private List<InputTypeInfo> GetNecessaryInputTypes(List<JsonContentType> contentTypes, IAppWorkCtxPlus appCtx)
    {
        var l = Log.Fn<List<InputTypeInfo>>($"{nameof(contentTypes)}: {contentTypes.Count}");
        var fields = contentTypes
            .SelectMany(t => t.AttributesSafe())
            .Select(a => a.InputType)
            .Distinct()
            .ToList();

        l.A("Found these input types to load: " + string.Join(", ", fields));

        var allInputType = inputTypes.New(appCtx).GetInputTypes();

        var found = allInputType
            .Where(it => fields.Contains(it.Type))
            .ToList();

        if (found.Count == fields.Count)
            l.A("Found all");
        else
        {
            l.A($"It seems some input types were not found. Needed {fields.Count}, found {found.Count}. Will try to log details for this.");
            try
            {
                var notFound = fields.Where(field => found.All(fnd => fnd.Type != field));
                l.A("Didn't find: " + string.Join(",", notFound));
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                l.A("Ran into problems logging missing input types.");
            }
        }

        return l.Return(found, $"{found.Count}");
    }
}
