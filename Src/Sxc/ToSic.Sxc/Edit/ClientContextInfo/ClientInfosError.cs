using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ClientInfosError
    {
        // ReSharper disable once InconsistentNaming
        public string type;

        internal ClientInfosError(IBlock cb)
        {
            if (cb.DataIsMissing)
                type = "DataIsMissing";
        }
    }
}
