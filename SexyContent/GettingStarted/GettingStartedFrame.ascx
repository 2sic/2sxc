<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GettingStartedFrame.ascx.cs" Inherits="ToSic.SexyContent.GettingStarted.GettingStartedFrame" %>

<div style="position:relative;">
    <iframe id="frGettingStarted" src="<%= GettingStartedUrl() %>" width="100%" height="300px"></iframe>

    <script type="text/javascript">

        window.addEventListener("message", receiveMessage<%= ModuleID %>, false);

        function receiveMessage<%= ModuleID %>(event) {
            var modId = <%=ModuleID %>;
            var regExToCheckOrigin = /^(http|https):\/\/((gettingstarted|[a-z]*)\.)?(2sexycontent|2sxc)\.org(\/.*)?$/gi;
            if (!regExToCheckOrigin.test(event.origin)) {
                console.error("can't execute, wrong source");
                return;
            }

            // Data is sent as text because IE8 and 9 cannot send objects through postMessage
            var data = JSON.parse(event.data);

            // If message does not belong to this module, return
            if (data.moduleId !== modId)
                return;

            if (data.action === "install") {
                var sf = $.ServicesFramework(modId);

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
                    $(".DnnModule-" + modId + " #pnlLoading").show();
                    var label = $(".DnnModule-" + modId + " #packageName");

                    label.html("...");
                    
                    runOneInstallJob(packages, 0, sf, label);
                }

            }
            else if (data.action === "resize")
                resizeIFrame(modId, data.height);
        }

        function resizeIFrame(modId, height) {
                $(".DnnModule-" + modId + " #frGettingStarted").height(height);
        }

        function runOneInstallJob(packages, i, sf, label) {
            var currentPackage = packages[i];
            console.log(currentPackage.displayName + "(" + i + ") started");
            label.html(currentPackage.displayName);
            return $.ajax({
                type: "GET",
                dataType: "json",
                async: true,
                url: sf.getServiceRoot('2sxc') + "GettingStarted/" + "InstallPackage",
                data: "packageUrl=" + currentPackage.url,
                beforeSend: sf.setModuleHeaders
            })
            .complete(function(jqXHR, textStatus) {
                console.log(currentPackage.displayName + "(" + i + ") completed");
                if (i + 1 < packages.length) {
                    runOneInstallJob(packages, i + 1, sf, label);
                } else {
                    alert("Done installing. If you saw no errors, everything worked.");
                    window.location.reload();
                }
            })
            .error(function (xhr, result, status) {
                var errorMessage = "Something went wrong while installing '" + currentPackage.displayName + "': " + status;
                if(xhr.responseText && xhr.responseText !== "")
                {
                    var response = $.parseJSON(xhr.responseText);
                    if(response.messages)
                        errorMessage = errorMessage + " - " + response.messages[0].Message;
                    else if(response.Message)
                        errorMessage = errorMessage + " - " + response.Message;
                }
                errorMessage += " (you might find more informations about the error in the DNN event log).";
                alert(errorMessage);
            });
        }
    </script>

    <div class="sc-loading" id="pnlLoading" style="display:none;">
        <i class="icon-sxc-spinner animate-spin"></i>
        <br/>
        <br/>
        <span class="sc-loading-label">
            installing <span id="packageName">.</span>
        </span>
    </div>
</div>