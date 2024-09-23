using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Client.Services;

internal class OqtDebugStateService : IOqtDebugStateService
{
    public const string DebugKey = "2sxcDebug";
    private readonly IJSRuntime _jsRuntime;

    public OqtDebugStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public bool IsDebugEnabled => _debug ??= false; 

    public async Task<bool> GetDebugAsync() => await GetState<bool>(DebugKey);

    public void SetDebug(bool value)
    {
        _debug = value;
#pragma warning disable CS4014
        SaveState(DebugKey, value);
#pragma warning restore CS4014
    }

    private bool? _debug;

    private async Task SaveState(string key, object value)
    {
        var json = JsonSerializer.Serialize(value);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, json);
    }

    private async ValueTask<T> GetState<T>(string key)
    {
        var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
        if (json is null) return default;
        return JsonSerializer.Deserialize<T>(json);
    }

    public string Platform => "Client";
}