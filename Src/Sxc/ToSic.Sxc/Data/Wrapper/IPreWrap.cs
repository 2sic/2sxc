using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data.Wrapper
{
    [PrivateApi]
    [JsonConverter(typeof(DynamicJsonConverter))]
    internal interface IPreWrap : IHasKeys, IHasJsonSource
    {
        TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, [CallerMemberName] string cName = default);

    }
}
