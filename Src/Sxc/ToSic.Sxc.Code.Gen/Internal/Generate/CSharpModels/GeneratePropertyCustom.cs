using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Sxc.Cms.Data;

namespace ToSic.Sxc.Code.Gen.Internal.Generate;

internal class GeneratePropertyCustom(CSharpGeneratorHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.Custom;

    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs)
    {
        if (attribute.InputType() != "custom-gps")
            return [];

        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "GpsCoordinates", name, $"{Specs.ItemAccessor}.Gps", usings: UsingCustomData, summary:
            [
                $"{name} as GPS Coordinates object with {nameof(GpsCoordinates.Latitude)} and {nameof(GpsCoordinates.Longitude)}.",
            ]),
        ];
    }

    private List<string> UsingCustomData { get; } = [typeof(GpsCoordinates).Namespace];
}