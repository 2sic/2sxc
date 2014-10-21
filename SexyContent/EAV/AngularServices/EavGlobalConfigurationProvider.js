// EavGlobalConfigurationProvider providers default global values for the EAV angular system
// The ConfigurationProvider in 2sxc is not the same as in the EAV project.
angular.module('eavGlobalConfigurationProvider', [])
	.factory('eavGlobalConfigurationProvider', function () {

		// Get needed moduleContext variables from parent node
		var globals = $.parseJSON($("[data-2sxc-globals]").attr("data-2sxc-globals"));

		return {
			apiBaseUrl: globals.ApplicationPath + "DesktopModules/2sxc/API",
			defaultApiParams: {
				portalId: globals.ModuleContext.PortalId,
				moduleId: globals.ModuleContext.ModuleId,
				tabId: globals.ModuleContext.TabId
			},
			dialogClass: "dnnFormPopup",
			itemForm: {
				newItemUrl: "/EAVManagement.aspx?ManagementMode=NewItem&AttributeSetId=[AttributeSetId]&CultureDimension=[CultureDimension]&KeyNumber=[KeyNumber]&KeyGuid=[KeyGuid]&AssignmentObjectTypeId=[AssignmentObjectTypeId]",
				editItemUrl: "/EAVManagement.aspx?ManagementMode=EditItem&EntityId=[EntityId]&CultureDimension=2"
			},
			pipelineDesigner: {
				outDataSource: {
					className: 'SexyContentTemplate',
					in: ['Content', 'Presentation', 'ListContent', 'ListPresentation'],
					name: '2SexyContent Module',
					description: 'The module/template which will show this data',
					visualDesignerData: { Top: 50, Left: 410 }
				},
				defaultPipeline: {
					dataSources: [
						{
							partAssemblyAndType: 'ToSic.Eav.DataSources.App, ToSic.Eav',
							visualDesignerData: { Top: 400, Left: 450 }
						}
					],
					streamWiring: [
						{ From: 'unsaved1', Out: 'Default', To: 'Out', In: 'Content' },
						{ From: 'unsaved1', Out: 'Default', To: 'Out', In: 'ListContent' },
						{ From: 'unsaved1', Out: 'Default', To: 'Out', In: 'Presentation' },
						{ From: 'unsaved1', Out: 'Default', To: 'Out', In: 'ListPresentation' }
					]
				}
			}
		}
	});