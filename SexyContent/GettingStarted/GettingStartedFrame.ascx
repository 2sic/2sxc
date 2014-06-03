<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GettingStartedFrame.ascx.cs" Inherits="ToSic.SexyContent.GettingStarted.GettingStartedFrame" %>

<div style="position:relative;">
    <iframe runat="server" id="frGettingStarted" src="" width="100%" height="300px"></iframe>

    <script type="text/javascript">
    
        window.addEventListener("message", recieveMessage<%= ModuleID %>, false);

        function recieveMessage<%= ModuleID %>(event) {
            if (event.origin !== "http://gettingstarted.2sexycontent.org")
                return;

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
                $("#<%=frGettingStarted.ClientID %>").height(data.height);
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
                    url: sf.getServiceRoot('ToSIC_SexyContent') + "GettingStarted/" + "InstallPackage",
                    data: "packageUrl=" + currentPackage.url,
                    beforeSend: sf.setModuleHeaders
                }).done(function (e) {
                    if (e.error != "") {
                        alert("Something went wrong while installing '" + currentPackage.displayName + "': \n" + e.error);
                        success = false;
                    }
                    if (window.console)
                        console.log(e);
                }).fail(function (xhr, result, status) {
                    alert("Something went wrong while installing '" + currentPackage.displayName + "': " + status);
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