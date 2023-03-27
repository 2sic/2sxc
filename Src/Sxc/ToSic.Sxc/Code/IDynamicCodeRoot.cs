using ToSic.Eav.DataSources;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// This is the same as IDynamicCode, but the root object. 
    /// We create another interface to ensure we don't accidentally pass around a sub-object where the root is really needed.
    /// </summary>
    [PrivateApi]
    public interface IDynamicCodeRoot : IDynamicCode12
    {
        [PrivateApi("WIP")] IBlock Block { get; }

        [PrivateApi]
        ILookUpEngine ConfigurationProvider { get; }
        
        [PrivateApi]
        IDataSourceFactory DataSourceFactory { get; }

        [PrivateApi]
        void AttachApp(IApp app);

    }
}
