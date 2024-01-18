namespace ToSic.Sxc.Data;

[Flags]
[PrivateApi]
[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public enum SecureDataAuthorities
{
    /// <summary>
    /// No authority / not a secret or wasn't able to decipher
    /// </summary>
    None = 0,

    /// <summary>
    /// Root authority, ATM not in use yet
    /// </summary>
    Root = 1,

    /// <summary>
    /// System authority (similar to root), ATM not in use yet
    /// </summary>
    System = 2,

    /// <summary>
    /// Platform authority, meaning the secret was decrypted or authorized by the platform like Dnn or Oqtane
    /// </summary>
    Platform = 4,

    [PrivateApi] Reserved1 = 8,
    [PrivateApi] Reserved2 = 16,
    [PrivateApi] Reserved3 = 32,
    [PrivateApi] Reserved4 = 64,
    [PrivateApi] Reserved5 = 128,
    [PrivateApi] Reserved6 = 256,

    /// <summary>
    /// Authority is a preset of 2sxc / EAV
    /// </summary>
    Preset = 512,

    /// <summary>
    /// The installation / global authorized this, so it's a key given by the installation
    /// </summary>
    Global = 1024,


}