using ToSic.Eav.DataFormats.EavLight;
using ToSic.Sys.OData;

namespace ToSic.Sxc.Backend.App;

public static class ODataConverterHelpers
{
    internal static IConvertToEavLight PrepareDataConverter(this Generator<IConvertToEavLight> dataConverter, ODataOptions options, bool withGuid, bool isEditor)
    {
        var dc = dataConverter.New();
        dc.WithGuid = withGuid;
        // 2026-11-11 2dm - dropped this, believe it's not used anymore; keep for reference till ca. 2027-07 #DropWithEdit
        //if (dc is ConvertToEavLightWithCmsInfo serializerWithEdit)
        //    serializerWithEdit.WithEdit = isEditor;
        dc.AddSelectFields(options.Select.ToListOpt());
        return dc;
    }

}
