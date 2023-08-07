using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data.Typed
{
    [PrivateApi]
    [JsonConverter(typeof(DynamicJsonConverter))]
    internal interface IGet4Typed: IHasKeys, IHasJsonSource
    {
        (bool Found, object Result) Get(string name);

        TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, [CallerMemberName] string cName = default);

    }
}
