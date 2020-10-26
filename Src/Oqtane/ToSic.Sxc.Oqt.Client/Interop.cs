using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Client
{
    public class Interop
    {
        private readonly IJSRuntime _jsRuntime;

        public Interop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> GetElementByName(string name)
        {
            try
            {
                return _jsRuntime.InvokeAsync<string>(
                    "Oqtane.Interop.getElementByName",
                    name);
            }
            catch
            {
                return new ValueTask<string>(Task.FromResult(string.Empty));
            }
        }

        public Task IncludeMeta(string id, string attribute, string name, string content, string key)
        {
            try
            {
                _jsRuntime.InvokeVoidAsync(
                    "Oqtane.Interop.includeMeta",
                    id, attribute, name, content, key);
                return Task.CompletedTask;
            }
            catch
            {
                return Task.CompletedTask;
            }
        }

        public Task IncludeScript(string id, string src, string integrity, string crossorigin, string content, string location, string key)
        {
            try
            {
                _jsRuntime.InvokeVoidAsync(
                    "Oqtane.Interop.includeScript",
                    id, src, integrity, crossorigin, content, location, key);
                return Task.CompletedTask;
            }
            catch
            {
                return Task.CompletedTask;
            }
        }

        public async Task IncludeScripts(object[] scripts)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync(
                    "Oqtane.Interop.includeScripts",
                    (object)scripts);
            }
            catch
            {
                // ignore exception
            }
        }

        public Task UpdateTitle(string title)
        {
            try
            {
                _jsRuntime.InvokeVoidAsync(
                    "Oqtane.Interop.updateTitle",
                    title);
                return Task.CompletedTask;
            }
            catch
            {
                return Task.CompletedTask;
            }
        }
    }
}
