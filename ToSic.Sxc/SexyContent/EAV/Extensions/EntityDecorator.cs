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

        public string GetBestTitle() => _baseEntity.GetBestTitle();


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

        public string GetBestTitle(string[] dimensions)
            => _baseEntity.GetBestTitle(dimensions);

        object IEntityLight.Title => ((IEntityLight) _baseEntity).Title;

        object IEntityLight.this[string attributeName] => ((IEntityLight) _baseEntity)[attributeName];

        public int Version => _baseEntity.Version;

        public IMetadataOfItem Metadata => _baseEntity.Metadata;

        #endregion

        #region breaking changes

        public object Value => _baseEntity.Value;

        #endregion
    }
}