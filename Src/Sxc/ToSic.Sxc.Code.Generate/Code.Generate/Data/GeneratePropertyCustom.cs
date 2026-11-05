using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyCustom(CSharpGeneratorHelper helper) : ICodeGenProperty
{
    public ValueTypes ForDataType => ValueTypes.Custom;

    public List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs)
    {
        if (fieldDef.InputType != "custom-gps")
            return [];

        var name = fieldDef.Name;

        return
        [
            helper.GenPropSnip(tabs, "GpsCoordinates", name, $"{helper.Specs.ItemAccessor}.Gps", usings: UsingCustomData, summary:
            [
                $"{name} as GPS Coordinates object with {nameof(GpsCoordinates.Latitude)} and {nameof(GpsCoordinates.Longitude)}.",
            ]),
        ];
    }

    private List<string> UsingCustomData { get; } =
    [
        typeof(GpsCoordinates).Namespace!,
    ];
}