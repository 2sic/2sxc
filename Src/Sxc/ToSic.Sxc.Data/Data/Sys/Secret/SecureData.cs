﻿namespace ToSic.Sxc.Data.Sys.Secret;

[PrivateApi("hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class SecureData<T>(T result, bool isSecure) : ISecureData<T>
{
    public T Value { get; internal set; } = result;

    //public bool IsEncrypted { get; internal set; } = false;
    //public bool IsSigned { get; internal set; } = false;
    //public SecureDataAuthorities Authority { get; internal set; } = SecureDataAuthorities.None;
    public bool IsSecured { get; internal set; } = isSecure;


    public bool IsSecuredBy(string authorityName) 
        => IsSecured && "preset".EqualsInsensitive(authorityName);

    public override string? ToString() => Value?.ToString();
}