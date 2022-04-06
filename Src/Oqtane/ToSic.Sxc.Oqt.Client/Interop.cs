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

        /// <summary>
        /// Added to fix vertical compatibility with Oqtane 3.1
        /// </summary>
        /// <param name="id"></param>
        /// <param name="src"></param>
        /// <param name="integrity"></param>
        /// <param name="crossorigin"></param>
        /// <param name="content"></param>
        /// <param name="location"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public new Task IncludeScript(string id, string src, string integrity, string crossorigin, string content, string location, string key)
        {
            try
            {
                _jsRuntime.InvokeVoidAsync(
                    "Oqtane.Interop.includeScript",
                    id, src, integrity, crossorigin, content, location);
                return Task.CompletedTask;
            }
            catch
            {
                return Task.CompletedTask;
            }
        }
    }
}
