@set BuildTarget=..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc

@REM Copy the data folders
robocopy /mir "..\..\Data\.data\ " "%BuildTarget%\.data\ "
rmdir /Q /S "%BuildTarget%\.databeta"
rmdir /Q /S "%BuildTarget%\.data-custom"
robocopy /mir "..\..\Data\assets\ " "%BuildTarget%\assets\ "

@REM Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "

@REM Copy ImportExpor assets
@REM TODO: @stv - THIS LOOKS Bad - we should not get files from here, get them from the SRC
robocopy /mir "..\..\..\..\..\2sxc-dnn742\Website\DesktopModules\ToSIC_SexyContent\ImportExport\ " "%BuildTarget%\system\ImportExport\ "


.nuget\nuget.exe pack ToSic.Sxc.nuspec
XCOPY "*.nupkg" "c:\Projects\2sxc\2sxc\InstallPackages\OqtaneModule\" /Y
