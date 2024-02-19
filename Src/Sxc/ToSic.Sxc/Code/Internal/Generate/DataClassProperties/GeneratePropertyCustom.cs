using Custom.Data;
using ToSic.Sxc.Cms.Data;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyCustom: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Custom;

    public override List<CodeFragment> Generate(CodeGenSpecs specs, IContentTypeAttribute attribute, int tabs)
    {
        if (attribute.InputType() != "custom-gps")
            return [];

        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "GpsCoordinates", name, "Gps", usings: UsingCustomData, summary:
            [
                $"{name} as GPS Coordinates object with {nameof(GpsCoordinates.Latitude)} and {nameof(GpsCoordinates.Longitude)}.",
            ]),
        ];
    }

    private List<string> UsingCustomData { get; } = [typeof(GpsCoordinates).Namespace];
}