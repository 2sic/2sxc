using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data;

[PrivateApi("hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SecureData<T>: ISecureData<T>
{
    public SecureData(T result, bool isSecure)
    {
        Value = result;
        IsSecure = isSecure;
    }

    public T Value { get; internal set; }

    public bool IsEncrypted { get; internal set; } = false;
    public bool IsSigned { get; internal set; } = false;
    public SecretAuthorities Authority { get; internal set; } = SecretAuthorities.None;
    public bool IsSecure { get; internal set; }


    public bool IsSecuredBy(string authorityName) 
        => IsSecure && "preset".EqualsInsensitive(authorityName);

    public override string ToString() => Value.ToString();
}