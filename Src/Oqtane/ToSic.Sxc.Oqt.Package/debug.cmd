@Echo(
@Echo the build folder (Debug or DebugOqtane) must be passed in as a parameter
@set BuildFolder=%1
@set Dev2sxcOqtaneRoot=c:\Projects\2sxc\oqtane\oqtane.framework\Oqtane.Server\
@set OqtaneBin=%Dev2sxcOqtaneRoot%bin\%BuildFolder%\net6.0\

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
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net6.0\System.Data.SqlClient.dll" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - ICSharpCode.SharpZipLib.dll
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net6.0\ICSharpCode.SharpZipLib.dll" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - Shared
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net6.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net6.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - Client Assets
XCOPY "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc\*" "%Dev2sxcOqtaneRoot%wwwroot\Modules\ToSic.Sxc\" /Y /S /I

@Echo(
@Echo 2sxc Oqtane - System.Data.SqlClient
XCOPY "..\..\packages\system.data.sqlclient\4.8.3\lib\netstandard2.0\System.Data.SqlClient.dll" "%OqtaneBin%" /Y

@Echo(
@Echo nuget dependencies - Imazen ImageFlow
XCOPY "..\..\packages\imazen.common\0.5.6\lib\netstandard2.0\Imazen.Common.dll" "%OqtaneBin%" /Y

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
@set BuildTarget=c:\Projects\2sxc\oqtane\oqtane.framework\Oqtane.Server\wwwroot\Modules\ToSic.Sxc

@Echo(
@Echo Copy the data folders
robocopy /mir "..\..\Data\.data\ " "%BuildTarget%\.data\ "
robocopy /mir "..\..\Data\.databeta\ " "%BuildTarget%\.databeta\ "
robocopy /mir "..\..\Data\.data-custom\ " "%BuildTarget%\.data-custom\ "
robocopy /mir "..\..\Data\assets\ " "%BuildTarget%\assets\ "

@Echo(
@Echo Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "

@Echo(
@echo Copied all files to this Website target: '%BuildTarget%' in mode Debug


