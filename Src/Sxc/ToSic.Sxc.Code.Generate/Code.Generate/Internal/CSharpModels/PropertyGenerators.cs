namespace ToSic.Sxc.Code.Generate.Internal;

internal class GenDataProperties
{
    internal static List<GeneratePropertyBase> Generators(CSharpGeneratorHelper helper)
    {
        return
        [
            new GeneratePropertyBool(helper),
            new GeneratePropertyString(helper),
            new GeneratePropertyEmpty(helper),
            new GeneratePropertyHyperlink(helper),
            new GeneratePropertyNumber(helper),
            new GeneratePropertyDateTime(helper),
            new GeneratePropertyCustom(helper),
            new GeneratePropertyEntity(helper),
        ];
    }
}