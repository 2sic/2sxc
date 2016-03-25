(function () {

    var diag = $2sxc.dialog = {
        mode: "iframe",
        templates: {
            inline: "<iframe width='100%' height='200px' src='{{url}}'"
            // + "onload=\"this.syncHeight(this.contentWindow.document.body.scrollHeight);\""
            //+ "onload=\"this.style.height=this.contentWindow.document.body.scrollHeight + 'px';\""
            + "onresize=\"console.log('resize')\""
            + "style=\"display: none\""
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

        diagBox.getManageInfo = function() {
            return $2sxc(iid).manage._manageInfo;
        };

        diagBox.getCommands = function() {
            return diagBox.vm; // created by inner code
        };

        diagBox.syncHeight = function (height) {
            console.log("tried resize to " + height);
            diagBox.style.height = height + "px";
        };

        diagBox.toggle = function() {
            diagBox.style.display = diagBox.style.display === "none" ? "" : "none";
        };

        diagBox.replaceContent = function (newContent) {
            $(viewPortSelector).html(newContent);
            $2sxc(iid).manage._processToolbars();
        };

        block.prepend(diagBox);

        return diagBox;
    };

    /*
     * todo
     * - get design to work
     * - get system to create/destroy iframe
     * - find right position for the iframe
     * - get system to be fast again
     * - extract i18n
     * 
     * fine tuning
     * - get dynamic src
     * - get shrink resize
     * - create the iframe without jquery
     */
})();
