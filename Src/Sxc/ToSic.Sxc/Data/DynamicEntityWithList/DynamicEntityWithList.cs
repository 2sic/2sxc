using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a List of <see cref="IDynamicEntity"/>, which also behaves as an IDynamicEntity itself. <br/>
    /// So if it has any elements you can enumerate it (foreach). <br/>
    /// You can also do things like `.EntityId` or `.SomeProperty` just like a DynamicEntity.
    /// </summary>
    /// <remarks>
    /// * Added in 2sxc 10.27
    /// * Hidden from docs in 2sxc 12.03 because we'll probably merge the functionality with IDynamicEntity
    /// - it was never actually documented / mentioned, and since the use is usually dynamic, people won't have any code with the type-name
    /// </remarks>
    [PrivateApi]
    public partial class DynamicEntityWithList: DynamicEntity, IReadOnlyList<IDynamicEntity>
    {
        //[PrivateApi] protected List<IDynamicEntity> DynEntities => ListHelper.DynEntities;
        
        //[PrivateApi]
        //protected List<IDynamicEntity> DynEntities {
        //    get
        //    {
        //        if (_list != null) return _list;
        //        var index = 0;
        //        _list = _listEntities != null // note: ATM as of 2021-06-16 this can never happen, but it will in future for single-entity use
        //            ? _listEntities
        //                .Select(e =>
        //                {
        //                    // we create an Entity with some metadata-decoration, so that toolbars know it's part of a list
        //                    var blockEntity = new EntityInBlock(e, _parent.EntityGuid, _parentField, index++);
        //                    return SubDynEntity(blockEntity);
        //                })
        //                .ToList()
        //            : new List<IDynamicEntity> {this};
        //        return _list;
        //    }
        //}

        //private List<IDynamicEntity> _list;
        //private readonly IEntity[] _listEntities;

        // ReSharper disable once NotAccessedField.Local
        //private readonly IEntity _parent;
        //private readonly string _parentField;
        internal readonly DynamicEntityListHelper ListHelper;

        [PrivateApi]
        internal DynamicEntityWithList(IEntity parent, string field, IEnumerable<IEntity> entities, DynamicEntityDependencies dependencies) 
            : base(null, dependencies)
        {
            // Set the entity - if there was one, or if the list is empty, create a dummy Entity so toolbars will know what to do
            SetEntity(entities.FirstOrDefault() ?? PlaceHolder(parent, field));
            
            ListHelper = new DynamicEntityListHelper(parent, field, entities, dependencies);
            //_parent = parent;
            //_parentField = field;
            //_listEntities = entities.ToArray();
            //var index = 0;
            //DynEntities = _listEntities
            //    .Select(e =>
            //    {
            //        // we create an Entity with some metadata-decoration, so that toolbars know it's part of a list
            //        var blockEntity = new EntityInBlock(e, parent.EntityGuid, _parentField, index++);
            //        return SubDynEntity(blockEntity);
            //    })
            //    .ToList();

        }

        public IEnumerator<IDynamicEntity> GetEnumerator() => ListHelper.DynEntities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => ListHelper.DynEntities.Count;

        public IDynamicEntity this[int index]
        {
            get => ListHelper.DynEntities[index];
            // note: set must be defined for IList<IDynamicEntity>
            set => throw new NotImplementedException();
        }

        private EntityInBlock PlaceHolder(IEntity parent, string field)
        {
            var sp = _Dependencies.ServiceProviderOrNull ?? Eav.Factory.GetServiceProvider();
            var builder = sp.Build<IDataBuilder>();
            return new EntityInBlock(builder.FakeEntity(parent.AppId), parent.EntityGuid, field, 0);
        }
    }
}
