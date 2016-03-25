(function () {

    var diag = $2sxc.dialog = {
        mode: "iframe",
        templates: {
            inline: "<iframe width='100%' height='200px' src='{{url}}'"
            // + "onload=\"this.syncHeight(this.contentWindow.document.body.scrollHeight);\""
            //+ "onload=\"this.style.height=this.contentWindow.document.body.scrollHeight + 'px';\""
            + "onresize=\"console.log('resize')\""
            // + "style=\"display: none\""
            + "></iframe>"
        }
    };

    //var ifr = diag.iframe = {};

    diag.create = function (iid, block, url, closeCallback) {
        var viewPortSelector = ".DnnModule-" + iid + " .sc-viewport";

        block = $(block);

        var ifrm = $(diag.templates.inline.replace("{{url}}", url));

        var diagBox = ifrm[0];
        diagBox.callback = function() {
            alert("got called");
        };
        diagBox.closeCallback = closeCallback;

        diagBox.sxc = $2sxc(iid);

        diagBox.getManageInfo = function() {
            return diagBox.sxc.manage._manageInfo;
        };

        diagBox.getCommands = function() {
            return diagBox.vm; // created by inner code
        };

        diagBox.syncHeight = function (height) {
            console.log("tried resize to " + height);
            diagBox.style.height = height + "px";
        };

        diagBox.toggle = function () {
            alert('toggle');
            diagBox.style.display = diagBox.style.display === "none" ? "" : "none";
            diagBox.vm.toggle(); // tell the dashboard about this
        };

        diagBox.hideFromInside = function () {
            diagBox.style.display = "none";
        };

        block.prepend(diagBox);

        return diagBox;
    };

    /*
     * todo
     * - get design to work
     * - get system to create/destroy iframe
     * - get system to be fast again
     * - extract i18n
     * 
     * phase 2
     * - getting-started installer
     * - get installer to work again
     * 
     * fine tuning
     * - get shrink resize
     * - create the iframe without jquery
     */
})();
