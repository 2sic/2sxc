#if NETFRAMEWORK

using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Compatibility.Internal
{
	/// <summary>
    /// Old interface for the SexyContent Web Page
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IDynamicCodeBeforeV10
    {

        /// <summary>
        /// this is for compatibility with old systems, to ensure that things cast to IEntity in a razor can still be cast back
        /// </summary>
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        dynamic AsDynamic(Eav.Interfaces.IEntity entity);

        /// <summary>
        /// Returns the value of a KeyValuePair as DynamicEntity
        /// </summary>
        /// <param name="entityKeyValuePair"></param>
        /// <returns></returns>
        dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair);

        /// <summary>
        /// this is for compatibility with old systems, to ensure that things cast to IEntity in a razor can still be cast back
        /// </summary>
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair);

        /// <summary>
        /// this is for compatibility with old systems, to ensure that things cast to IEntity in a razor can still be cast back
        /// </summary>
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities);

        /// <summary>
        /// Create a <see cref="IDataSource"/> which will process data from the given stream.
        /// </summary>
        /// <returns>A typed DataSource object</returns>
        [Obsolete("Please use the CreateSource<T> overload instead.")]
        [PrivateApi]
        IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine configurationProvider = null);

    }
}
#endif