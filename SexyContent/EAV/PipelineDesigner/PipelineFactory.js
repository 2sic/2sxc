// PipelineFactory provides an interface to the Server Backend storing Pipelines and their Pipeline Parts
pipelineDesigner.factory('pipelineFactory', ['$resource', '$q', '$filter', 'eavGlobalConfigurationProvider', '$http', function ($resource, $q, $filter, eavGlobalConfigurationProvider, $http) {
	'use strict';

	// Web API Service
	var pipelineResource = $resource(eavGlobalConfigurationProvider.api.baseUrl + '/EAV/PipelineDesigner/:action');
	var entitiesResource = $resource(eavGlobalConfigurationProvider.api.baseUrl + '/EAV/Entities/:action');
	// Add additional Headers to each http-Request
	angular.extend($http.defaults.headers.common, eavGlobalConfigurationProvider.api.additionalHeaders);


	// Get the Definition of a DataSource
	var getDataSourceDefinitionProperty = function (model, dataSource) {
		return $filter('filter')(model.InstalledDataSources, function (d) { return d.PartAssemblyAndType == dataSource.PartAssemblyAndType; })[0];
	};

	// Extend Pipeline-Model retrieved from the Server
	var postProcessDataSources = function (model) {
		// Append Out-DataSource for the UI
		model.DataSources.push({
			Name: eavGlobalConfigurationProvider.pipelineDesigner.outDataSource.name,
			Description: eavGlobalConfigurationProvider.pipelineDesigner.outDataSource.description,
			EntityGuid: 'Out',
			PartAssemblyAndType: eavGlobalConfigurationProvider.pipelineDesigner.outDataSource.className,
			VisualDesignerData: eavGlobalConfigurationProvider.pipelineDesigner.outDataSource.visualDesignerData,
			ReadOnly: true
		});

		// Extend each DataSource with Definition-Property and ReadOnly Status
		angular.forEach(model.DataSources, function (dataSource) {
			dataSource.Definition = function () { return getDataSourceDefinitionProperty(model, dataSource); }
			dataSource.ReadOnly = dataSource.ReadOnly || !model.Pipeline.AllowEdit;
		});
	};

	return {
		// get a Pipeline with Pipeline Info with Pipeline Parts and Installed DataSources
		getPipeline: function (pipelineEntityId, appId) {
			var deferred = $q.defer();

			var getPipeline = pipelineResource.get({ action: 'GetPipeline', id: pipelineEntityId, appId: appId });
			var getInstalledDataSources = pipelineResource.query({ action: 'GetInstalledDataSources' });

			// Join and modify retrieved Data
			$q.all([getPipeline.$promise, getInstalledDataSources.$promise]).then(function (results) {
				var model = JSON.parse(angular.toJson(results[0]));	// workaround to remove AngularJS Promise from the result-Objects
				model.InstalledDataSources = JSON.parse(angular.toJson(results[1]));

				// Init new Pipeline Object
				if (!pipelineEntityId) {
					model.Pipeline = {
						AllowEdit: 'True'
					};
				}

				// Add Out-DataSource for the UI
				model.InstalledDataSources.push({
					PartAssemblyAndType: eavGlobalConfigurationProvider.pipelineDesigner.outDataSource.className,
					ClassName: eavGlobalConfigurationProvider.pipelineDesigner.outDataSource.className,
					In: eavGlobalConfigurationProvider.pipelineDesigner.outDataSource.in,
					Out: null,
					allowNew: false
				});

				postProcessDataSources(model);

				deferred.resolve(model);
			}, function (reason) {
				deferred.reject(reason);
			});

			return deferred.promise;
		},
		// Ensure Model has all DataSources and they're linked to their Definition-Object
		postProcessDataSources: function (model) {
			// stop Post-Process if the model already contains the Out-DataSource
			if ($filter('filter')(model.DataSources, function (d) { return d.EntityGuid == 'Out'; })[0])
				return;

			postProcessDataSources(model);
		},
		// Get a JSON for a DataSource with Definition-Property
		getNewDataSource: function (model, dataSourceBase) {
			return { Definition: function () { return getDataSourceDefinitionProperty(model, dataSourceBase); } }
		},
		// Save whole Pipline
		savePipeline: function (appId, pipeline, dataSources) {
			if (!appId)
				return $q.reject('AppId must be set to save a Pipeline');

			// Remove some Properties from the DataSource before Saving
			var dataSourcesPrepared = [];
			angular.forEach(dataSources, function (dataSource) {
				var dataSourceClone = angular.copy(dataSource);
				delete dataSourceClone.ReadOnly;
				dataSourcesPrepared.push(dataSourceClone);
			});

			return pipelineResource.save({ action: 'SavePipeline', appId: appId, Id: pipeline.EntityId }, { pipeline: pipeline, dataSources: dataSourcesPrepared }).$promise;
		},
		// clone a whole Pipeline
		clonePipeline: function (appId, pipelineEntityId) {
			return pipelineResource.get({ action: 'ClonePipeline', appId: appId, Id: pipelineEntityId }).$promise;
		},
		// Get the URL to configure a DataSource
		getDataSourceConfigurationUrl: function (appId, dataSource) {
			var dataSourceFullName = $filter('typename')(dataSource.PartAssemblyAndType, 'classFullName');
			var contentTypeName = '|Config ' + dataSourceFullName;
			var assignmentObjectTypeId = 4;
			var keyGuid = dataSource.EntityGuid;
			var preventRedirect = true;

			var deferred = $q.defer();

			// Query for existing Entity
			entitiesResource.query({ action: 'GetAssignedEntities', appId: appId, assignmentObjectTypeId: assignmentObjectTypeId, keyGuid: keyGuid, contentTypeName: contentTypeName }, function (success) {
				if (success.length) // Edit existing Entity
					deferred.resolve(eavGlobalConfigurationProvider.itemForm.getEditItemUrl(success[0].EntityId, null, preventRedirect));
				else { // Create new Entity
					entitiesResource.get({ action: 'GetContentType', appId: appId, name: contentTypeName }, function (contentType) {
						// test for null-response
						if (contentType[0] == 'n' && contentType[1] == 'u' && contentType[2] == 'l' && contentType[3] == 'l')
							deferred.reject('Content Type ' + contentTypeName + ' not found.');
						else
							deferred.resolve(eavGlobalConfigurationProvider.itemForm.getNewItemUrl(contentType.AttributeSetId, assignmentObjectTypeId, { KeyGuid: keyGuid, ReturnUrl: null }, preventRedirect));
					}, function (reason) {
						deferred.reject(reason);
					});
				}
			}, function (reason) {
				deferred.reject(reason);
			});

			return deferred.promise;
		},
		// Query the Data of a Pipeline
		queryPipeline: function (appId, id) {
			return pipelineResource.get({ action: 'QueryPipeline', appId: appId, id: id }).$promise;
		}
	}
}]);