@ECHO off
ECHO(
ECHO the build folder (Debug or DebugOqtane) must be passed in as a parameter
REM Displaying the parameters received
SET BuildFolder=%1
SET OqtaneTarget=%2

ECHO BuildFolder=%BuildFolder%
ECHO OqtaneTarget=%OqtaneTarget%

SET OqtaneBin=%OqtaneTarget%bin\%BuildFolder%\net9.0
SET PackageName=ToSic.Sxc.Oqtane
SET BuildTarget=%OqtaneTarget%wwwroot\Modules\%PackageName%
ECHO The target folder is: %OqtaneBin%

ECHO(
ECHO 2sxc Oqtane - Client
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\%BuildFolder%\net8.0\ToSic.*.dll" "%OqtaneBin%\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\%BuildFolder%\net8.0\ToSic.*.pdb" "%OqtaneBin%\" /Y

ECHO(
ECHO 2sxc Oqtane - Server
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net8.0\ToSic.*.dll" "%OqtaneBin%\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net8.0\ToSic.*.pdb" "%OqtaneBin%\" /Y

ECHO(
ECHO 2sxc Oqtane - Server Framework DLLs
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net8.0\Microsoft.AspNetCore.Mvc.Razor.*.dll" "%OqtaneBin%\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net8.0\Microsoft.AspNetCore.Razor.*" "%OqtaneBin%\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net8.0\Microsoft.CodeAnalys*.*" "%OqtaneBin%\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net8.0\Microsoft.Extensions.DependencyModel.dll" "%OqtaneBin%\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net8.0\System.Runtime.Caching.dll" "%OqtaneBin%\" /Y

ECHO(
ECHO 2sxc Oqtane - 3rd party deps
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net8.0\CsvHelper.dll" "%OqtaneBin%\" /Y

ECHO(
ECHO 2sxc Oqtane - Shared
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net8.0\ToSic.*.dll" "%OqtaneBin%\" /Y
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net8.0\ToSic.*.pdb" "%OqtaneBin%\" /Y

ECHO(
ECHO 2sxc Oqtane - Client Assets
XCOPY "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\%PackageName%\*" "%BuildTarget%\" /Y /S /I

ECHO(
ECHO nuget dependencies - oqt-imageflow
XCOPY "..\..\packages\tosic.imageflow.oqtane\1.11.0\lib\net7.0\*" "%OqtaneBin%\" /Y
XCOPY "..\..\packages\tosic.imageflow.oqtane\1.11.0\runtimes\win-x64\*" "%OqtaneBin%\runtimes\win-x64\" /S /C /Y
XCOPY "..\..\packages\tosic.imageflow.oqtane\1.11.0\runtimes\win-x86\*" "%OqtaneBin%\runtimes\win-x86\" /S /C /Y

ECHO(
ECHO Copy Koi DLLs
XCOPY "..\..\..\Dependencies\Koi\net6.0\Connect.Koi.dll" "%OqtaneBin%\" /Y
XCOPY "..\..\..\Dependencies\Koi\net6.0\Connect.Koi.pdb" "%OqtaneBin%\" /Y

ECHO(
ECHO Copy RazorBlade DLLs from Debug
XCOPY "..\..\..\Dependencies\RazorBlade\Release\net6.0\ToSic.Razor.dll" "%OqtaneBin%\" /Y
XCOPY "..\..\..\Dependencies\RazorBlade\Release\net6.0\ToSic.Razor.pdb" "%OqtaneBin%\" /Y

ECHO(
ECHO the target for js, css, json etc is: %BuildTarget%
REM robocopy /mir "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc.Oqtane\js\ " "%BuildTarget%\js\ "
REM robocopy /mir "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc.Oqtane\dist\ " "%BuildTarget%\dist\ "
REM robocopy /mir "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc.Oqtane\system\ " "%BuildTarget%\system\ "

ECHO(
ECHO Copy ImportExport instructions
robocopy /mir "..\ToSic.Sxc.Oqt.Server\Content\2sxc\system\ImportExport\ " "%OqtaneTarget%\Content\2sxc\system\ImportExport\ "

ECHO(
ECHO Copy the data folders
robocopy /mir "..\..\Data\App_Data\ " "%OqtaneTarget%\Content\2sxc\system\App_Data\ "
robocopy /s "..\..\..\..\2sxc-dev-materials\App_Data\ " "%OqtaneTarget%\Content\2sxc\system\App_Data\ "
robocopy /mir "..\..\Data\assets\ " "%BuildTarget%\assets\ "

ECHO(
ECHO Copied all files to this Website target: '%BuildTarget%' in mode '%BuildFolder%'
