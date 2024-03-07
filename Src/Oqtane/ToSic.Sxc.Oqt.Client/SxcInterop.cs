using Microsoft.JSInterop;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Client;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SxcInterop : Oqtane.UI.Interop
{
    private readonly IJSRuntime _jsRuntime;

    public SxcInterop(IJSRuntime jsRuntime) : base(jsRuntime)
    {
        _jsRuntime = jsRuntime;

    }
    public ValueTask<string> GetTitleValue()
    {
        try
        {
            return _jsRuntime.InvokeAsync<string>($"{OqtConstants.PackageName}.getTitleValue");
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
                $"{OqtConstants.PackageName}.getMetaTagContentByName",
                name);
        }
        catch
        {
            return new ValueTask<string>(Task.FromResult(string.Empty));
        }
    }

    /// <summary>
    /// IncludeScriptsWithAttributes is fork of
    /// Oqtane.SxcInterop.IncludeScripts from Oqtane v3.1.0
    /// with addition of httpAttributes support
    /// </summary>
    /// <param name="scripts"> scripts (object[]),
    /// script (object) is with optional property httpAttributes (object)
    /// </param>
    /// <returns></returns>
    public async Task IncludeScriptsWithAttributes(object[] scripts)
    {
        // fix for https://github.com/2sic/2sxc/issues/2844
        // we use solution with javascript native module import "./Modules/ToSic.Sxc.Oqtane/NativeModule.js"
        // instead of default oqtane Module.js pattern (that is commented bellow)
        // because our PageChangesHelper.AttachScriptsAndStylesForInteractiveRendering in OnAfterRenderAsync in index.razor.cs
        // is sometimes executing interop call to 'ToSic.Sxc.Oqtane.includeScriptsWithAttributes'
        // earlier than "Modules/ToSic.Sxc.Oqtane/Module.js" is loaded in browser by oqtane ModuleBase.cs
        var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", $"./Modules/{OqtConstants.PackageName}/NativeModule.js");
        await module.InvokeVoidAsync($"includeScriptsWithAttributes", (object)scripts);
        //try
        //{
        //    await _jsRuntime.InvokeVoidAsync(
        //        $"{OqtConstants.PackageName}.includeScriptsWithAttributes",
        //        (object)scripts);
        //}
        //catch
        //{
        //    // ignore exception
        //}
    }
}