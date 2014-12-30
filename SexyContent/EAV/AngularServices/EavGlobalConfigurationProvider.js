// EavGlobalConfigurationProvider providers default global values for the EAV angular system
// The ConfigurationProvider in 2sxc is not the same as in the EAV project.
angular.module('eavGlobalConfigurationProvider', []).factory('eavGlobalConfigurationProvider', function ($location) {

	// Get DNN ModuleContext
	var globals = $('div[data-2sxc-globals]').data('2sxc-globals');
	if (!globals)
		alert('Please ensure the DNN-Page is in Edit-Mode');

	var getApiAdditionalHeaders = function () {
		var sf = $.ServicesFramework(globals.ModuleContext.ModuleId);

		return {
			ModuleId: sf.getModuleId(),
			TabId: sf.getTabId(),
			RequestVerificationToken: sf.getAntiForgeryValue()
		};
	}

	var baseUrl = globals.FullUrl + '?mid=' + globals.ModuleContext.ModuleId + '&popUp=true&AppId=' + globals.ModuleContext.AppId + '&';

	var getItemFormUrl = function (mode, params, preventRedirect) {
		if (mode == 'New')
			params.editMode = 'New';
		if (!params.ReturnUrl)
			params.ReturnUrl = $location.url();
		if (preventRedirect)
			params.PreventRedirect = true;
		if (typeof globals.DefaultLanguageID == 'number')
			params.cultureDimension = globals.DefaultLanguageID;
		return baseUrl + 'ctl=editcontentgroup&' + $.param(params);
	};

	return {
		api: {
			baseUrl: globals.ApplicationPath + 'DesktopModules/2sxc/API',
			additionalHeaders: getApiAdditionalHeaders(),
			defaultParams: {
				portalId: globals.ModuleContext.PortalId,
				moduleId: globals.ModuleContext.ModuleId,
				tabId: globals.ModuleContext.TabId
			}
		},
		dialogClass: 'dnnFormPopup',
		itemForm: {
			getNewItemUrl: function (attributeSetId, assignmentObjectTypeId, params, preventRedirect, prefill) {
				if (prefill)
					params.prefill = JSON.stringify(prefill);
				return getItemFormUrl('New', angular.extend({ AttributeSetId: attributeSetId, AssignmentObjectTypeId: assignmentObjectTypeId }, params), preventRedirect);
			},
			getEditItemUrl: function (entityId, params, preventRedirect) {
				return getItemFormUrl('Edit', angular.extend({ EntityId: entityId }, params), preventRedirect);
			}
		},
		pipelineDesigner: {
			getUrl: function (appId, pipelineId) {
				return baseUrl + 'ctl=pipelinedesigner&PipelineId=' + pipelineId;
			},
			outDataSource: {
				className: 'SexyContentTemplate',
				in: ['ListPresentation', 'Presentation', 'ListContent', 'Default'],
				name: '2SexyContent Module',
				description: 'The module/template which will show this data',
				visualDesignerData: { Top: 20, Left: 420 }
			},
			defaultPipeline: {
				dataSources: [
					{
						entityGuid: 'unsaved1',
						partAssemblyAndType: 'ToSic.Eav.DataSources.Caches.ICache, ToSic.Eav',
						visualDesignerData: { Top: 800, Left: 440 }
					},
					{
						entityGuid: 'unsaved2',
						partAssemblyAndType: 'ToSic.Eav.DataSources.PublishingFilter, ToSic.Eav',
						visualDesignerData: { Top: 620, Left: 440 }
					},
					{
						entityGuid: 'unsaved3',
						partAssemblyAndType: 'ToSic.SexyContent.DataSources.ModuleDataSource, ToSic.SexyContent',
						visualDesignerData: { Top: 440, Left: 440 }
					}
				],
				streamWiring: [
					{ From: 'unsaved1', Out: 'Default', To: 'unsaved2', In: 'Default' },
					{ From: 'unsaved1', Out: 'Drafts', To: 'unsaved2', In: 'Drafts' },
					{ From: 'unsaved1', Out: 'Published', To: 'unsaved2', In: 'Published' },
					{ From: 'unsaved2', Out: 'Default', To: 'unsaved3', In: 'Default' },
					{ From: 'unsaved3', Out: 'ListPresentation', To: 'Out', In: 'ListPresentation' },
					{ From: 'unsaved3', Out: 'ListContent', To: 'Out', In: 'ListContent' },
					{ From: 'unsaved3', Out: 'Presentation', To: 'Out', In: 'Presentation' },
					{ From: 'unsaved3', Out: 'Default', To: 'Out', In: 'Default' }
				]
			},
			testParameters: '[Module:ModuleID]=' + globals.ModuleContext.ModuleId
		},
		assignmentObjectTypeIdDataPipeline: 4
	}
});