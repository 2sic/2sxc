@REM the build folder (Debug or DebugOqtane) must be passed in as a parameter
@set BuildFolder=%1
@set Dev2sxcOqtaneRoot=c:\Projects\2sxc\oqtane\oqtane.framework\Oqtane.Server\
@set OqtaneBin=%Dev2sxcOqtaneRoot%bin\%BuildFolder%\net5.0\

@REM 2sxc Oqtane - Client
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\%BuildFolder%\net5.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\%BuildFolder%\net5.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@REM 2sxc Oqtane - Server
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@REM 2sxc Oqtane - Server Framework DLLs
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\Microsoft.AspNetCore.Mvc.Razor.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\Microsoft.AspNetCore.Razor.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\Microsoft.CodeAnalys*.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\Microsoft.Extensions.DependencyModel.dll" "%OqtaneBin%" /Y

@Echo Copying refs folder for runtime compilation of Razor cshtml
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\refs\*.dll" "%OqtaneBin%refs\" /Y

@REM 2sxc Oqtane - Shared
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net5.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net5.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@REM 2sxc Oqtane - Client Assets
XCOPY "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc\*" "%Dev2sxcOqtaneRoot%wwwroot\Modules\ToSic.Sxc\" /Y /S /I

@REM nuget dependences
XCOPY "..\..\packages\imazen.common\0.5.6\lib\netstandard2.0\Imazen.Common.dll" "%OqtaneBin%" /Y
XCOPY "..\..\packages\system.drawing.common\5.0.0\runtimes\win\lib\netcoreapp3.0\System.Drawing.Common.dll" "%OqtaneBin%" /Y

@REM the target for js, css, json etc.
@set BuildTarget=c:\Projects\2sxc\oqtane\oqtane.framework\Oqtane.Server\wwwroot\Modules\ToSic.Sxc

@REM Copy the data folders
robocopy /mir "..\..\Data\.data\ " "%BuildTarget%\.data\ "
robocopy /mir "..\..\Data\.databeta\ " "%BuildTarget%\.databeta\ "
robocopy /mir "..\..\Data\.data-custom\ " "%BuildTarget%\.data-custom\ "

@REM Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "

@echo Copied all files to this Website target: '%BuildTarget%' in mode Debug


