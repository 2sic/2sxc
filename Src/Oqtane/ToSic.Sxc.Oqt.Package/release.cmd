@set OqtaneRoot=..\ToSic.Sxc.Oqt.Server
@set BuildTarget=%OqtaneRoot%\wwwroot\Modules\ToSic.Sxc

@REM Copy the data folders
robocopy /mir "..\..\Data\.data\ " "%OqtaneRoot%\Content\2sxc\system\.data\ "
rmdir /Q /S "%BuildTarget%\.databeta"
rmdir /Q /S "%BuildTarget%\.data-custom"
robocopy /mir "..\..\Data\assets\ " "%BuildTarget%\assets\ "
robocopy /mir "..\..\Data\App_Data\ " "%OqtaneRoot%\Content\2sxc\system\App_Data\ "

@REM Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "

@REM Copy ImportExpor assets
robocopy /mir "..\..\Dnn\ToSic.Sxc.Dnn\ImportExport\ " "%OqtaneRoot%\Content\2sxc\system\ImportExport\ "


.nuget\nuget.exe pack ToSic.Sxc.nuspec
XCOPY "*.nupkg" "c:\Projects\2sxc\2sxc\InstallPackages\OqtaneModule\" /Y
