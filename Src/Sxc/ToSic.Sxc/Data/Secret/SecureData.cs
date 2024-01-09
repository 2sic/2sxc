using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data;

[PrivateApi("hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class SecureData<T>(T result, bool isSecure) : ISecureData<T>
{
    public T Value { get; internal set; } = result;

    //public bool IsEncrypted { get; internal set; } = false;
    //public bool IsSigned { get; internal set; } = false;
    //public SecureDataAuthorities Authority { get; internal set; } = SecureDataAuthorities.None;
    public bool IsSecured { get; internal set; } = isSecure;


    public bool IsSecuredBy(string authorityName) 
        => IsSecured && "preset".EqualsInsensitive(authorityName);

    public override string ToString() => Value.ToString();
}