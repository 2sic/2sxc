using System.Text;

namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Empty properties don't result in any code
/// </summary>
internal class GeneratePropertyEmpty: GeneratePropertyBase
{
    public override string ForDataType => "Empty";

    public override GeneratedCode Generate(GenerateCodeHelper genHelper, IContentTypeAttribute attribute, int indent)
    {
        return new(new());
    }
}