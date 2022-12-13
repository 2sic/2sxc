@set OqtaneRoot=..\ToSic.Sxc.Oqt.Server
@set PackageName=ToSic.Sxc.Oqtane
@set BuildTarget=%OqtaneRoot%\wwwroot\Modules\%PackageName%

@REM Copy the data folders
robocopy /mir "..\..\Data\App_Data\new-app\ " "%OqtaneRoot%\Content\2sxc\system\App_Data\new-app\ "
robocopy /mir "..\..\Data\App_Data\system\ " "%OqtaneRoot%\Content\2sxc\system\App_Data\system\ "
rmdir /Q /S "%BuildTarget%\.databeta"
rmdir /Q /S "%BuildTarget%\.data-custom"
rmdir /Q /S "%BuildTarget%\App_Data\system-beta"
rmdir /Q /S "%BuildTarget%\App_Data\system-custom"
robocopy /mir "..\..\Data\assets\ " "%BuildTarget%\assets\ "

@REM Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "

@REM Copy ImportExpor assets
robocopy /mir "..\..\Dnn\ToSic.Sxc.Dnn\ImportExport\ " "%OqtaneRoot%\Content\2sxc\system\ImportExport\ "


.nuget\nuget.exe pack %PackageName%.nuspec
XCOPY "*.nupkg" "c:\Projects\2sxc\2sxc\InstallPackages\OqtaneModule\" /Y
