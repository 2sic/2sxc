@Echo(
@Echo the build folder (Debug or DebugOqtane) must be passed in as a parameter
@set BuildFolder=%1
@set Dev2sxcOqtaneRoot=c:\Projects\2sxc\oqtane\oqtane.framework\Oqtane.Server\
@set OqtaneBin=%Dev2sxcOqtaneRoot%bin\%BuildFolder%\net6.0\
@set PackageName=ToSic.Sxc.Oqtane

@Echo(
@Echo 2sxc Oqtane - Client
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\%BuildFolder%\net6.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\%BuildFolder%\net6.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - Server
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net6.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net6.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - Server Framework DLLs
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net6.0\Microsoft.AspNetCore.Mvc.Razor.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net6.0\Microsoft.AspNetCore.Razor.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net6.0\Microsoft.CodeAnalys*.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net6.0\Microsoft.Extensions.DependencyModel.dll" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - 3rd party deps
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net6.0\CsvHelper.dll" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - Shared
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net6.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net6.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - Client Assets
XCOPY "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\%PackageName%\*" "%Dev2sxcOqtaneRoot%wwwroot\Modules\%PackageName%\" /Y /S /I

@Echo(
@Echo nuget dependencies - oqt-imageflow
XCOPY "..\..\packages\tosic.imageflow.oqtane\1.0.4\lib\net5.0\*" "%OqtaneBin%" /Y
XCOPY "..\..\packages\tosic.imageflow.oqtane\1.0.4\runtimes\*" "%OqtaneBin%\runtimes" /S /C /Y

@Echo(
@Echo Copy Koi DLLs
XCOPY "..\..\..\Dependencies\Koi\netstandard2.0\Connect.Koi.dll" "%OqtaneBin%" /Y
XCOPY "..\..\..\Dependencies\Koi\netstandard2.0\Connect.Koi.pdb" "%OqtaneBin%" /Y

@Echo(
@Echo Copy RazorBlade DLLs from Debug
XCOPY "..\..\..\Dependencies\RazorBlade\Release\net5.0\ToSic.Razor.dll" "%OqtaneBin%" /Y
XCOPY "..\..\..\Dependencies\RazorBlade\Release\net5.0\ToSic.Razor.pdb" "%OqtaneBin%" /Y

@Echo(
@Echo the target for js, css, json etc.
@set BuildTarget=c:\Projects\2sxc\oqtane\oqtane.framework\Oqtane.Server\wwwroot\Modules\%PackageName%

@Echo(
@Echo Copy ImportExport instructions
robocopy /mir "..\ToSic.Sxc.Oqt.Server\Content\2sxc\system\ImportExport\ " "%Dev2sxcOqtaneRoot%\Content\2sxc\system\ImportExport\ "

@Echo(
@Echo Copy the data folders
robocopy /mir "..\..\Data\App_Data\ " "%Dev2sxcOqtaneRoot%\Content\2sxc\system\App_Data\ "
robocopy /mir "..\..\Data\assets\ " "%BuildTarget%\assets\ "

@Echo(
@Echo Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "

@Echo(
@echo Copied all files to this Website target: '%BuildTarget%' in mode Debug
