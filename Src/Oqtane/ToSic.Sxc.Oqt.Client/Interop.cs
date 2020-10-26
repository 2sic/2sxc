using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Client
{
    public class Interop : Oqtane.UI.Interop
    {
        private readonly IJSRuntime _jsRuntime;

        public Interop(IJSRuntime jsRuntime) : base(jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
    }
}
