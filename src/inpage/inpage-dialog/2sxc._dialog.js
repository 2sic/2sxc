// this is a dialog handler which will create in-page dialogs for 
// - the template / view picker
// - the getting-started / install-templates dialog
// 
// known issues
// - we never got around to making the height adjust automatically
(function () {
    var diag = $2sxc._dialog = {
        mode: "iframe",
        template: "<iframe width='100%' height='100px' src='{{url}}' onresize=\"console.log('resize')\"></iframe>"
    };

    diag.create = function (sxc, wrapperTag, url, closeCallback) {
        var iframe = $(diag.template.replace("{{url}}", url))[0];    // build iframe tag

        iframe.closeCallback = closeCallback;
        iframe.sxc = sxc;
        // iframe.attr("data-for-manual-debug", "app: " + sxc.manage.ContentGroup.AppUrl);

        //#region data bridge both ways
        iframe.getManageInfo = function() {
            return iframe.sxc.manage.dialogParameters;
        };

        iframe.getAdditionalDashboardConfig = function () {
            return iframe.sxc.manage.dashboardConfig;
        };

        iframe.getCommands = function() {
            return iframe.vm;
        };
        //#endregion

        //#region sync size
        iframe.syncHeight = function () {
            var height = iframe.contentDocument.body.offsetHeight;
            if (iframe.previousHeight === height)
                return;
            window.diagBox = iframe;
            iframe.height = height + 'px';
            iframe.previousHeight = height;
        };

        function loadEventListener()  {
            iframe.syncHeight();
            iframe.resizeInterval = window.setInterval(iframe.syncHeight, 200); // Not ideal - polling the document height may cause performance issues
            //diagBox.contentDocument.body.addEventListener('resize', function () { diagBox.syncHeight(); }, true); // The resize event is not called reliable when the iframe content changes
        }
        iframe.addEventListener('load', loadEventListener);

        //#endregion

        //#region Visibility toggle & status

        iframe.isVisible = function() { return iframe.style.display !== "none";   };
        iframe.toggle = function () { iframe.style.display = iframe.style.display === "none" ? "" : "none"; };
        iframe.justHide = function () { iframe.style.display = "none"; };
        //#endregion

        // remove the diagBox - important when replacing the app with ajax, and the diag needs to be re-initialized
        iframe.destroy = function () {
            window.clearInterval(iframe.resizeInterval);   // clear this first, to prevent errors
            iframe.remove(); // use the jquery remove for this
        };

        $(wrapperTag).before(iframe);

        return iframe;
    };

})();
