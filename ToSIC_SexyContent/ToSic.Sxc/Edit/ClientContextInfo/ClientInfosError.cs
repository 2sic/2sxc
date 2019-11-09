using ToSic.Sxc.Blocks;

// ReSharper disable InconsistentNaming

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ClientInfosError
    {
        public string type;

        internal ClientInfosError(IBlock cb)
        {
            if (cb.DataIsMissing)
                type = "DataIsMissing";
        }
    }
}
