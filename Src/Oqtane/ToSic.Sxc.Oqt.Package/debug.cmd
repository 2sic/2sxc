@Echo(
@Echo the build folder (Debug or DebugOqtane) must be passed in as a parameter
@set BuildFolder=%1
@set Dev2sxcOqtaneRoot=c:\Projects\2sxc\oqtane\oqtane.framework\Oqtane.Server\
@set OqtaneBin=%Dev2sxcOqtaneRoot%bin\%BuildFolder%\net6.0\

@Echo(
@Echo 2sxc Oqtane - Client
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\%BuildFolder%\net5.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Client\bin\%BuildFolder%\net5.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - Server
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\ToSic.*.pdb" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - Server Framework DLLs
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\Microsoft.AspNetCore.Mvc.Razor.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\Microsoft.AspNetCore.Razor.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\Microsoft.CodeAnalys*.*" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\Microsoft.Extensions.DependencyModel.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\System.Data.SqlClient.dll" "%OqtaneBin%" /Y

@Echo(
@Echo 2sxc Oqtane - ICSharpCode.SharpZipLib.dll
XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\ICSharpCode.SharpZipLib.dll" "%OqtaneBin%" /Y

@Echo(
@Echo Copying refs folder for runtime compilation of Razor cshtml
:: XCOPY "..\ToSic.Sxc.Oqt.Server\bin\%BuildFolder%\net5.0\refs\*.dll" "%OqtaneBin%refs\" /Y
XCOPY "..\..\..\..\oqtane-razor-refs\net6.0\refs\*.dll" "%OqtaneBin%refs\" /Y

@Echo(
@Echo 2sxc Oqtane - Shared
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net5.0\ToSic.*.dll" "%OqtaneBin%" /Y
XCOPY "..\ToSic.Sxc.Oqt.Shared\bin\%BuildFolder%\net5.0\ToSic.*.pdb" "%OqtaneBin%" /Y

::@Echo(
::@Echo 2sxc Oqtane - ImportExport Assets
::XCOPY "..\..\..\..\..\2sxc-dnn742\Website\DesktopModules\ToSIC_SexyContent\ImportExport\*" "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc\ImportExport" /Y /S /I

@Echo(
@Echo 2sxc Oqtane - Client Assets
XCOPY "..\ToSic.Sxc.Oqt.Server\wwwroot\Modules\ToSic.Sxc\*" "%Dev2sxcOqtaneRoot%wwwroot\Modules\ToSic.Sxc\" /Y /S /I

@Echo(
@Echo 2sxc Oqtane - System.Data.SqlClient - version used in Oqt 2.0.2
XCOPY "..\..\packages\system.data.sqlclient\4.6.0\lib\netcoreapp2.1\System.Data.SqlClient.dll" "%OqtaneBin%" /Y

@Echo(
@Echo nuget dependencies - Imazen ImageFlow
XCOPY "..\..\packages\imazen.common\0.5.6\lib\netstandard2.0\Imazen.Common.dll" "%OqtaneBin%" /Y
::XCOPY "..\..\packages\system.drawing.common\5.0.0\runtimes\win\lib\netcoreapp3.0\System.Drawing.Common.dll" "%OqtaneBin%" /Y

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
robocopy /mir "..\..\Data\.data\ " "%Dev2sxcOqtaneRoot%\2sxc\.data\ "
robocopy /mir "..\..\Data\.databeta\ " "%Dev2sxcOqtaneRoot%\2sxc\.databeta\ "
robocopy /mir "..\..\Data\.data-custom\ " "%Dev2sxcOqtaneRoot%\2sxc\.data-custom\ "
robocopy /mir "..\..\Data\assets\ " "%BuildTarget%\assets\ "

@Echo(
@Echo Copy 2sxc JS stuff
robocopy /mir "%Dev2sxcAssets%\js\ " "%BuildTarget%\js\ "
robocopy /mir "%Dev2sxcAssets%\dist\ " "%BuildTarget%\dist\ "
robocopy /mir "%Dev2sxcAssets%\system\ " "%BuildTarget%\system\ "

@Echo(
@echo Copied all files to this Website target: '%BuildTarget%' in mode Debug


