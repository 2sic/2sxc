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

        //#region data bridge both ways
        diagBox.getManageInfo = function() {    return diagBox.sxc.manage.dialogParameters; };

        diagBox.getAdditionalDashboardConfig = function () {
            return diagBox.sxc.manage.dashboardConfig;
        };

        diagBox.getCommands = function() { return diagBox.vm;  };// created by inner code

        //#endregion

        //#region sync size - not completed yet
        // todo: sync sizes
        diagBox.syncHeight = function (height) {
            console.log("tried resize to " + height);
            diagBox.style.height = height + "px";
        };

        //#endregion

        //#region Visibility toggle & status

        diagBox.isVisible = function() { return diagBox.style.display !== "none";   };

        diagBox.toggle = function () { diagBox.style.display = diagBox.style.display === "none" ? "" : "none"; };

        diagBox.justHide = function () { diagBox.style.display = "none"; };
        //#endregion

        $(tag).before(diagBox);

        return diagBox;
    };

})();
