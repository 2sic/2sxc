using ToSic.Sxc.Blocks;

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
