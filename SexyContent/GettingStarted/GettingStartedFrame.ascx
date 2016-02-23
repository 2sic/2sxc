<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GettingStartedFrame.ascx.cs" Inherits="ToSic.SexyContent.GettingStarted.GettingStartedFrame" %>

<div style="position:relative;">
    <iframe id="frGettingStarted" src="<%= GettingStartedUrl() %>" width="100%" height="300px"></iframe>

    <script type="text/javascript">
    
        window.addEventListener("message", recieveMessage<%= ModuleID %>, false);

        function recieveMessage<%= ModuleID %>(event) {
            var regExToCheckOrigin = /^(http|https):\/\/gettingstarted\.(2sexycontent|2sxc)\.org.*/gi;
            if (!regExToCheckOrigin.test(event.origin))
                return;

            // old one, before we used new domains and optional SSL
            //if (event.origin !== "http://gettingstarted.2sexycontent.org")
            //    return;

            // Data is sent as text because IE8 and 9 cannot send objects through postMessage
            var data = JSON.parse(event.data);

            // If message does not belong to this module, return
            if (data.moduleId != <%=ModuleID %>)
                return;

            if (data.action == "install") {
                var packages = data.packages;
                var packagesDisplayNames = "";

                // Loop all packages to install
                for (var i = 0; i < packages.length; i++) {
                    packagesDisplayNames += "- " + packages[i].displayName + "\n";
                }

                if (confirm("Do you want to install these packages?\n\n" + packagesDisplayNames)) {
                    InstallPackages<%= ModuleID %>(packages);
                }

            }
            else if (data.action == "resize") {
                $(".DnnModule-<%= ModuleID %> #frGettingStarted").height(data.height);
            }
        }

        function InstallPackages<%= ModuleID %>(packages) {

            $("#<%=pnlLoading.ClientID%>").show();
            var sf = $.ServicesFramework(<%= ModuleID %>);
            var success = true;

            // Loop all packages and install them
            for (var i = 0; i < packages.length; i++) {

                var currentPackage = packages[i];

                $.ajax({
                    type: "GET",
                    dataType: "json",
                    async: false,
                    url: sf.getServiceRoot('2sxc') + "GettingStarted/" + "InstallPackage",
                    data: "packageUrl=" + currentPackage.url,
                    beforeSend: sf.setModuleHeaders
                }).done(function (e) { })
                .error(function (xhr, result, status) {
                    var errorMessage = "Something went wrong while installing '" + currentPackage.displayName + "': " + status;
                    if(xhr.responseText && xhr.responseText != "")
                    {
                        var response = $.parseJSON(xhr.responseText);
                        if(response.messages)
                            errorMessage = errorMessage + " - " + response.messages[0].Message;
                        else if(response.Message)
                            errorMessage = errorMessage + " - " + response.Message;
                    }
                    errorMessage += " (you might find more informations about the error in the DNN event log).";
                    alert(errorMessage);
                    success = false;
                });

            }

            if (success) {
                alert("Installed  all packages successfully.");
                window.location.reload();
            }

        }

    </script>

    <div class="sc-loading" id="pnlLoading" runat="server" style="display:none;"></div>
</div>