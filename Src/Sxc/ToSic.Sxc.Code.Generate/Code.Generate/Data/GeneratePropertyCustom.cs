using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyCustom(CSharpGeneratorHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.Custom;

    public override List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs)
    {
        if (fieldDef.InputType != "custom-gps")
            return [];

        var name = fieldDef.Name;

        return
        [
            GenPropSnip(tabs, "GpsCoordinates", name, $"{Specs.ItemAccessor}.Gps", usings: UsingCustomData, summary:
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