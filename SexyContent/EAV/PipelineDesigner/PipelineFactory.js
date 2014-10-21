// PipelineFactory provides an interface to the Server Backend storing Pipelines and their Pipeline Parts
pipelineDesigner.factory('pipelineFactory', ['$resource', '$q', '$filter', 'eavGlobalConfigurationProvider', function ($resource, $q, $filter, eavGlobalConfigurationProvider) {
	'use strict';

	// Web API Service
	var pipelineResource = $resource(eavGlobalConfigurationProvider.apiBaseUrl + '/EAV/PipelineDesigner/:action');

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
		// Get the URL
		getDataSourceConfigurationUrl: function (appId, dataSource) {
			return pipelineResource.get({
				action: 'GetDataSourceConfigurationUrl',
				appId: appId,
				dataSourceEntityGuid: dataSource.EntityGuid,
				dataSourceFullName: $filter('typename')(dataSource.PartAssemblyAndType, 'classFullName'),
				newItemUrl: eavGlobalConfigurationProvider.itemForm.newItemUrl + '&PreventRedirect=true',
				editItemUrl: eavGlobalConfigurationProvider.itemForm.editItemUrl + '&PreventRedirect=true'
			}).$promise;
		},
		// Query the Data of a Pipeline
		queryPipeline: function (appId, id) {
			return pipelineResource.get({ action: 'QueryPipeline', appId: appId, id: id }).$promise;
		}
	}
}]);