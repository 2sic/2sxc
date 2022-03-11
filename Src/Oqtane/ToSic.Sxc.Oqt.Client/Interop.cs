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

        /// <summary>
        /// IncludeScriptsWithAttributes is fork of
        /// Oqtane.Interop.IncludeScripts from Oqtane v3.0.3
        /// with addition of httpAttributes support
        /// </summary>
        /// <param name="scripts"> scripts (object[]),
        /// script (object) is with optional property httpAttributes (object)
        /// </param>
        /// <returns></returns>
        public async Task IncludeScriptsWithAttributes(object[] scripts)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync(
                    "ToSic.Sxc.includeScriptsWithAttributes",
                    (object)scripts);
            }
            catch
            {
                // ignore exception
            }
        }
    }
}
