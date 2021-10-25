using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web
{
    public class RazorServiceUnknown : IRazorService
    {
        public RazorServiceUnknown(WarnUseOfUnknown<RazorServiceUnknown> warn)
        {
        }

        public string Render(string partialName, object model)
        {
            throw new System.NotImplementedException();
        }

        public void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            throw new System.NotImplementedException();
        }
    }
}