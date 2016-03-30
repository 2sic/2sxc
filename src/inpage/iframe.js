(function () {

    var diag = $2sxc.dialog = {
        mode: "iframe",
        templates: {
            inline: "<iframe width='100%' height='200px' src='{{url}}'"
            // + "onload=\"this.syncHeight(this.contentWindow.document.body.scrollHeight);\""
            //+ "onload=\"this.style.height=this.contentWindow.document.body.scrollHeight + 'px';\""
            + "onresize=\"console.log('resize')\""
            + "></iframe>"
        }
    };

    diag.create = function (iid, block, url, closeCallback) {
        block = $(block);

        var ifrm = $(diag.templates.inline.replace("{{url}}", url));

        var diagBox = ifrm[0];

        diagBox.closeCallback = closeCallback;

        diagBox.sxc = $2sxc(iid);

        diagBox.getManageInfo = function() {
            return diagBox.sxc.manage.dialogParameters;
        };

        diagBox.getCommands = function() {
            return diagBox.vm; // created by inner code
        };

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

        block.prepend(diagBox);

        return diagBox;
    };

})();
