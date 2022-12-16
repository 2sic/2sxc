@set OqtaneRoot=..\ToSic.Sxc.Oqt.Server
@set PackageName=ToSic.Sxc.Oqtane
@set BuildTarget=%OqtaneRoot%\wwwroot\Modules\%PackageName%

@REM Copy the data folders
robocopy /mir "..\..\Data\assets\ " "%BuildTarget%\assets\ "

@REM Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "


.nuget\nuget.exe pack %PackageName%.nuspec
XCOPY "*.nupkg" "c:\Projects\2sxc\2sxc\InstallPackages\OqtaneModule\" /Y
