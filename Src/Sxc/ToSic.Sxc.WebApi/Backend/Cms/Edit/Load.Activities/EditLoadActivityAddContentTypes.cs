using ToSic.Eav.ImportExport.Json.Sys;
using ToSic.Eav.Serialization.Sys;
using ToSic.Sxc.Data.Sys;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityAddContentTypes(Generator<JsonSerializer> jsonSerializerGenerator)
{
    public record ActionContext(List<IContentType> UsedTypes);

    public EditLoadDto Run(EditLoadDto result, EditLoadActivityContext mainCtx, ActionContext actionCtx)
    {
        var serSettings = new JsonSerializationSettings
        {
            CtIncludeInherited = true,
            CtAttributeIncludeInheritedMetadata = true
        };

        var serializerForTypes = jsonSerializerGenerator.New().SetApp(mainCtx.AppReader);
        serializerForTypes.ValueConvertHyperlinks = true;

        var jsonTypes = actionCtx.UsedTypes
            .Select(t => serializerForTypes.ToPackage(t, serSettings))
            .ToListOpt();

        // Fix not-supported input-type names; map to correct name
        jsonTypes = jsonTypes
            .Select(jt =>
            {
                jt = jt with
                {
                    ContentType = jt.ContentType == null
                        ? null
                        : jt.ContentType with
                        {
                            Attributes = (jt.ContentType.Attributes ?? [])
                            .Select(a => a with
                            {
                                // ensure that the input-type is set, otherwise it will be null
                                InputType = InputTypes.MapInputTypeV10(a.InputType! /* it can't really be null, only in very old imports, and this is not an import */)
                            })
                            .ToListOpt(),
                        }
                };
                return jt;
            })
            .ToListOpt();

        result = result with
        {
            ContentTypes = jsonTypes
                .Select(t => t.ContentType!)
                .ToList(),

            // Also add global Entities like Formulas which would not be included otherwise
            ContentTypeItems = jsonTypes
                .SelectMany(t => t.Entities!)
                .ToList(),
        };

        return result;
    }
}