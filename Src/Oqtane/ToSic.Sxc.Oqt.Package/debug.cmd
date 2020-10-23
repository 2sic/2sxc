XCOPY "..\ToSic.Sxc.Oqt.Client\bin\Debug\netstandard2.1\ToSic.*.dll" "c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\Debug\netstandard2.1\ToSic.*.pdb" "c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\ToSic.*.dll" "c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\ToSic.*.pdb" "c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\Debug\netstandard2.1\ToSic.*.dll" "c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\Debug\netstandard2.1\ToSic.*.pdb" "c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc\*" "c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\wwwroot\Modules\ToSic.Sxc\" /Y /S /I




@set BuildTarget=c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\wwwroot\Modules\ToSic.Sxc

@REM Copy the data folders
robocopy /mir "..\..\Data\.data\ " "%BuildTarget%\.data\ "
robocopy /mir "..\..\Data\.databeta\ " "%BuildTarget%\.databeta\ "
robocopy /mir "..\..\Data\.data-custom\ " "%BuildTarget%\.data-custom\ "

@REM Copy 2sxc JS stuff
@REM robocopy /mir "$(Dev2sxcAssets)\js\ " "%BuildTarget%\js\ "
@REM robocopy /mir "$(Dev2sxcAssets)\dist\ " "%BuildTarget%\dist\ "
@REM robocopy /mir "$(Dev2sxcAssets)\system\ " "%BuildTarget%\system\ "

@echo Copied all files to this Website target: '%BuildTarget%'


