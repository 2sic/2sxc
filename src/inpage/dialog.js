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
        diagBox.getAdditionalDashboardConfig = function () {
            return diagBox.sxc.manage.dashboardConfig;
        };

        diagBox.getCommands = function() {
            return diagBox.vm; // created by inner code
        };

        // todo: sync sizes
        diagBox.syncHeight = function (height) {
            console.log("tried resize to " + height);
            diagBox.style.height = height + "px";
        };

        diagBox.isVisible = function() {
            return diagBox.style.display !== "none";
        };

        //diagBox.persistState = function () {
        //    var currentlyShowing = diagBox.style.display !== "none";
        //    if (sxc.manage && sxc.manage.rootCB && sxc.manage.editContext.ContentBlock.ShowTemplatePicker !== currentlyShowing)
        //        sxc.manage.rootCB.setTemplateChooserState(currentlyShowing);
        //};

        //diagBox.justToggle = function () {
        //    diagBox.style.display = diagBox.style.display === "none" ? "" : "none";
        //};

        diagBox.toggle = function () {
            diagBox.style.display = diagBox.style.display === "none" ? "" : "none";
            // todo: probably stop toggling, or maybe in the future pass in reset configs
            //diagBox.vm.toggle(); // tell the inner dashboard about this
            //diagBox.persistState();
        };

        diagBox.justHide = function () {
            diagBox.style.display = "none";
            //diagBox.persistState();
        };


        $(tag).prepend(diagBox);

        //diagBox.persistState();

        return diagBox;
    };

})();
