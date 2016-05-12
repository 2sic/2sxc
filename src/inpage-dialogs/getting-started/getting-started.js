
// note: this is code which still uses jQuery etc., so it's not really clean
// because of this we're including it as simple code and not packaging it as a service quite yet...

function processInstallMessage(event, modId, progressIndicator, $http) {
    var regExToCheckOrigin = /^(http|https):\/\/((gettingstarted|[a-z]*)\.)?(2sexycontent|2sxc)\.org(\/.*)?$/gi;
    if (!regExToCheckOrigin.test(event.origin)) {
        console.error("can't execute, wrong source domain");
        return;
    }

    // Data is sent as text because IE8 and 9 cannot send objects through postMessage
    var data = JSON.parse(event.data);

    modId = Number(modId);
    // If message does not belong to this module, return
    if (data.moduleId !== modId)
        return;

    if (data.action === "install") {
        // var sf = $.ServicesFramework(modId);

        var packages = data.packages;
        var packagesDisplayNames = "";

        // Loop all packages to install
        for (var i = 0; i < packages.length; i++) {
            packagesDisplayNames += "- " + packages[i].displayName + "\n";
        }

        if (confirm("Do you want to install these packages?\n\n"
            + packagesDisplayNames + "\nThis could take 10 to 60 seconds per package, "
            + "please don't reload the page while it's installing. "
            + "You will see a message once it's done and progess is logged to the JS-console.")) {

            progressIndicator.show = true;
            progressIndicator.label = ".....";

            runOneInstallJob(packages, 0, progressIndicator, $http);
        }

    }
    else if (data.action === "resize")
        resizeIFrame(modId, data.height);
}

function resizeIFrame(modId, height) {
    document.getElementById("frGettingStarted").style.height = (height + 10) + "px";
}

function runOneInstallJob(packages, i, progressIndicator, $http) {
    var currentPackage = packages[i];
    console.log(currentPackage.displayName + "(" + i + ") started");
    progressIndicator.label = currentPackage.displayName;
    return $http.get("app/installer/installpackage",
        { params: { "packageUrl": currentPackage.url } })
    //$.ajax({
    //    type: "GET",
    //    dataType: "json",
    //    async: true,
    //    url: sf.getServiceRoot('2sxc') + "Installer/" + "InstallPackage",
    //    data: "packageUrl=" + currentPackage.url,
    //    beforeSend: sf.setModuleHeaders
    //})
    .then(function (response) {
        console.log(currentPackage.displayName + "(" + i + ") completed");
        if (i + 1 < packages.length) {
            runOneInstallJob(packages, i + 1, progressIndicator, $http);
        } else {
            alert("Done installing. If you saw no errors, everything worked.");
            window.location.reload();
        }
    }, function (xhr) {
        var errorMessage = "Something went wrong while installing '" + currentPackage.displayName + "': " + status;
        if (xhr.responseText && xhr.responseText !== "") {
            var response = JSON.parse(xhr.responseText);
            if (response.messages)
                errorMessage = errorMessage + " - " + response.messages[0].Message;
            else if (response.Message)
                errorMessage = errorMessage + " - " + response.Message;
        }
        errorMessage += " (you might find more informations about the error in the DNN event log).";
        alert(errorMessage);
    });
}