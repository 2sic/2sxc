(function () {

    var diag = $2sxc._dialog = {
        mode: "iframe",
        templates: {
            inline: "<iframe width='100%' height='100px' src='{{url}}'"
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

        //#region sync size
        diagBox.syncHeight = function () {
            var height = diagBox.contentDocument.body.offsetHeight;
            if (diagBox.previousHeight == height)
                return;
            window.diagBox = diagBox;
            console.log("tried resize to " + height);
            diagBox.height = height + 'px';
            diagBox.previousHeight = height;
        };
        diagBox.addEventListener('load', function () {
            diagBox.syncHeight();
            window.setInterval(diagBox.syncHeight, 200); // Not ideal - polling the document height may cause performance issues
            //diagBox.contentDocument.body.addEventListener('resize', function () { diagBox.syncHeight(); }, true); // The resize event is not called reliable when the iframe content changes
        });

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
