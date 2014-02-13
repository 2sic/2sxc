;
(function() {

    // todo 2dm
    // enable dynamic add in-connections (drop on box) + corresponding delete-connections
    // improve labels for connections (mouseover?)
    // improve label on arrow (Default>Default?)
    // provide save-method posting state (positions, connections)


    window.jsPlumbDemo = {
        init: function() {

            jsPlumb.importDefaults({
                // default drag options
                DragOptions: { cursor: 'pointer', zIndex: 2000 },
                // default to blue at one end and green at the other
                EndpointStyles: [{ fillStyle: '#225588' }, { fillStyle: '#558822' }],
                // blue endpoints 7 px; green endpoints 11.
                Endpoints: [["Dot", { radius: 7 }], ["Dot", { radius: 11 }]],
                // the overlays to decorate each connection with.  note that the label overlay uses a function to generate the label text; in this
                // case it returns the 'labelText' member that we set on each connection in the 'init' method below.
                ConnectionOverlays: [
                    ["Arrow", { location: 0.9 }]
                    /*["Label", {
                        location: 0.5,
                        id: "label",
                        cssClass: "aLabel"
                    }]*/
                ]
            });


            // this is the paint style for the connecting lines..
            var connectorPaintStyle = {
                lineWidth: 5,
                strokeStyle: "#deea18",
                joinstyle: "round",
                outlineColor: "#EAEDEF",
                outlineWidth: 7
            },
                // .. and this is the hover style. 
                connectorHoverStyle = {
                    lineWidth: 7,
                    strokeStyle: "#2e2aF8"
                },
                // the label
                sourceEndpointOverlay = {
                    location: [0.5, 1.5],
                    label: "Dragx",
                    cssClass: "endpointSourceLabel"
                },
                dynEndpointOverlay = function(label, isTarget) {
                    return {
                        location: [0.5, (isTarget ? -0.5 : 1.5)],
                        label: label,
                        cssClass: "endpoint" + (isTarget ? "Target" : "Source") + "Label"
                    }
                },
                // the definition of source endpoints (the small blue ones)
                sourceEndpoint = {
                    endpoint: "Dot",
                    paintStyle: { fillStyle: "#225588", radius: 7 },
                    isSource: true,
                    connector: ["Flowchart", { stub: [30, 30], gap: 10 }], // "Straight", // [ "Flowchart", { stub:[40, 60], gap:10 } ],                                                                     
                    connectorStyle: connectorPaintStyle,
                    hoverPaintStyle: connectorHoverStyle,
                    connectorHoverStyle: connectorHoverStyle,
                    dragOptions: {},
                    maxConnections: -1,
                    //                overlays:[
                //                  [ "Label", dynEndpointOverlay("from", false)
                ////                    { 
                ////                       location:[0.5, 1.5], 
                ////                       label:"Drag",
                ////                       cssClass:"endpointSourceLabel" 
                ////                   } 
                //                    ]
                //                ]
                },
                // a source endpoint that sits at BottomCenter
            //     bottomSource = jsPlumb.extend( { anchor:"BottomCenter" }, sourceEndpoint),
     // the definition of target endpoints (will appear when the user drags a connection) 
                targetEndpoint = {
                    endpoint: "Dot",
                    paintStyle: { fillStyle: "#558822", radius: 11 },
                    hoverPaintStyle: connectorHoverStyle,
                    maxConnections: 1,
                    dropOptions: { hoverClass: "hover", activeClass: "active" },
                    isTarget: true,
                    //                overlays:[
                //                  [ "Label", dynEndpointOverlay("to", true) ] //{ location:[0.5, -0.5], label:"Drop", cssClass:"endpointTargetLabel" } ]
                //                ]
                },
                init = function(connection) {
                    /*connection.getOverlay("label").setLabel(connection.sourceId.substring(6) + "-" + connection.targetId.substring(6));*/
                    connection.bind("editCompleted", function(o) {
                        if (typeof console != "undefined")
                            console.log("connection edited. path is now ", o.path);
                    });
                };

            var allSourceEndpoints = [], allTargetEndpoints = [];
            //                         _addEndpoints = function(toId, sourceAnchors, targetAnchors) {
            //                                for (var i = 0; i < sourceAnchors.length; i++) {
            //                                      var sourceUUID = toId + sourceAnchors[i];
            //                                      allSourceEndpoints.push(jsPlumb.addEndpoint(toId, sourceEndpoint, { anchor:sourceAnchors[i], uuid:sourceUUID, overlays:[["Label", dynEndpointOverlay("Default", false)]] }));                                    
            //                                }
            //                                for (var j = 0; j < targetAnchors.length; j++) {
            //                                      var targetUUID = toId + targetAnchors[j];
            //                                      allTargetEndpoints.push(jsPlumb.addEndpoint(toId, targetEndpoint, { anchor:targetAnchors[j], uuid:targetUUID }));                                        
            //                                }
            //                         };

            //                  _addEndpoints("window4", ["TopCenter", "BottomCenter"], ["LeftMiddle", "RightMiddle"]);                  
            //                  _addEndpoints("window2", ["LeftMiddle", "BottomCenter"], ["TopCenter", "RightMiddle"]);
            //                  _addEndpoints("window3", ["RightMiddle", "BottomCenter"], ["LeftMiddle", "TopCenter"]);
            //                  _addEndpoints("window1", ["LeftMiddle", "RightMiddle"], ["TopCenter", "BottomCenter"]);

            // listen for new connections; initialise them the same way we initialise the connections at startup.
            jsPlumb.bind("jsPlumbConnection", function(connInfo, originalEvent) {
                init(connInfo.connection);
            });

            // make all the window divs draggable                                     
            jsPlumb.draggable(jsPlumb.getSelector(".window"), { grid: [20, 20] });
            // THIS DEMO ONLY USES getSelector FOR CONVENIENCE. Use your library's appropriate selector method!
            //jsPlumb.draggable(jsPlumb.getSelector(".window"));


            // connect a few up
            //                  jsPlumb.connect({uuids:["window2BottomCenter", "window3TopCenter"], editable:true});
            //                  jsPlumb.connect({uuids:["window2LeftMiddle", "window4LeftMiddle"], editable:true});
            //                  jsPlumb.connect({uuids:["window4TopCenter", "window4RightMiddle"], editable:true});
            //                  jsPlumb.connect({uuids:["window3RightMiddle", "window2RightMiddle"], editable:true});
            //                  jsPlumb.connect({uuids:["window4BottomCenter", "window1TopCenter"], editable:true});
            //                  jsPlumb.connect({uuids:["window3BottomCenter", "window1BottomCenter"], editable:true});
            //


            // 2dm code
            var maxX = 100, maxY = 100, minWidth = 12, streamWidth = 6;
            var screenX = 60, screenY = 40;
            var DataSources;
            var ConnectionList;

            if (window.location.href.toLowerCase().indexOf("datasourceid=1") != -1) {
                DataSources = [
                    { sourceid: 17, name: "MainSource", description: "", type: "ICache", out: ["Default"], in: [], posx: 50, posy: 100 },
                    //{ sourceid: 18, name: "Category Filter", description: "", type: "EntityTypeFilter", out: ["Default"], in: ["Default"], posx: 75, posy: 70 },
                    { sourceid: 19, name: "Module List Source", description: "Default source for Templates", type: "ToSic...ModuleDataProvider", out: ["Default", "DefaultPresentation", "Header", "HeaderPresentation"], in: ["Default"], posx: 50, posy: 50 },
                    { sourceid: 20, name: "2SexyContent Module", description: "The module/template which will show this data", type: "SexyContentTemplate", out: [], in: ["Default", "DefaultPresentation", "Header", "HeaderPresentation"], posx: 50, posy: 5 }
                ];
                ConnectionList = [
                    { from: "17:Default", to: "19:Default" },
                    //{ from: "17:Default", to: "18:Default" },
                    { from: "19:Default", to: "20:Default" },
                    { from: "19:DefaultPresentation", to: "20:DefaultPresentation" },
                    { from: "19:Header", to: "20:Header" },
                    { from: "19:HeaderPresentation", to: "20:HeaderPresentation" }
                ];
            } else {
                DataSources = [
                    { sourceid: 17, name: "MainSource", description: "", type: "ICache", out: ["Default"], in: [], posx: 50, posy: 110 },
                    { sourceid: 18, name: "QuestionsFilter", description: "", type: "EntitiesFromList", out: ["Default"], in: ["Default"], posx: 55, posy: 58, configurationEntityId: 1047 },
                    { sourceid: 21, name: "SubCategoriesFilter", description: "", type: "EntitiesFromList", out: ["Default"], in: ["Default"], posx: 140, posy: 60, configurationEntityId: 1050 },
                    { sourceid: 19, name: "CategoriesFilter", description: "Default source for Templates", type: "ToSic...ModuleDataProvider", out: ["Default"], in: ["Default"], posx: 90, posy: 50, configurationEntityId: 1051 },
                    { sourceid: 22, name: "Module List Source", description: "Default source for Templates", type: "ToSic...ModuleDataProvider", out: ["Default", "DefaultPresentation", "Header", "HeaderPresentation"], in: ["Default"], posx: 2, posy: 60 },
                    { sourceid: 20, name: "2SexyContent Module", description: "The module/template which will show this data", type: "SexyContentTemplate", out: [], in: ["Default", "DefaultPresentation", "Header", "HeaderPresentation", "Questions", "Categories", "SubCategories"], posx: 27, posy:1 }
                ];
                ConnectionList = [
                    { from: "17:Default", to: "18:Default" },
                    { from: "17:Default", to: "21:Default" },
                    { from: "17:Default", to: "19:Default" },
                    { from: "18:Default", to: "20:Questions" },
                    { from: "19:Default", to: "20:Categories" },
                    { from: "21:Default", to: "20:SubCategories" },
                    { from: "22:Default", to: "20:Default" },
                    { from: "22:DefaultPresentation", to: "20:DefaultPresentation" },
                    { from: "22:Header", to: "20:Header" },
                    { from: "22:HeaderPresentation", to: "20:HeaderPresentation" },
                    { from: "17:Default", to: "22:Default" }
                    //{ from: "19:DefaultPresentation", to: "20:DefaultPresentation" },
                    //{ from: "19:Header", to: "20:Header" },
                    //{ from: "19:HeaderPresentation", to: "20:HeaderPresentation" }
                ];
            }

            function showDataSources(sourceList) {
                var container = $("div#main");
                var count = sourceList.length;
                for (var ds = 0; ds < sourceList.length; ds++) {
                    var src = sourceList[ds];
                    if (src.posx == undefined) src.posx = Math.floor((Math.random() * maxX) + 1);
                    if (src.posy == undefined) src.posy = Math.floor((Math.random() * maxY) + 1);
                    src.posx = src.posx / maxX * screenX;
                    src.posy = src.posy / maxY * screenY;
                    // set ideal with based on amount of streams
                    src.boxWidth = src.out.length * streamWidth;
                    if (src.in.length > src.out.length) src.boxWidth = src.in.length * streamWidth;
                    if (src.boxWidth < minWidth) src.boxWidth = minWidth;
                    // set some constants
                    src.id = 'DataSource' + src.sourceid;
                    container.append('<div class="window" id="' + src.id + '" style="top:' + src.posy + 'em; left:' + src.posx + 'em; width:' + src.boxWidth + 'em"><strong><a href="/GettingStarted/tabid/55/ctl/eavmanagement/mid/709/ManagementMode/EditItem/language/en-US/Default.aspx?EntityId=' + src.configurationEntityId + '&popUp=true">' + src.name + '</a></strong>'
                        + '</div>');

                    // Create all out-points
                    for (var o = 0; o < src.out.length; o++) {
                        var pLeftMargin = (src.boxWidth - src.out.length * streamWidth) / 2;
                        var pOffset = o * streamWidth + streamWidth / 2;
                        var posLblX = (pLeftMargin + pOffset) / src.boxWidth;
                        var sourceUUID = src.id + '_out_' + src.out[o];
                        allSourceEndpoints.push(jsPlumb.addEndpoint(src.id, sourceEndpoint, { anchor: [posLblX, 0, 0, 0], uuid: sourceUUID, overlays: [["Label", dynEndpointOverlay(src.out[o], false)]] }));
                    }

                    // Create all in-points
                    for (var i = 0
                    ;
                    i < src.in.length;
                    i++)
                    {
                        var pLeftMargin = (src.boxWidth - src.in.length * streamWidth) / 2;
                        var pOffset = i * streamWidth + streamWidth / 2;
                        var posLblX = (pLeftMargin + pOffset) / src.boxWidth;
                        var sourceUUID = src.id + '_in_' + src.in[i];
                        allTargetEndpoints.push(jsPlumb.addEndpoint(src.id, targetEndpoint, { anchor: [posLblX, 1, 0, 0], uuid: sourceUUID, overlays: [["Label", dynEndpointOverlay(src.in[i], true)]] }));
                    }

                }
                ;
            }

            ;
            showDataSources(DataSources);
            // make all the window divs draggable                                      (again)
            jsPlumb.draggable(jsPlumb.getSelector(".window"), { grid: [20, 20] });

            function setInitialConnections(conList) {
                for (var c = 0; c < conList.length; c++) {
                    var from = 'DataSource' + conList[c].from.replace(":", "_out_");
                    var to = 'DataSource' + conList[c].to.replace(":", "_in_");
                    jsPlumb.connect({ uuids: [from, to], editable: true });
                }
            }

            ;
            setInitialConnections(ConnectionList);


            //
            // listen for clicks on connections, and offer to delete connections on click.
            //
            jsPlumb.bind("click", function(conn, originalEvent) {
                if (confirm("Delete connection from " + conn.sourceId + " to " + conn.targetId + "?"))
                    jsPlumb.detach(conn);
            });

            jsPlumb.bind("connectionDrag", function(connection) {
                console.log("connection " + connection.id + " is being dragged");
            });

            jsPlumb.bind("connectionDragStop", function(connection) {
                console.log("connection " + connection.id + " was dragged");
            });
        }
    };
})();