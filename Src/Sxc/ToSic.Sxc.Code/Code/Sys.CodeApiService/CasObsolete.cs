#if NETFRAMEWORK
using ToSic.Eav.Data.EntityDecorators.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.SexyContent;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeApiServiceObsolete(IExecutionContext dynCode)
{
    [PrivateApi("obsolete")]
    [Obsolete("you should use the CreateSource<T> instead. Deprecated ca. v4 (but not sure), changed to error in v15.")]
    public IDataSource CreateSource(string typeName = "", IDataSource? links = null, ILookUpEngine? configuration = null)
    {
        // 2023-03-12 2dm
        // Completely rewrote this, because I got rid of some old APIs in v15 on the DataFactory
        // This has never been tested but probably works, but we won't invest time to be certain.

        var dataSources = ((ExecutionContext)dynCode).DataSources;

        try
        {
            // try to find with assembly name, or otherwise with GlobalName / previous names
            var app = dynCode.GetApp();
            var type = dataSources.Catalog.Value.FindDataSourceInfo(typeName, app.AppId)?.Type;
            configuration ??= dataSources.LookUpEngine;
            var cnf2Wip = new DataSourceOptions
            {
                AppIdentityOrReader = null, // #WipAppIdentityOrReader must become not null
                LookUp = configuration,
            };
            if (links != null)
                return dataSources.DataSources.Value.Create(type: type!, attach: links, options: cnf2Wip);

            var initialSource = dataSources.DataSources.Value.CreateDefault(new DataSourceOptions
            {
                AppIdentityOrReader = app,
                LookUp = dataSources.LookUpEngine,
            });
            return typeName != ""
                ? dataSources.DataSources.Value.Create(type: type!, attach: initialSource, options: cnf2Wip)
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


    // #RemovedV20 #Element
    //#pragma warning disable 618
    //[PrivateApi]
    //[Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in 2sxc v10")]
    //[field: AllowNull, MaybeNull]
    //public List<Element> ElementList => field ??= TryToBuildElementList();

    //[field: AllowNull, MaybeNull]
    //private ICodeDataFactory Cdf => field ??= dynCode.GetCdf();

    // #RemovedV20 #Element
    ///// <remarks>
    ///// This must be lazy-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
    ///// - this would break API even though the List etc. are never accessed
    ///// </remarks>
    //private List<Element> TryToBuildElementList()
    //{
    //    dynCode.Log.A("try to build old List");

    //    var block = dynCode.GetState<IBlock>();
    //    if (!block.DataIsReady || !block.ViewIsReady)
    //        return [];

    //    var data = dynCode.GetState<IDataSource>();
    //    if (data == null! /* paranoid */ || !data.Out.ContainsKey(DataSourceConstants.StreamDefaultName))
    //        return [];

    //    var entities = data.List.ToList();

    //    return entities.Select(GetElementFromEntity).ToList();

    //    Element GetElementFromEntity(IEntity e)
    //    {
    //        var el = new Element
    //        {
    //            EntityId = e.EntityId,
    //            Content = Cdf.CodeAsDyn(e)
    //        };

    //        var editDecorator = e.GetDecorator<EntityInBlockDecorator>();

    //        if (editDecorator != null)
    //        {
    //            el.Presentation = editDecorator.Presentation == null ? null : Cdf.CodeAsDyn(editDecorator.Presentation);
    //            el.SortOrder = editDecorator.SortOrder;
    //        }

    //        return el;
    //    }
    //}
    //#pragma warning restore 618

}

#endif