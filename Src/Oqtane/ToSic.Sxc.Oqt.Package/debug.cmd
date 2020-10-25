@set Dev2sxcOqtaneRoot=c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\
@set OqtaneBin=%Dev2sxcOqtaneRoot%bin\Debug\netcoreapp3.1\

@REM 2sxc Oqtane - Client
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\Debug\netstandard2.1\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\Debug\netstandard2.1\ToSic.*.pdb" "%OqtaneBin%" /Y

@REM 2sxc Oqtane - Server
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\ToSic.*.pdb" "%OqtaneBin%" /Y

@REM 2sxc Oqtane - Server Framework DLLs
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\Microsoft.AspNetCore.Mvc.Razor.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\Microsoft.AspNetCore.Razor.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\Microsoft.CodeAnalys*.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\Microsoft.Extensions.DependencyModel.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\refs\Microsoft.AspNetCore.Antiforgery.dll" "%OqtaneBin%" /Y


@REM XCOPY "..\ToSic.Sxc.Oqt.Server\bin\Debug\netcoreapp3.1\refs\Microsoft.AspNetCore.Antiforgery.dll" "%OqtaneBin%refs" /Y

@REM 2sxc Oqtane - Shared
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\Debug\netstandard2.1\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\Debug\netstandard2.1\ToSic.*.pdb" "%OqtaneBin%" /Y

@REM 2sxc Oqtane - Client Assets
XCOPY "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc\*" "%Dev2sxcOqtaneRoot%wwwroot\Modules\ToSic.Sxc\" /Y /S /I




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


