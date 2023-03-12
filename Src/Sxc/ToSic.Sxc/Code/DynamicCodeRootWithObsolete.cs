#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Catalog;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Eav.LookUp;
using ToSic.SexyContent;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public class DynamicCodeObsolete
    {
        private readonly IDynamicCodeRoot _root;
        public DynamicCodeObsolete(IDynamicCodeRoot dynCode)
        {
            _root = dynCode;
        }


        [PrivateApi("obsolete")]
        [Obsolete("you should use the CreateSource<T> instead. Deprecated ca. v4 (but not sure), changed to error in v15.")]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine lookUpEngine = null)
        {
            // 2023-03-12 2dm
            // Completely rewrote this, because I got rid of some old APIs in v15 on the DataFactory
            // This has never been tested but probably works, but we won't invest time to be certain.

            try
            {
                // try to find with assembly name, or otherwise with GlobalName / previous names
                var type = DataSourceCatalog.FindType(typeName);
                lookUpEngine = lookUpEngine ?? _root.ConfigurationProvider;

                if (inSource != null)
                    return _root.DataSourceFactory.Create(type, inSource, inSource, lookUpEngine);

                var userMayEdit = _root.Block?.Context?.UserMayEdit ?? false;

                var initialSource = _root.DataSourceFactory.GetPublishing(_root.App, userMayEdit,
                    _root.ConfigurationProvider as LookUpEngine);
                return typeName != ""
                    ? _root.DataSourceFactory.Create(type, initialSource, initialSource, lookUpEngine)
                    : initialSource;
            }
            catch (Exception ex)
            {
                var errMessage = $"The razor code is calling a very old method {nameof(CreateSource)}." +
                                 $" In this version, you used the type name as a string {nameof(CreateSource)}(string typeName, ...)." +
                                 $" This has been deprecated since ca. v4 and has been removed now. " +
                                 $" Please use the newer {nameof(CreateSource)}<Type>(...) overload.";

                throw new Exception(errMessage, ex);
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
            _root.Log.A("try to build old List");
            _list = new List<Element>();

            if (_root.Data == null || _root.Block.View == null) return;
            if (!_root.Data.Out.ContainsKey(DataSourceConstants.DefaultStreamName)) return;

            var entities = _root.Data.List.ToList();

            _list = entities.Select(GetElementFromEntity).ToList();

            Element GetElementFromEntity(IEntity e)
            {
                var el = new Element
                {
                    EntityId = e.EntityId,
                    Content = _root.AsDynamic(e)
                };

                var editDecorator = e.GetDecorator<EntityInBlockDecorator>();

                //if (e is EntityInBlock c)
                if (editDecorator != null)
                {
                    // 2021-10-12 2dm #dropGroupId - believe this is never used anywhere. Leave comment till EOY 2021
                    //el.GroupId = editDecorator.GroupId;
                    el.Presentation = editDecorator.Presentation == null ? null : _root.AsDynamic(editDecorator.Presentation);
                    el.SortOrder = editDecorator.SortOrder;
                }

                return el;
            }
        }
#pragma warning restore 618

    }
}

#endif