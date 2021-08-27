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
        public ValueTask<string> GetTitleValue()
        {
            try
            {
                return _jsRuntime.InvokeAsync<string>("ToSic.Sxc.getTitleValue");
            }
            catch
            {
                return new ValueTask<string>(Task.FromResult(string.Empty));
            }
        }

        public ValueTask<string> GetMetaTagContentByName(string name)
        {
            try
            {
                return _jsRuntime.InvokeAsync<string>(
                    "ToSic.Sxc.getMetaTagContentByName",
                    name);
            }
            catch
            {
                return new ValueTask<string>(Task.FromResult(string.Empty));
            }
        }

        public ValueTask<string> HeadAppend(string name)
        {
            try
            {
                return _jsRuntime.InvokeAsync<string>(
                    "ToSic.Sxc.headAppend",
                    name);
            }
            catch
            {
                return new ValueTask<string>(Task.FromResult(string.Empty));
            }
        }
    }
}
