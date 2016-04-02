(function () {

    var diag = $2sxc._dialog = {
        mode: "iframe",
        templates: {
            inline: "<iframe width='100%' height='200px' src='{{url}}'"
            // + "onload=\"this.syncHeight(this.contentWindow.document.body.scrollHeight);\""
            //+ "onload=\"this.style.height=this.contentWindow.document.body.scrollHeight + 'px';\""
            + "onresize=\"console.log('resize')\""
            + "></iframe>"
        }
    };

    diag.create = function (sxc, tag, url, closeCallback) {
        var diagBox = $(diag.templates.inline.replace("{{url}}", url))[0];    // build iframe tag

        diagBox.closeCallback = closeCallback;
        diagBox.sxc = sxc;

        diagBox.getManageInfo = function() {
            return diagBox.sxc.manage.dialogParameters;
        };
        diagBox.getAdditionalDashboardConfig = function() {
            var ec = sxc.manage.editContext;
            return {
                isContent: ec.ContentGroup.IsContent,
                templateId: ec.ContentGroup.TemplateId,
                contentTypeId: ec.ContentGroup.ContentTypeName,
                templateChooserVisible: ec.ContentGroup.TemplateChooserVisible,
            };
        };

        diagBox.getCommands = function() {
            return diagBox.vm; // created by inner code
        };

        // todo: sync sizes
        diagBox.syncHeight = function (height) {
            console.log("tried resize to " + height);
            diagBox.style.height = height + "px";
        };

        diagBox.toggle = function () {
            diagBox.style.display = diagBox.style.display === "none" ? "" : "none";
            diagBox.vm.toggle(); // tell the dashboard about this
        };

        diagBox.justHide = function () {
            diagBox.style.display = "none";
        };

        $(tag).prepend(diagBox);

        return diagBox;
    };

})();
