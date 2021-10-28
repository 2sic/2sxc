@set BuildTarget=..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc

@REM Copy the data folders
robocopy /mir "..\..\Data\.data\ " "%BuildTarget%\.data\ "

@REM Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "

@REM Copy ImportExpor assets
robocopy /mir "..\..\..\..\..\2sxc-dnn742\Website\DesktopModules\ToSIC_SexyContent\ImportExport\ " "%BuildTarget%\system\ImportExport\ "


"c:\Projects\2sxc\oqtane\oqtane.framework\oqtane.package\nuget.exe" pack ToSic.Sxc.nuspec
XCOPY "*.nupkg" "c:\Projects\2sxc\2sxc\InstallPackages\OqtaneModule\" /Y
