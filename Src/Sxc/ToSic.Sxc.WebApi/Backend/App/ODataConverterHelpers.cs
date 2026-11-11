using ToSic.Eav.DataFormats.EavLight;
using ToSic.Sxc.Data.Sys.Convert;
using ToSic.Sys.OData;

namespace ToSic.Sxc.Backend.App;

public static class ODataConverterHelpers
{
    internal static IConvertToEavLight PrepareDataConverter(this Generator<IConvertToEavLight> dataConverter, ODataOptions options, bool withGuid, bool isEditor)
    {
        var dc = dataConverter.New();
        dc.WithGuid = withGuid;
        if (dc is ConvertToEavLightWithCmsInfo serializerWithEdit)
            serializerWithEdit.WithEdit = isEditor;
        dc.AddSelectFields(options.Select.ToListOpt());
        return dc;
    }

}
