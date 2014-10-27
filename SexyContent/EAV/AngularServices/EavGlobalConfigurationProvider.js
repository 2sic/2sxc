// EavGlobalConfigurationProvider providers default global values for the EAV angular system
// The ConfigurationProvider in 2sxc is not the same as in the EAV project.
angular.module('eavGlobalConfigurationProvider', [])
	.factory('eavGlobalConfigurationProvider', function ($location) {

		// Get needed moduleContext variables from parent node
		var globals = $.parseJSON($("[data-2sxc-globals]").attr("data-2sxc-globals"));

		var getApiAdditionalHeaders = function () {
			var sf = $.ServicesFramework(globals.ModuleContext.ModuleId);

			return {
				ModuleId: sf.getModuleId(),
				TabId: sf.getTabId(),
				RequestVerificationToken: sf.getAntiForgeryValue()
			};
		}

		var baseUrl = globals.FullUrl + "/mid/" + globals.ModuleContext.ModuleId + "/popUp/true?AppID=" + globals.ModuleContext.AppId + "&";
		var itemFormBaseUrl = baseUrl + "ctl=EavManagement&";

		return {
			api: {
				baseUrl: globals.ApplicationPath + "DesktopModules/2sxc/API",
				additionalHeaders: getApiAdditionalHeaders()
			},
			//defaultApiParams: {
			//	portalId: globals.ModuleContext.PortalId,
			//	moduleId: globals.ModuleContext.ModuleId,
			//	tabId: globals.ModuleContext.TabId
			//},
			dialogClass: "dnnFormPopup",
			itemForm: {
				newItemUrl: baseUrl + 'ManagementMode=NewItem&AttributeSetId=[AttributeSetId]&CultureDimension=[CultureDimension]&KeyNumber=[KeyNumber]&KeyGuid=[KeyGuid]&AssignmentObjectTypeId=[AssignmentObjectTypeId]',
				editItemUrl: baseUrl + "ManagementMode=EditItem&EntityId=[EntityId]&CultureDimension=[CultureDimension]",
				getUrl: function (mode, params) {
					angular.extend(params, { ManagementMode: mode + 'Item' });
					if (!params.ReturnUrl)
						params.ReturnUrl = $location.url();
					return itemFormBaseUrl + $.param(params);
				}
			},
			pipelineDesigner: {
				getUrl: function (appId, pipelineId) {
					return '/Pages/PipelineDesigner.aspx?AppId=' + appId + '&PipelineId=' + pipelineId;
				},
				outDataSource: {
					className: 'SexyContentTemplate',
					in: ['Content', 'ListContent', 'Presentation', 'ListPresentation'],
					name: '2SexyContent Module',
					description: 'The module/template which will show this data',
					visualDesignerData: { Top: 50, Left: 410 }
				},
				defaultPipeline: {
					dataSources: [
						{
							partAssemblyAndType: 'ToSic.SexyContent.DataSources.ModuleDataSource, ToSic.SexyContent',
							visualDesignerData: { Top: 400, Left: 450 }
						}
					],
					streamWiring: [
						{ From: 'unsaved1', Out: 'Content', To: 'Out', In: 'Content' },
						{ From: 'unsaved1', Out: 'ListContent', To: 'Out', In: 'ListContent' },
						{ From: 'unsaved1', Out: 'Presentation', To: 'Out', In: 'Presentation' },
						{ From: 'unsaved1', Out: 'ListPresentation', To: 'Out', In: 'ListPresentation' }
					]
				}
			}
		}
	});