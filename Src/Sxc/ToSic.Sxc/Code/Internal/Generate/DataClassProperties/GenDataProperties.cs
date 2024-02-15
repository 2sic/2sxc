namespace ToSic.Sxc.Code.Internal.Generate;

internal class GenDataProperties
{
    internal static List<GeneratePropertyBase> Generators =
    [
        new GeneratePropertyBool(),
        new GeneratePropertyString(),
        new GeneratePropertyEmpty(),
        new GeneratePropertyHyperlink(),
        new GeneratePropertyNumber(),
        new GeneratePropertyDateTime(),
        new GeneratePropertyCustom(),
        new GeneratePropertyEntity(),
    ];
}