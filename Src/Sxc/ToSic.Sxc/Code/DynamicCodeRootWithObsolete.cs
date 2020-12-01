using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.SexyContent;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public class DynamicCodeObsolete
    {
        public DynamicCodeRoot DynCode;
        public DynamicCodeObsolete(DynamicCodeRoot dynCode)
        {
            DynCode = dynCode;
        }

        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => DynCode.AsDynamic(entity as IEntity);


        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => DynCode.AsDynamic(entityKeyValuePair.Value);

        //[PrivateApi]
        //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        //public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => entities.Select(e => AsDynamic(e));

        ///// <inheritdoc />
        //public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => entities.Select(e => AsDynamic(e));


        [PrivateApi("obsolete")]
        [Obsolete("you should use the CreateSource<T> instead")]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine lookUpEngine = null)
        {
            if (lookUpEngine == null)
                lookUpEngine = DynCode.ConfigurationProvider;

            if (inSource != null)
                return DynCode.DataSourceFactory.GetDataSource(typeName, inSource, inSource, lookUpEngine);

            var userMayEdit = DynCode.Block.Context.UserMayEdit;

            var initialSource = DynCode.DataSourceFactory.GetPublishing(
                DynCode.App, userMayEdit, DynCode.ConfigurationProvider as LookUpEngine);
            return typeName != "" ? DynCode.DataSourceFactory.GetDataSource(typeName, initialSource, initialSource, lookUpEngine) : initialSource;
        }


        //[PrivateApi]
        //[Obsolete("use Header instead")]
        //public dynamic ListContent => DynCode.Header;

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
            DynCode.Log.Add("try to build old List");
            _list = new List<Element>();

            if (DynCode.Data == null || DynCode.Block.View == null) return;
            if (!DynCode.Data.Out.ContainsKey(Eav.Constants.DefaultStreamName)) return;

            var entities = DynCode.Data.Immutable.ToList();
            //if (entities.Any()) _content = AsDynamic(entities.First());

            _list = entities.Select(GetElementFromEntity).ToList();

            Element GetElementFromEntity(IEntity e)
            {
                var el = new Element
                {
                    EntityId = e.EntityId,
                    Content = DynCode.AsDynamic(e)
                };

                if (e is EntityInBlock c)
                {
                    el.GroupId = c.GroupId;
                    el.Presentation = c.Presentation == null ? null : DynCode.AsDynamic(c.Presentation);
                    el.SortOrder = c.SortOrder;
                }

                return el;
            }
        }
#pragma warning restore 618

    }
}
