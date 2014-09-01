

var $2sxc = function (id) {
    
    if (!$2sxc._data[id])
        $2sxc._data[id] = {};

    var controller = $2sxc._controllers[id] ? $2sxc._controllers[id] : $2sxc._controllers[id] = {
        data: {
            // in-streams
            "in": {},

            // Will hold the default stream (["in"]["Default"].List
            List: [],

            controller: null,

            // Load data via ajax
            load: function (source) {
                // If source is already the data, set it
                if (source && source.List) {
                    controller.data.setData(source);
                    return controller.data;
                } else {
                    if (!source)
                        source = {};
                    if (!source.url)
                        source.url = window.location.href + (window.location.href.indexOf("?") != -1 ? "&" : "?") + "mid=" + id + "&standalone=true&popUp=true&type=data";
                    source.origSuccess = source.success;
                    source.success = function (data) {

                        for (var dataSetName in data) {
                            if (data[dataSetName].List != null) {
                                controller.data["in"][dataSetName] = data[dataSetName];
                                controller.data["in"][dataSetName].name = dataSetName;
                            }
                        }

                        if (controller.data["in"]["Default"])
                            controller.List = controller.data["in"]["Default"].List;
                        
                        if (source.origSuccess)
                            source.origSuccess(controller.data);
                        
                        controller.isLoaded = true;
                        controller.lastRefresh = new Date();
                        controller.data._triggerLoaded();
                    };
                    source.error = function(request) {
                        alert(JSON.parse(request.responseText).error);
                    };
                    controller.data.source = source;
                    return controller.data.reload();
                }
            },

            reload: function (optionalCallback) {
                
                // todo: convert dates...

                if (optionalCallback)
                    controller.data.source.success = optionalCallback;

                $.ajax(controller.data.source);
                return controller.data;
            },

            on: function (events, callback) {
                return $(controller.data).bind("2scLoad", callback)[0]._triggerLoaded();
            },

            _triggerLoaded: function () {
                return controller.isLoaded ? $(controller.data).trigger("2scLoad", [controller.data])[0] : controller.data;
            },

            one: function (events, callback) {
                if (!controller.isLoaded)
                    return $(controller.data).one("2scLoad", callback)[0];
                callback({}, controller.data);
                return controller.data;
            }
        },
        
        id: id,
        source: null,
        isLoaded: false,
        lastRefresh: null,
        manage: $2sxc.getManageController ? $2sxc.getManageController(id) : null,
        isEditMode: function() {
            return controller.manage && controller.manage.isEditMode();
        }
    };

    // Make sure back-reference to controller is set
    controller.data.controller = controller;

    return controller;
};

$2sxc._controllers = {};
$2sxc.metaName = "The 2SexyContent Controller object";
$2sxc.metaVersion = "06.02.00";
$2sxc.beta = {};
$2sxc._data = {};