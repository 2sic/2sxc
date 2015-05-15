// AngularJS Controller for the Pipeline Designer
pipelineDesigner.controller('PipelineDesignerController',
			['$scope', 'pipelineService', '$location', '$timeout', '$filter', 'uiNotification', 'eavDialogService', '$log', 'eavGlobalConfigurationProvider', '$q',
	function ($scope, pipelineService, $location, $timeout, $filter, uiNotification, eavDialogService, $log, eavGlobalConfigurationProvider, $q) {
		'use strict';

		// Init
		uiNotification.wait();
		$scope.readOnly = true;
		$scope.dataSourcesCount = 0;
		$scope.dataSourceIdPrefix = 'dataSource_';
		$scope.debug = false;

		// Load Pipeline Data
		$scope.PipelineEntityId = $location.search().PipelineId;
		// Stop if no AppId is set
		if (!$location.search().AppId) {
			$timeout(function () {
				uiNotification.error('Please specify an AppId');
			});
			return;
		}
		pipelineService.setAppId($location.search().AppId);

		// Get Data from PipelineService (Web API)
		pipelineService.getPipeline($scope.PipelineEntityId).then(function (success) {
			$scope.pipelineData = success;
			$scope.readOnly = !success.Pipeline.AllowEdit;
			uiNotification.note('Ready', $scope.readOnly ? 'This pipeline is read only' : 'You can now design the Pipeline. \nNote that there are still a few UI bugs.\nVisit 2sxc.org/help for more.', true);

			// If a new Pipeline is made, init new Pipeline
			if (!$scope.PipelineEntityId || $scope.pipelineData.DataSources.length == 1)
				initNewPipeline();
		}, function (reason) {
			uiNotification.error('Loading Pipeline failed', reason);
		});

		// init new jsPlumb Instance
		jsPlumb.ready(function () {
			$scope.jsPlumbInstance = jsPlumb.getInstance({
				Connector: ['Bezier', { curviness: 70 }],
				HoverPaintStyle: {
					lineWidth: 4,
					strokeStyle: "#216477",
					outlineWidth: 2,
					outlineColor: "white"
				},
				PaintStyle: {
					lineWidth: 4,
					strokeStyle: '#61B7CF',
					joinstyle: 'round',
					outlineColor: 'white',
					outlineWidth: 2
				},
				Container: 'pipelineContainer'
			});
			// If connection on Out-DataSource was removed, remove custom Endpoint
			$scope.jsPlumbInstance.bind('connectionDetached', function (info) {
				if (info.targetId == $scope.dataSourceIdPrefix + 'Out') {
					var fixedEndpoints = angular.element(info.target).scope().dataSource.Definition().In;
					var label = info.targetEndpoint.getOverlay('endpointLabel').label;
					if (fixedEndpoints.indexOf(label) == -1) {
						$timeout(function () {
							$scope.jsPlumbInstance.deleteEndpoint(info.targetEndpoint);
						});
					}
				}
			});
			// If a new connection is created, ask for a name of the In-Stream
			$scope.jsPlumbInstance.bind('connection', function (info) {
				if (!$scope.connectionsInitialized) return;

				// Repeat until a valid Stream-Name is provided by the user
				var repeatCount = 0;
				while (true) {
					repeatCount++;

					var promptMessage = "Please name the Stream";
					if (repeatCount > 1)
						promptMessage += ". Ensure the name is not used by any other Stream on this DataSource.";

					var endpointLabel = info.targetEndpoint.getOverlay('endpointLabel');
					var labelPrompt = prompt(promptMessage, endpointLabel.getLabel());
					if (labelPrompt)
						endpointLabel.setLabel(labelPrompt);
					else
						continue;

					// Check if any other Target-Endpoint has the same Stream-Name (Label)
					var endpoints = $scope.jsPlumbInstance.getEndpoints(info.target.id);
					var targetEndpointHavingSameLabel = null;
					angular.forEach(endpoints, function (endpoint) {
						var label = endpoint.getOverlay('endpointLabel').getLabel();
						if (label === labelPrompt && info.targetEndpoint.id !== endpoint.id && angular.element(endpoint.canvas).hasClass("targetEndpoint")) {
							targetEndpointHavingSameLabel = endpoint;
						}
					});
					if (targetEndpointHavingSameLabel)
						continue;

					break;
				}
			});
		});

		// #region jsPlumb Endpoint Definitions
		var getEndpointOverlays = function (isSource) {
			return [['Label', {
				id: 'endpointLabel',
				location: [0.5, isSource ? -0.5 : 1.5],
				label: 'Default',
				cssClass: isSource ? 'endpointSourceLabel' : 'endpointTargetLabel',
				events: {
					dblclick: function (labelOverlay) {
						if ($scope.readOnly) return;

						var newLabel = prompt('Rename Stream', labelOverlay.label);
						if (newLabel)
							labelOverlay.setLabel(newLabel);
					}
				}
			}]];
		}

		// the definition of source endpoints (the small blue ones)
		var sourceEndpoint = {
			paintStyle: { fillStyle: 'transparent', radius: 10, lineWidth: 0 },
			cssClass: "sourceEndpoint",
			maxConnections: -1,
			isSource: true,
			anchor: ['Continuous', { faces: ['top'] }],
			overlays: getEndpointOverlays(true)
		};

		// the definition of target endpoints (will appear when the user drags a connection) 
		var targetEndpoint = {
			paintStyle: { fillStyle: 'transparent', radius: 10, lineWidth: 0 },
			cssClass: "targetEndpoint",
			maxConnections: 1,
			isTarget: true,
			anchor: ['Continuous', { faces: ['bottom'] }],
			overlays: getEndpointOverlays(false),
			dropOptions: { hoverClass: "hover", activeClass: "active" }
		};
		// #endregion

		// make a DataSource with Endpoints, called by the datasource-Directive
		$scope.makeDataSource = function (dataSource, element) {
			// suspend drawing and initialise
			$scope.jsPlumbInstance.doWhileSuspended(function () {
				// Add Out- and In-Endpoints from Definition
				var dataSourceDefinition = dataSource.Definition();
				if (dataSourceDefinition != null) {
					// Add Out-Endpoints
					angular.forEach(dataSourceDefinition.Out, function (name) {
						addEndpoint(element, name, false);
					});
					// Add In-Endpoints
					angular.forEach(dataSourceDefinition.In, function (name) {
						addEndpoint(element, name, true);
					});
					// make the DataSource a Target for new Endpoints (if .In is an Array)
					if (dataSourceDefinition.In) {
						var targetEndpointUnlimited = targetEndpoint;
						targetEndpointUnlimited.maxConnections = -1;
						$scope.jsPlumbInstance.makeTarget(element, targetEndpointUnlimited);
					}

					$scope.jsPlumbInstance.makeSource(element, sourceEndpoint, { filter: '.ep .glyphicon', });
				}

				// make DataSources draggable
				if (!$scope.readOnly) {
					$scope.jsPlumbInstance.draggable(element, {
						grid: [20, 20],
						drag: $scope.dataSourceDrag
					});
				}
			});

			$scope.dataSourcesCount++;
		}

		// Add a jsPlumb Endpoint to an Element
		var addEndpoint = function (element, name, isIn) {
			if (!element.length) {
				$log.error({ message: 'Element not found', selector: element.selector });
				return;
			}

			var dataSource = element.scope().dataSource;
			var uuid = element.attr('id') + (isIn ? '_in_' : '_out_') + name;
			var params = {
				uuid: uuid,
				enabled: !dataSource.ReadOnly || dataSource.EntityGuid == 'Out'	// Endpoints on Out-DataSource must be always enabled
			};
			var endPoint = $scope.jsPlumbInstance.addEndpoint(element, (isIn ? targetEndpoint : sourceEndpoint), params);
			endPoint.getOverlay('endpointLabel').setLabel(name);
		}

		// Initialize jsPlumb Connections once after all DataSources were created in the DOM
		$scope.connectionsInitialized = false;
		$scope.$on('ngRepeatFinished', function () {
			if ($scope.connectionsInitialized) return;

			// suspend drawing and initialise
			$scope.jsPlumbInstance.doWhileSuspended(function () {
				initWirings($scope.pipelineData.Pipeline.StreamWiring);
			});
			$scope.repaint();	// repaint so continuous connections are aligned correctly

			$scope.connectionsInitialized = true;
		});
		var initWirings = function (streamWiring) {
			angular.forEach(streamWiring, function (wire) {
				// read connections from Pipeline
				var sourceElementId = $scope.dataSourceIdPrefix + wire.From;
				var fromUuid = sourceElementId + '_out_' + wire.Out;
				var targetElementId = $scope.dataSourceIdPrefix + wire.To;
				var toUuid = targetElementId + '_in_' + wire.In;

				// Ensure In- and Out-Endpoint exist
				if (!$scope.jsPlumbInstance.getEndpoint(fromUuid))
					addEndpoint(jsPlumb.getSelector('#' + sourceElementId), wire.Out, false);
				if (!$scope.jsPlumbInstance.getEndpoint(toUuid))
					addEndpoint(jsPlumb.getSelector('#' + targetElementId), wire.In, true);

				try {
					$scope.jsPlumbInstance.connect({ uuids: [fromUuid, toUuid] });
				} catch (e) {
					$log.error({ message: 'Connection failed', from: fromUuid, to: toUuid });
				}
			});
		}

		// Init a new Pipeline with DataSources and Wirings from Configuration
		var initNewPipeline = function () {
			angular.forEach(eavGlobalConfigurationProvider.pipelineDesigner.defaultPipeline.dataSources, function (dataSource) {
				$scope.addDataSource(dataSource.partAssemblyAndType, dataSource.visualDesignerData, false, dataSource.entityGuid);
			});

			// Wait until all DataSources were created
			var initWiringsListener = $scope.$on('ngRepeatFinished', function () {
				$scope.connectionsInitialized = false;
				initWirings(eavGlobalConfigurationProvider.pipelineDesigner.defaultPipeline.streamWiring);
				$scope.connectionsInitialized = true;

				initWiringsListener(); // unbind the Listener
			});
		}

		// Add new DataSource
		$scope.addDataSource = function (partAssemblyAndType, visualDesignerData, autoSave, entityGuid) {
			if (!partAssemblyAndType) {
				partAssemblyAndType = $scope.addDataSourceType.PartAssemblyAndType;
				$scope.addDataSourceType = null;
			}
			if (!visualDesignerData)
				visualDesignerData = { Top: 100, Left: 100 };

			var newDataSource = {
				VisualDesignerData: visualDesignerData,
				Name: $filter('typename')(partAssemblyAndType, 'className'),
				Description: '',
				PartAssemblyAndType: partAssemblyAndType,
				EntityGuid: entityGuid || 'unsaved' + ($scope.dataSourcesCount + 1)
			};
			// Extend it with a Property to it's Definition
			newDataSource = angular.extend(newDataSource, pipelineService.getNewDataSource($scope.pipelineData, newDataSource));

			$scope.pipelineData.DataSources.push(newDataSource);

			if (autoSave != false)
				$scope.savePipeline();
		}

		// Delete a DataSource
		$scope.remove = function (index) {
			var dataSource = $scope.pipelineData.DataSources[index];
			if (!confirm('Delete DataSource "' + (dataSource.Name || '(unnamed)') + '"?')) return;
			var elementId = $scope.dataSourceIdPrefix + dataSource.EntityGuid;
			$scope.jsPlumbInstance.selectEndpoints({ element: elementId }).remove();
			$scope.pipelineData.DataSources.splice(index, 1);
		}

		// Edit name of a DataSource
		$scope.editName = function (dataSource) {
			if (dataSource.ReadOnly) return;

			var newName = prompt('Rename DataSource', dataSource.Name);
			if (newName != undefined && newName.trim())
				dataSource.Name = newName;
		}

		// Edit Description of a DataSource
		$scope.editDescription = function (dataSource) {
			if (dataSource.ReadOnly) return;

			var newDescription = prompt('Edit Description', dataSource.Description);
			if (newDescription != undefined && newDescription.trim())
				dataSource.Description = newDescription;
		}

		// Update DataSource Position on Drag
		$scope.dataSourceDrag = function () {
			var $this = $(this);
			var offset = $this.offset();
			var dataSource = $this.scope().dataSource;
			$scope.$apply(function () {
				dataSource.VisualDesignerData.Top = Math.round(offset.top);
				dataSource.VisualDesignerData.Left = Math.round(offset.left);
			});
		}

		// Configure a DataSource
		$scope.configureDataSource = function (dataSource) {
			if (dataSource.ReadOnly) return;

			// Ensure dataSource Entity is saved
			if (!dataSourceIsPersisted(dataSource)) {
				$scope.savePipeline();
				return;
			}

			uiNotification.wait();

			pipelineService.getDataSourceConfigurationUrl(dataSource).then(function (url) {
				uiNotification.clear();
				eavDialogService.open({ url: url, title: 'Configure DataSource ' + dataSource.Name });
			}, function (error) {
				uiNotification.error('Open Configuration UI failed', error);
			});
		}

		// Test wether a DataSource is persisted on the Server
		var dataSourceIsPersisted = function (dataSource) {
			return dataSource.EntityGuid.indexOf('unsaved') == -1;
		}

		// Show/Hide Endpoint Overlays
		$scope.showEndpointOverlays = true;
		$scope.toggleEndpointOverlays = function () {
			$scope.showEndpointOverlays = !$scope.showEndpointOverlays;

			var endpoints = $scope.jsPlumbInstance.selectEndpoints();
			if ($scope.showEndpointOverlays)
				endpoints.showOverlays();
			else
				endpoints.hideOverlays();
		}

		// Edit Pipeline Entity
		$scope.editPipelineEntity = function () {
			// save Pipeline, then open Edit Dialog
			$scope.savePipeline().then(function () {
				eavDialogService.open({
					url: pipelineService.getPipelineUrl('edit', $scope.PipelineEntityId),
					title: 'Edit Test Values',
					onClose: function () {
						pipelineService.getPipeline($scope.PipelineEntityId).then(pipelineSaved);
					}
				});
			});
		}

		// Sync jsPlumb Connections and StreamsOut to the pipelineData-Object
		var syncPipelineData = function () {
			var connectionInfos = [];
			angular.forEach($scope.jsPlumbInstance.getAllConnections(), function (connection) {
				connectionInfos.push({
					From: connection.sourceId.substr($scope.dataSourceIdPrefix.length),
					Out: connection.endpoints[0].getOverlay('endpointLabel').label,
					To: connection.targetId.substr($scope.dataSourceIdPrefix.length),
					In: connection.endpoints[1].getOverlay('endpointLabel').label
				});
			});
			$scope.pipelineData.Pipeline.StreamWiring = connectionInfos;

			var streamsOut = [];
			$scope.jsPlumbInstance.selectEndpoints({ target: $scope.dataSourceIdPrefix + 'Out' }).each(function (endpoint) {
				streamsOut.push(endpoint.getOverlay('endpointLabel').label);
			});
			$scope.pipelineData.Pipeline.StreamsOut = streamsOut.join(',');
		}

		// #region Save Pipeline
		// Save Pipeline
		// returns a Promise about the saving state
		$scope.savePipeline = function (successHandler) {
			uiNotification.wait('Saving...');
			$scope.readOnly = true;

			syncPipelineData();

			var deferred = $q.defer();

			if (typeof successHandler == 'undefined')	// set default success Handler
				successHandler = pipelineSaved;

			pipelineService.savePipeline($scope.pipelineData.Pipeline, $scope.pipelineData.DataSources).then(successHandler, function (reason) {
				uiNotification.error('Save Pipeline failed', reason);
				$scope.readOnly = false;
				deferred.reject();
			}).then(function () {
				deferred.resolve();
			});

			return deferred.promise;
		}

		// Handle Pipeline Saved, success contains the updated Pipeline Data
		var pipelineSaved = function (success) {
			// Update PipelineData with data retrieved from the Server
			$scope.pipelineData.Pipeline = success.Pipeline;
			$scope.PipelineEntityId = success.Pipeline.EntityId /*EntityId*/;
			$location.search('PipelineId', success.Pipeline.EntityId /*EntityId*/);
			$scope.readOnly = !success.Pipeline.AllowEdit;
			$scope.pipelineData.DataSources = success.DataSources;
			pipelineService.postProcessDataSources($scope.pipelineData);

			uiNotification.success('Saved', 'Pipeline ' + success.Pipeline.EntityId /*EntityId*/ + ' saved and loaded', true);

			// Reset jsPlumb, re-Init Connections
			$scope.jsPlumbInstance.reset();
			$scope.connectionsInitialized = false;
		};
		// #endregion

		// Repaint jsPlumb
		$scope.repaint = function () {
			$scope.jsPlumbInstance.repaintEverything();
		}

		// Show/Hide Debug info
		$scope.toogleDebug = function () {
			$scope.debug = !$scope.debug;
		}

		// Query the Pipeline
		$scope.queryPipeline = function () {
			var query = function () {
				// Query pipelineService for the result...
				uiNotification.wait('Running Query ...');

				pipelineService.queryPipeline($scope.PipelineEntityId).then(function (success) {
					// Show Result in a UI-Dialog
					uiNotification.clear();
					eavDialogService.open({
						title: 'Query result',
						content: '<div><div>The Full result was logged to the Browser Console. Further down you\'ll find more debug-infos. </div>'
                            + '<h3>Parameters used</h3><div>' + ($scope.pipelineData.Pipeline.TestParameters.length > 5 ? $scope.pipelineData.Pipeline.TestParameters.replace('\n', '<br>') : 'no test params specified') + '</div> '
                            + '<h3>Query result</h3><div> <pre id="pipelineQueryResult">' + $filter('json')(success.Query) + '</pre>' + showConnectionTable(success) + '</div>'
                            + '</div'
					});
					$timeout(function () {
						showEntityCountOnStreams(success);
					});
					$log.debug(success);
				}, function (reason) {
					uiNotification.error('Query failed', reason);
				});
			};

			// Create html-table with connection debug-info
			var showConnectionTable = function (result) {
				var srcTbl = '<h3>Sources</h3>' +
			        '<table><tr><th>Guid</th><th>Type</th><th>Config</th></tr>';
				var src = result.Sources;
				for (var s in src) {
					if (s[0] != '$') {
						srcTbl += "<tr><td><pre>" + s.substring(0, 13) + "...</pre></td><td>" + src[s].Type + "</td><td>";
						var cnf = src[s].Configuration;
						for (var c in cnf)
							if (c[0] != '$')
								srcTbl += '<b>' + c + '</b>' + "=" + cnf[c] + '</br>';
						srcTbl += "</td></tr>";
					}
				}
				srcTbl += "</table>";


				srcTbl += "<h3>Streams</h3>" +
		            "<table><tr><th>Source</th><th>Target</th><th>Items</th><th>Err</th></tr>";
				src = result.Streams;
				for (var s in src) {
					if (s[0] != '$') {
						srcTbl += "<tr><td><pre>"
                            + src[s].Source.substring(0, 13) + ":" + src[s].SourceOut + "</pre></td><td><pre>"
                            + src[s].Target.substring(0, 13) + ":" + src[s].TargetIn + "</pre></td><td>"
                            + src[s].Count + "</td><td>" +
                            +src[s].Error + "</td></tr>";
					}
				}
				srcTbl += "</table>";

				return srcTbl;
			};

			var showEntityCountOnStreams = function (result) {
				angular.forEach(result.Streams, function (stream) {
					// Find jsPlumb Connection for the current Stream
					var sourceElementId = $scope.dataSourceIdPrefix + stream.Source;
					var targetElementId = $scope.dataSourceIdPrefix + stream.Target;
					if (stream.Target === "00000000-0000-0000-0000-000000000000")
						targetElementId = $scope.dataSourceIdPrefix + "Out";

					var fromUuid = sourceElementId + '_out_' + stream.SourceOut;
					var toUuid = targetElementId + '_in_' + stream.TargetIn;

					var sourceEndpoint = $scope.jsPlumbInstance.getEndpoint(fromUuid);
					angular.forEach(sourceEndpoint.connections, function (connection) {
						if (connection.endpoints[1].getUuid() === toUuid) {
							// when connection found, update it's label with the Entities-Count
							connection.setLabel({
								label: stream.Count.toString(),
								cssClass: "streamEntitiesCount"
							});
						}
					});
				});
			}

			// Ensure the Pipeline is saved
			$scope.savePipeline().then(query);
		}

		// Clone the Pipeline
		$scope.clonePipeline = function () {
			if (!confirm('Clone Pipeline ' + $scope.PipelineEntityId + '?')) return;

			// Clone and get new PipelineEntityId
			var clone = function () {
				return pipelineService.clonePipeline($scope.PipelineEntityId);
			};
			// Get the new Pipeline (Pipeline and DataSources)
			var getClonePipeline = function (success) {
				return pipelineService.getPipeline(success.EntityId /*EntityId*/);
			};

			// Save, clone, get clone, load clone
			$scope.savePipeline(null).then(clone).then(getClonePipeline).then(pipelineSaved);
		}
	}]);
