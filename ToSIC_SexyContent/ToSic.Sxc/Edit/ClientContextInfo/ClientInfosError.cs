using ToSic.SexyContent.Interfaces;

// ReSharper disable InconsistentNaming

namespace ToSic.SexyContent.Edit.ClientContextInfo
{
    public class ClientInfosError
    {
        public string type;

        internal ClientInfosError(IContentBlock cb)
        {
            if (cb.DataIsMissing)
                type = "DataIsMissing";
        }
    }
}
