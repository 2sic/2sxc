using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using IEntity = ToSic.Eav.Data.IEntity;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.EAVExtensions
{
    /// <summary>
    /// This is an <see cref="IEntity"/> which has more properties and information.
    /// Everything in the original <see cref="IEntity"/> is passed through invisibly.
    /// </summary>
    [PublicApi]
    public abstract class EntityDecorator : IEntity
    {
        private readonly IEntity _baseEntity;

        /// <summary>
        /// Initialize the object and store the underlying IEntity.
        /// </summary>
        /// <param name="baseEntity"></param>
        protected EntityDecorator(IEntity baseEntity)
        {
            _baseEntity = baseEntity;
        }

        #region IEntity Implementation

        /// <inheritdoc />
        public IEntity GetDraft() => _baseEntity.GetDraft();

        /// <inheritdoc />
        public IEntity GetPublished() => _baseEntity.GetPublished();

        /// <inheritdoc />
        public int AppId => _baseEntity.AppId;

        /// <inheritdoc />
        public int EntityId => _baseEntity.EntityId;

        /// <inheritdoc />
        public int RepositoryId => _baseEntity.RepositoryId;

        /// <inheritdoc />
        public Guid EntityGuid => _baseEntity.EntityGuid;

        /// <inheritdoc />
        public IMetadataFor MetadataFor => _baseEntity.MetadataFor;

        /// <inheritdoc cref="IEntity" />
        public Dictionary<string, IAttribute> Attributes => _baseEntity.Attributes;

        /// <inheritdoc />
        public IContentType Type => _baseEntity.Type;

        /// <inheritdoc cref="IEntity" />
        public IAttribute Title => _baseEntity.Title;

        /// <inheritdoc />
        public DateTime Modified => _baseEntity.Modified;

         /// <inheritdoc cref="IEntity" />
       public IAttribute this[string attributeName] => _baseEntity[attributeName];

        /// <inheritdoc />
        public IRelationshipManager Relationships => _baseEntity.Relationships;

        /// <inheritdoc />
        public bool IsPublished => _baseEntity.IsPublished;

        /// <inheritdoc />
        public string Owner => _baseEntity.Owner;


        /// <inheritdoc />
        public object GetBestValue(string attributeName, bool resolveHyperlinks = false)
            => _baseEntity.GetBestValue(attributeName, resolveHyperlinks);


        /// <inheritdoc cref="IEntity" />
        public object GetBestValue(string attributeName, string[] languages, bool resolveHyperlinks = false)
            => _baseEntity.GetBestValue(attributeName, languages, resolveHyperlinks);

        /// <inheritdoc cref="IEntity" />
        public T GetBestValue<T>(string attributeName, string[] languages, bool resolveHyperlinks = false) 
            => _baseEntity.GetBestValue<T>(attributeName, languages, resolveHyperlinks);

        [PrivateApi]
        public object PrimaryValue(string attributeName, bool resolveHyperlinks = false) 
            => _baseEntity.PrimaryValue(attributeName, resolveHyperlinks);

        [PrivateApi]
        public T PrimaryValue<T>(string attributeName, bool resolveHyperlinks = false) 
            => _baseEntity.PrimaryValue<T>(attributeName, resolveHyperlinks);

        /// <inheritdoc />
        public TVal GetBestValue<TVal>(string name, bool resolveHyperlinks = false)
            => _baseEntity.GetBestValue<TVal>(name, resolveHyperlinks);

        /// <inheritdoc />
        public string GetBestTitle() => _baseEntity.GetBestTitle();

        /// <inheritdoc cref="IEntity" />
        public string GetBestTitle(string[] dimensions)
            => _baseEntity.GetBestTitle(dimensions);

        /// <inheritdoc />
        object IEntityLight.Title => ((IEntityLight) _baseEntity).Title;

        /// <inheritdoc />
        object IEntityLight.this[string attributeName] => ((IEntityLight) _baseEntity)[attributeName];

        /// <inheritdoc cref="IEntity" />
        public int Version => _baseEntity.Version;

        /// <inheritdoc cref="IEntity" />
        public IMetadataOfItem Metadata => _baseEntity.Metadata;

        /// <inheritdoc />
        public IEnumerable<IEntity> Permissions => _baseEntity.Permissions;

        #endregion


        /// <inheritdoc cref="IEntity" />
        public List<IEntity> Children(string field = null, string type = null) => _baseEntity.Children(field, type);

        /// <inheritdoc cref="IEntity" />
        public List<IEntity> Parents(string type = null, string field = null) => _baseEntity.Parents(type, field);


        #region experimental support for LINQ enhancements

        [PrivateApi]
        public object Value(string field, bool resolve = true) => _baseEntity.Value(field, resolve);

        [PrivateApi]
        public T Value<T>(string field, bool resolve = true) => _baseEntity.Value<T>(field, resolve);

        #endregion

    }
}