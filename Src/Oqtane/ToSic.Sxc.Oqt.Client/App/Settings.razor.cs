using Oqtane.Modules;
using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class Settings
{
    public override string Title => "2sxc App Settings";

    protected override async Task OnInitializedAsync()
    {
        try
        {

        }
        catch (Exception ex)
        {
            await Task.Run(() => ModuleInstance.AddModuleMessage(ex.Message, MessageType.Error));
        }
    }

    public async Task UpdateSettings()
    {
        try
        {

        }
        catch (Exception ex)
        {
            await Task.Run(() => ModuleInstance.AddModuleMessage(ex.Message, MessageType.Error));
        }
    }
}