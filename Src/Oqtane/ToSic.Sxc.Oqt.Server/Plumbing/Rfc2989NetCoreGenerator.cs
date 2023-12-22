using System.Security.Cryptography;
using ToSic.Eav.Security.Encryption;

namespace ToSic.Sxc.Oqt.Server.Plumbing;

/// <summary>
/// The built in Generator in EAV can only implement .net 472 because
/// the .net Standard 2.0 implementation lacks the option to specify the HashAlgorithm
/// This ensures that we also have this working in Oqtane
/// </summary>
internal class Rfc2898NetCoreGenerator : Rfc2898Generator
{
    public override byte[] GetKeyBytes(AesConfiguration config)
    {
        var pwBytes = new Rfc2898DeriveBytes(config.Password, config.SaltBytes(), config.KeyGenIterations, HashAlgorithmName.SHA256);
        var keyNew = pwBytes.GetBytes(config.KeySize / 8);
        return keyNew;
    }
}