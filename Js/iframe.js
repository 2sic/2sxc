(function () {
    var diag = $2sxc.dialog = {
        mode: "iframe"
    };

    var ifr = diag.iframe = {};

    diag.create = function(block, iid) {
        block = $(block);

        var diagBox = $("<iframe width='100%' height='50px' src='/desktopmodules/tosic_sexycontent/js/iframe.html'"
            // + "onload=\"this.syncHeight(this.contentWindow.document.body.scrollHeight);\""
            //+ "onload=\"this.style.height=this.contentWindow.document.body.scrollHeight + 'px';\""
            + "onresize=\"console.log('resize')\""
            + "></iframe>");

        diagBox[0].callback = function() {
            alert("got called");
        };

        diagBox[0].syncHeight = function (height) {
            console.log("tried resize to " + height);
            diagBox[0].style.height = height + "px";
            // $(".DnnModule-" + modId + " #frGettingStarted").height(height);
        };

        block.prepend(diagBox);

        return diagBox;
    };

    /*
     * todo
     * - load angular inside...
     * - get design to work
     * - pass parameters in
     * - get ajax-reload to work going out
     * - get system to create/destroy iframe
     * - get iframe transparent
     * - find right position for the iframe
     * 
     * 
     * fine tuning
     * - get dynamic src
     * - get shrink resize
     */
})();
