#if NETFRAMEWORK
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.LookUp;
using ToSic.SexyContent;
using ToSic.Sxc.Data.Internal.Decorators;

namespace ToSic.Sxc.Code.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeApiServiceObsolete(ICodeApiService dynCode)
{
    [PrivateApi("obsolete")]
    [Obsolete("you should use the CreateSource<T> instead. Deprecated ca. v4 (but not sure), changed to error in v15.")]
    public IDataSource CreateSource(string typeName = "", IDataSource links = null, ILookUpEngine configuration = null)
    {
        // 2023-03-12 2dm
        // Completely rewrote this, because I got rid of some old APIs in v15 on the DataFactory
        // This has never been tested but probably works, but we won't invest time to be certain.

        var dataSources = ((CodeApiService)dynCode).DataSources;

        try
        {
            // try to find with assembly name, or otherwise with GlobalName / previous names
            //var catalog = _root.GetService<DataSourceCatalog>();
            var type = dataSources.Catalog.Value.FindDataSourceInfo(typeName, dynCode.App.AppId)?.Type;
            configuration ??= dataSources.LookUpEngine; // _root.ConfigurationProvider;
            var cnf2Wip = new DataSourceOptions(lookUp: configuration);
            if (links != null)
                return dataSources.DataSources.Value/*_root.DataSourceFactory*/.Create(type: type, attach: links, options: cnf2Wip);

            var initialSource = dataSources.DataSources.Value/* _root.DataSourceFactory*/
                .CreateDefault(new DataSourceOptions(appIdentity: dynCode.App, lookUp: dataSources.LookUpEngine/*_root.ConfigurationProvider*/));
            return typeName != ""
                ? dataSources.DataSources.Value/*_root.DataSourceFactory*/.Create(type: type, attach: initialSource, options: cnf2Wip)
                : initialSource;
        }
        catch (Exception ex)
        {
            var errMessage = $"The razor code is calling a very old method {nameof(CreateSource)}." +
                             $" In this version, you used the type name as a string {nameof(CreateSource)}(string typeName, ...)." +
                             $" This has been deprecated since ca. v4 and has been removed now. " +
                             $" Please use the newer {nameof(CreateSource)}<Type>(...) overload.";

            throw new(errMessage, ex);
        }
    }


#pragma warning disable 618
    [PrivateApi]
    [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in 2sxc v10")]
    public List<Element> ElementList
    {
        get
        {
            if (_list == null) TryToBuildElementList();
            return _list;
        }
    }
    private List<Element> _list;


    /// <remarks>
    /// This must be lazy-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
    /// - this would break API even though the List etc. are never accessed
    /// </remarks>
    private void TryToBuildElementList()
    {
        dynCode.Log.A("try to build old List");
        _list = [];

        if (dynCode.Data == null || ((ICodeApiServiceInternal)dynCode)._Block.View == null) return;
        if (!dynCode.Data.Out.ContainsKey(DataSourceConstants.StreamDefaultName)) return;

        var entities = dynCode.Data.List.ToList();

        _list = entities.Select(GetElementFromEntity).ToList();

        Element GetElementFromEntity(IEntity e)
        {
            var el = new Element
            {
                EntityId = e.EntityId,
                Content = dynCode.Cdf.CodeAsDyn(e)
            };

            var editDecorator = e.GetDecorator<EntityInBlockDecorator>();

            if (editDecorator != null)
            {
                el.Presentation = editDecorator.Presentation == null ? null : dynCode.Cdf.CodeAsDyn(editDecorator.Presentation);
                el.SortOrder = editDecorator.SortOrder;
            }

            return el;
        }
    }
#pragma warning restore 618

}

#endif