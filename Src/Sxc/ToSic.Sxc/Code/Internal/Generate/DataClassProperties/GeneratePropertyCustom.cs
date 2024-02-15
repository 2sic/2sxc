using Custom.Data;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyCustom: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Custom;

    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs)
    {
        if (attribute.InputType() != "@custom-gps")
            return [];

        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "GpsCoordinates", name, "ToGps", summary:
            [
                $"{name} as GPS Coordinates object with {nameof(Item16.GpsCoordinates.Latitude)} and {nameof(Item16.GpsCoordinates.Longitude)}.",
            ]),
        ];
    }
}