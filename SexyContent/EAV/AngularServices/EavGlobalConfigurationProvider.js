// EavGlobalConfigurationProvider providers default global values for the EAV angular system
// The ConfigurationProvider in 2sxc is not the same as in the EAV project.
angular.module('eavGlobalConfigurationProvider', [])
	.factory('eavGlobalConfigurationProvider', function ($location) {

		// Get DNN ModuleContext
		var globals = $("div[data-2sxc-globals]").data("2sxc-globals");

		var getApiAdditionalHeaders = function () {
			var sf = $.ServicesFramework(globals.ModuleContext.ModuleId);

			return {
				ModuleId: sf.getModuleId(),
				TabId: sf.getTabId(),
				RequestVerificationToken: sf.getAntiForgeryValue()
			};
		}

		var baseUrl = globals.FullUrl + "/mid/" + globals.ModuleContext.ModuleId + "/popUp/true?AppId=" + globals.ModuleContext.AppId + "&";
		var itemFormBaseUrl = baseUrl + "ctl=editcontentgroup&";

		return {
			api: {
				baseUrl: globals.ApplicationPath + "DesktopModules/2sxc/API",
				additionalHeaders: getApiAdditionalHeaders(),
				defaultParams: {
					portalId: globals.ModuleContext.PortalId,
					moduleId: globals.ModuleContext.ModuleId,
					tabId: globals.ModuleContext.TabId
				}
			},
			dialogClass: "dnnFormPopup",
			itemForm: {
				newItemUrl: itemFormBaseUrl + 'ManagementMode=NewItem&AttributeSetId=[AttributeSetId]&CultureDimension=[CultureDimension]&KeyNumber=[KeyNumber]&KeyGuid=[KeyGuid]&AssignmentObjectTypeId=[AssignmentObjectTypeId]',
				editItemUrl: itemFormBaseUrl + "ManagementMode=EditItem&EntityId=[EntityId]&CultureDimension=[CultureDimension]",
				getUrl: function (mode, params) {
					if (mode == 'New')
						angular.extend(params, { editMode: 'New' });
					if (!params.ReturnUrl)
						params.ReturnUrl = $location.url();
					return itemFormBaseUrl + $.param(params);
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
				}
			}
		}
	});