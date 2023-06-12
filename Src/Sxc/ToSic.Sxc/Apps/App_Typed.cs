using ToSic.Sxc.Data;

namespace ToSic.Sxc.Apps
{
    public partial class App: IAppTyped
    {
        ITypedItem IAppTyped.Settings => (ITypedItem)Settings;

        ITypedItem IAppTyped.Resources => (ITypedItem)Resources;
    }
}
