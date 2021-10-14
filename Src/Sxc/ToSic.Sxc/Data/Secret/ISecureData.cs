using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    public interface ISecureData<out T>
    {
        T Value { get; }

        [PrivateApi("not final yet")]
        bool IsEncrypted { get; }

        [PrivateApi("not final yet")]
        bool IsSigned { get; }

        [PrivateApi("not final yet")]
        SecretAuthorities Authority { get; }

        /// <summary>
        /// Determines if the data is secure data, so it's either encrypted or signed
        /// </summary>
        [PrivateApi("name not final yet")]
        bool IsSecure { get; }


        /// <summary>
        /// Determines what authority secured this secure data.
        /// This is to figure out what certificate or source verified the decryption / signing
        ///
        /// As of 2sxc 12.05, it can only be "preset", other keys are currently not handled yet
        /// </summary>
        /// <param name="authorityName"></param>
        /// <returns></returns>
        bool IsSecuredBy(string authorityName);

        /// <summary>
        /// This object explicitly has a ToString, so you can use the result in string concatenation like `"key:" + secureResult`
        /// </summary>
        /// <returns></returns>
        string ToString();

    }
}
