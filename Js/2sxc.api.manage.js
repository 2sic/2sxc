
$2sxc.getManageController = function(id) {

    var moduleElement = $(".DnnModule-" + id);
    var config = $.parseJSON(moduleElement.attr("data-2sxc"));

    var manageController = {

        isEditMode: function() {
            // ToDo: Fix isEditMode
            return false;
        },

        // The config object has the following properties:
        // portalId, tabId, moduleId, contentGroupId, dialogUrl, returnUrl, appPath
        config: config,

        getLink: function(settings) {
            
        },

        openDialog: function(settings) {
            //dnnModal.open()
        },

        getButton: function(settings) {
            return $("<a href=''></a>");
        },

        getToolbar: function(entityId, editSettings, addSettings, addWithEditsettings) {
            
        }

    };

    return manageController;
}