using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps.Blocks
{
    public class BlockConfiguration: HasLog, IAppIdentity
    {
        internal IEntity Entity;
        public  int ZoneId { get; }
        public  int AppId { get; }
        internal readonly bool ShowDrafts;
        internal readonly bool VersioningEnabled;
        internal readonly Guid? PreviewTemplateId;

        public BlockConfiguration(IEntity contentGroupEntity, int zoneId, int appId, bool showDrafts, bool versioningEnabled, ILog parentLog)
            : base("CG.Group", parentLog, "constructor from entity", nameof(BlockConfiguration))
        {
            Entity = contentGroupEntity ?? throw new Exception("BlockConfiguration entity is null. This usually happens when you are duplicating a site, and have not yet imported the other content/apps. If that is your issue, check 2sxc.org/help?tag=export-import");
            ZoneId = zoneId;
            AppId = appId;
            ShowDrafts = showDrafts;
            VersioningEnabled = versioningEnabled;
        }

        /// <summary>
        /// Instantiate a "temporary" BlockConfiguration with the specified templateId and no Content items
        /// </summary>
        public BlockConfiguration(Guid? previewTemplateId, int zoneId, int appId, bool showDrafts, bool versioningEnabled, ILog parentLog)
            : base("CG.Group", parentLog, "constructor empty", nameof(BlockConfiguration))
        {
            PreviewTemplateId = previewTemplateId;
            ZoneId = zoneId;
            AppId = appId;
            ShowDrafts = showDrafts;
        }

        /// <summary>
        /// Returns true if a content group entity for this group really exists
        /// Means for example, that the app can't be changed anymore
        /// </summary>
        public bool Exists => Entity != null;

        internal bool DataIsMissing = false;
        
        
        #region View stuff


        public IView View
        {
            get
            {
                if (_view != null) return _view;

                IEntity templateEntity = null;

                if (PreviewTemplateId.HasValue)
                {
                    var dataSource = new DataSource(Log).GetPublishing(this/*ZoneId, AppId*/);
                    // ToDo: Should use an indexed Guid filter
                    templateEntity =
                        IEntityExtensions.One(dataSource.List, PreviewTemplateId.Value);
                }
                else if (Entity != null)
                    templateEntity = Entity.Children("Template").FirstOrDefault();
                //((EntityRelationship) Entity.Attributes["Template"][0]).FirstOrDefault();

                _view = templateEntity == null ? null : new View(templateEntity, Log);

                return _view;
            }
        }
        private IView _view;


        #endregion



        public int ContentGroupId => Entity?.EntityId ?? 0;

        public Guid ContentGroupGuid => Entity?.EntityGuid ?? Guid.Empty;

        #region Retrieve the lists - either as object or by the type-indexer

        public List<IEntity> Content
        {
            get
            {
                if (Entity == null) return new List<IEntity> {null};
                var list = Entity.Children(ViewParts.Content);
                return list.Count > 0 ? list : new List<IEntity> {null};
            }
        }

        public List<IEntity> Presentation => Entity?.Children(ViewParts.Presentation) ?? new List<IEntity>();

        public List<IEntity> Header => Entity?.Children(ViewParts.ListContent) ?? new List<IEntity>();

        public List<IEntity> HeaderPresentation => Entity?.Children(ViewParts.ListPresentation) ?? new List<IEntity>();

        public List<IEntity> this[string type]
        {
            get
            {
                switch (type.ToLower())
                {
                    case ViewParts.ContentLower:
                        return Content;
                    case ViewParts.PresentationLower: 
                        return Presentation;
                    case ViewParts.ListContentLower:
                        return Header;
                    case ViewParts.ListPresentationLower:
                        return HeaderPresentation;
                    default:
                        throw new Exception("Type " + type + " not allowed");
                }
            }
        }

        internal List<int?> ListWithNulls(string type) 
            => this[type].Select(p => p?.EntityId).ToList();

        #endregion





    }
}