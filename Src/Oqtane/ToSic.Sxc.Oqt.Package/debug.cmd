@set Dev2sxcOqtaneRoot=c:\Projects\2sxc\oqtane\oqtane.framework\Oqtane.Server\
@set OqtaneBin=%Dev2sxcOqtaneRoot%bin\Debug\net5.0\

@REM 2sxc Oqtane - Client
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\Debug\net5.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\Debug\net5.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@REM 2sxc Oqtane - Server
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\net5.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\net5.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@REM 2sxc Oqtane - Server Framework DLLs
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\net5.0\Microsoft.AspNetCore.Mvc.Razor.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\net5.0\Microsoft.AspNetCore.Razor.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\net5.0\Microsoft.CodeAnalys*.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\net5.0\Microsoft.Extensions.DependencyModel.dll" "%OqtaneBin%" /Y

@Echo Copying refs folder for runtime compilation of Razor cshtml
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\net5.0\refs\*.dll" "%OqtaneBin%refs\" /Y

@REM 2sxc Oqtane - Shared
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\Debug\net5.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\Debug\net5.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@REM 2sxc Oqtane - Client Assets
XCOPY "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc\*" "%Dev2sxcOqtaneRoot%wwwroot\Modules\ToSic.Sxc\" /Y /S /I




@set BuildTarget=c:\Projects\2sxc\oqtane\oqtane.framework\Oqtane.Server\wwwroot\Modules\ToSic.Sxc

@REM Copy the data folders
robocopy /mir "..\..\Data\.data\ " "%BuildTarget%\.data\ "
robocopy /mir "..\..\Data\.databeta\ " "%BuildTarget%\.databeta\ "
robocopy /mir "..\..\Data\.data-custom\ " "%BuildTarget%\.data-custom\ "

@REM ... find better source
@REM don't use this, the path is already in the env-variables @set Dev2sxcAssets=C:\Projects\2sxc\2sxc\Src\Mvc\Website\wwwroot

@REM Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "

@echo Copied all files to this Website target: '%BuildTarget%'


