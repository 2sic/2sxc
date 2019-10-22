using System;
using System.Collections.Generic;
using ToSic.Eav.Interfaces;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.EAVExtensions
{
    public abstract class EntityDecorator : IEntity
    {
        private readonly IEntity _baseEntity;

        protected EntityDecorator(IEntity baseEntity)
        {
            _baseEntity = baseEntity;
        }

        #region IEntity Implementation

        public IEntity GetDraft() => _baseEntity.GetDraft();
        

        public IEntity GetPublished() => _baseEntity.GetPublished();




        public int AppId => _baseEntity.AppId;

        public int EntityId => _baseEntity.EntityId;
        public int RepositoryId => _baseEntity.RepositoryId;
        public Guid EntityGuid => _baseEntity.EntityGuid;

        public IMetadataFor MetadataFor => _baseEntity.MetadataFor;

        public Dictionary<string, IAttribute> Attributes => _baseEntity.Attributes;
        public IContentType Type => _baseEntity.Type;
        public IAttribute Title => _baseEntity.Title;

        public DateTime Modified => _baseEntity.Modified;

        public IAttribute this[string attributeName] => _baseEntity[attributeName];

        public IRelationshipManager Relationships => _baseEntity.Relationships;

        public bool IsPublished => _baseEntity.IsPublished;

        public string Owner => _baseEntity.Owner;


        public object GetBestValue(string attributeName, bool resolveHyperlinks = false)
            => _baseEntity.GetBestValue(attributeName, resolveHyperlinks);


        public object GetBestValue(string attributeName, string[] languages, bool resolveHyperlinks = false)
            => _baseEntity.GetBestValue(attributeName, languages, resolveHyperlinks);

        public T GetBestValue<T>(string attributeName, string[] languages, bool resolveHyperlinks = false) 
            => _baseEntity.GetBestValue<T>(attributeName, languages, resolveHyperlinks);

        public object PrimaryValue(string attributeName, bool resolveHyperlinks = false) 
            => _baseEntity.PrimaryValue(attributeName, resolveHyperlinks);

        public T PrimaryValue<T>(string attributeName, bool resolveHyperlinks = false) 
            => _baseEntity.PrimaryValue<T>(attributeName, resolveHyperlinks);

        public TVal GetBestValue<TVal>(string name, bool resolveHyperlinks = false)
            => _baseEntity.GetBestValue<TVal>(name, resolveHyperlinks);

        public string GetBestTitle() => _baseEntity.GetBestTitle();

        public string GetBestTitle(string[] dimensions)
            => _baseEntity.GetBestTitle(dimensions);

        object IEntityLight.Title => ((IEntityLight) _baseEntity).Title;

        object IEntityLight.this[string attributeName] => ((IEntityLight) _baseEntity)[attributeName];

        public int Version => _baseEntity.Version;

        public IMetadataOfItem Metadata => _baseEntity.Metadata;
        public IEnumerable<IEntity> Permissions => _baseEntity.Permissions;

        #endregion




        #region experimental support for LINQ enhancements
        public List<IEntity> Children(string field = null, string type = null) => _baseEntity.Children(field, type);

        public List<IEntity> Parents(string type = null, string field = null) => _baseEntity.Parents(type, field);
        public object Value(string field, bool resolve = true) => _baseEntity.Value(field, resolve);

        public T Value<T>(string field, bool resolve = true) => _baseEntity.Value<T>(field, resolve);

        #endregion
    }
}