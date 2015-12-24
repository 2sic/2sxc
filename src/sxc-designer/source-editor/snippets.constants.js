(function () {
    /*jshint multistr: true */

    angular.module("SourceEditor")

        .constant("snippets", {
            "tokens":
                "# Some useful 2sxc tags / placeholders \n\
# toolbar\n\
snippet toolbar \n\
key Toolbar \n\
title Toolbar \n\
help Toolbar for inline editing with 2sxc. If used inside a <div class=\"sc-element\"> then the toolbar will automatically float \n\
	[${1:Content}:Toolbar]\n\
",


            "html": "",



            "razor": "# Some useful 2sxc tags / placeholders \n\
#######################\n\
### Razor App stuff\n\
# path\n\
set app\n\
title Path \n\
help returns the url to the current app, for integrating scripts, images etc. For example, use as ***\/scripts\/knockout.js\n\
snippet path \n\
	@App.Path\n\
# physical path\n\
set app\n\
title Physical path \n\
help physical path, in c:\\\n\
snippet physical path \n\
	@App.PhysicalPath\n\
# App Guid \n\
set app\n\
title App Guid \n\
help internal GUID - should stay the same across all systems for this specific App \n\
snippet app guid \n\
	@App.AppGuid\n\
# App Id \n\
set app\n\
title App Id \n\
help Id in the current data base. Is a different number in every App-Installation \n\
snippet app id \n\
	@App.AppId\n\
# App Name \n\
set app\n\
title App Name \n\
help internal name \n\
snippet app name \n\
	@App.Name\n\
# App Folder \n\
set app\n\
title App Folder \n\
help folder of the 2sxc-app, often used to create paths to scripts or join some values. if you only need to reference a script, please use App.Path \n\
snippet app folder \n\
	@App.Folder\n\
            "
            });


} ());