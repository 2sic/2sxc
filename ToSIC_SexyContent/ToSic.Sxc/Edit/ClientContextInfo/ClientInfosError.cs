using ToSic.Sxc.Interfaces;

// ReSharper disable InconsistentNaming

namespace ToSic.Sxc.Edit.ClientContextInfo
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
