using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly CmsRuntime _cmsRuntime;

        public BlockConfiguration(IEntity contentGroupEntity, CmsRuntime cmsRuntime, ILog parentLog)
            : this(cmsRuntime, parentLog, "constructor from entity")
        {
            Entity = contentGroupEntity 
                     ?? throw new Exception("BlockConfiguration entity is null. " +
                                            "This usually happens when you are duplicating a site, and have not yet imported the other content/apps. " +
                                            "If that is your issue, check 2sxc.org/help?tag=export-import");
        }

        /// <summary>
        /// Instantiate a "temporary" BlockConfiguration with the specified templateId and no Content items
        /// </summary>
        public BlockConfiguration(Guid? previewTemplateId, CmsRuntime cmsRuntime, ILog parentLog)
            : this(cmsRuntime, parentLog, "constructor empty")
        {
            PreviewTemplateId = previewTemplateId;
        }

        public BlockConfiguration(CmsRuntime cmsRuntime, ILog parentLog, string logNote)
            : base("Blk.Config", new CodeRef(), parentLog, logNote)
        {
            _cmsRuntime = cmsRuntime;
            ZoneId = cmsRuntime.ZoneId;
            AppId = cmsRuntime.AppId;
            ShowDrafts = cmsRuntime.ShowDrafts;
            VersioningEnabled = cmsRuntime.WithPublishing;
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

                // if we're previewing another template, look that up
                if (PreviewTemplateId.HasValue)
                {
                    //var dataSource = new DataSource(Log).GetPublishing(this, ShowDrafts);
                    // ToDo: Should use an indexed Guid filter
                    templateEntity = _cmsRuntime.Data.List.One(PreviewTemplateId.Value);
                }
                else if (Entity != null)
                    templateEntity = Entity.Children(ViewParts.TemplateContentType).FirstOrDefault();

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

        [Obsolete]
        internal List<int?> ListWithNulls(string type) 
            => this[type].Select(p => p?.EntityId).ToList();

        #endregion
    }
}