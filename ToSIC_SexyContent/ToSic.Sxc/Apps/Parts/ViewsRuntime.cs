using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.DataSources;
using ToSic.Eav.Logging;
using ToSic.Eav.Serializers;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Engines;

// note: not sure if the final namespace should be Sxc.Apps or Sxc.Views
namespace ToSic.Sxc.Apps
{
	public class ViewsRuntime: CmsRuntimeBase
    {
        internal ViewsRuntime(CmsRuntime cmsRuntime, ILog parentLog) : base(cmsRuntime, parentLog, "Cms.ViewRd") { }

        private IDataSource _viewDs;
		private IDataSource ViewsDataSource()
		{
            if(_viewDs!= null)return _viewDs;
		    // ReSharper disable once RedundantArgumentDefaultValue
            var dataSource = App.Data;
			dataSource = DataSource.GetDataSource<EntityTypeFilter>(upstream: dataSource);
		    ((EntityTypeFilter) dataSource).TypeName = Configuration.TemplateContentType;
		    _viewDs = dataSource;
			return dataSource;
		}

        public IEnumerable<IView> GetAll() 
            => _all ?? (_all = ViewsDataSource().List
                   .Select(p => new View(p, Log))
                   .OrderBy(p => p.Name));
        private IEnumerable<IView> _all;

        public IEnumerable<IView> GetRazor() => GetAll().Where(t => t.IsRazor);
        public IEnumerable<IView> GetToken() => GetAll().Where(t => !t.IsRazor);


        public IView Get(int templateId)
		{
			var dataSource = ViewsDataSource();
			dataSource = DataSource.GetDataSource<EntityIdFilter>(upstream: dataSource);
			((EntityIdFilter)dataSource).EntityIds = templateId.ToString();
			var templateEntity = dataSource.List.FirstOrDefault();

			if(templateEntity == null)
				throw new Exception("The template with id " + templateId + " does not exist.");

			return new View(templateEntity, Log);
		}


        internal IEnumerable<TemplateUiInfo> GetCompatibleViews(IApp app, BlockConfiguration blockConfiguration)
	    {
	        IEnumerable<IView> availableTemplates;
	        var items = blockConfiguration.Content;

            // if any items were already initialized...
	        if (items.Any(e => e != null))
	            availableTemplates = GetFullyCompatibleViews(blockConfiguration);

            // if it's only nulls, and only one (no list yet)
	        else if (items.Count <= 1)
	            availableTemplates = GetAll(); 

            // if it's a list of nulls, only allow lists
	        else
	            availableTemplates = GetAll().Where(p => p.UseForList);

	        return availableTemplates.Select(t => new TemplateUiInfo
	        {
	            TemplateId = t.Id,
	            Name = t.Name,
	            ContentTypeStaticName = t.ContentType,
	            IsHidden = t.IsHidden,
	            Thumbnail = TemplateHelpers.GetTemplateThumbnail(app, t.Location, t.Path)
	        });
	    }


        /// <summary>
        /// Get templates which match the signature of possible content-items, presentation etc. of the current template
        /// </summary>
        /// <param name="blockConfiguration"></param>
        /// <returns></returns>
	    private IEnumerable<IView> GetFullyCompatibleViews(BlockConfiguration blockConfiguration)
        {
            var isList = blockConfiguration.Content.Count > 1;

            var compatibleTemplates = GetAll().Where(t => t.UseForList || !isList);
            compatibleTemplates = compatibleTemplates
                .Where(t => blockConfiguration.Content.All(c => c == null) || blockConfiguration.Content.First(e => e != null).Type.StaticName == t.ContentType)
                .Where(t => blockConfiguration.Presentation.All(c => c == null) || blockConfiguration.Presentation.First(e => e != null).Type.StaticName == t.PresentationType)
                .Where(t => blockConfiguration.Header.All(c => c == null) || blockConfiguration.Header.First(e => e != null).Type.StaticName == t.HeaderType)
                .Where(t => blockConfiguration.HeaderPresentation.All(c => c == null) || blockConfiguration.HeaderPresentation.First(e => e != null).Type.StaticName == t.HeaderPresentationType);

            return compatibleTemplates;
        }


        // todo: check if this call could be replaced with the normal ContentTypeController.Get to prevent redundant code
        public IEnumerable<ContentTypeUiInfo> GetContentTypesWithStatus()
        {
            var templates = GetAll().ToList();
            var visible = templates.Where(t => !t.IsHidden).ToList();
            var serializer = new Serializer();

            return App.ContentTypes.FromScope(Settings.AttributeSetScope) 
                .Where(ct => templates.Any(t => t.ContentType == ct.StaticName)) // must exist in at least 1 template
                .OrderBy(ct => ct.Name)
                .Select(ct =>
                {
                    var metadata = ct.Metadata.Description;
                    return new ContentTypeUiInfo {
                        StaticName = ct.StaticName,
                        Name = ct.Name,
                        IsHidden = visible.All(t => t.ContentType != ct.StaticName),   // must check if *any* template is visible, otherise tell the UI that it's hidden
                        Thumbnail = metadata?.GetBestValue(View.TemplateIcon, true)?.ToString(),
                        Metadata = serializer.Prepare(metadata)
                    };
                });
        }



    }

}