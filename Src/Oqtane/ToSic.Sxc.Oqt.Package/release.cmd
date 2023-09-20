@ECHO off
SET Source=%1
SET OqtaneInstallPackage=%2
ECHO Source=%Source%
ECHO OqtaneInstallPackage=%OqtaneInstallPackage%


REM enables the use of the ! delimiter for delayed variable expansion.
SETLOCAL enabledelayedexpansion 

SET OqtaneRoot=..\ToSic.Sxc.Oqt.Server
SET PackageName=ToSic.Sxc.Oqtane
SET BuildTarget=%OqtaneRoot%\wwwroot\Modules\%PackageName%

REM Copy the data folders
ROBOCOPY /mir "..\..\Data\assets\ " "%BuildTarget%\assets\ "

REM Copy 2sxc JS stuff
ROBOCOPY /mir "%Source%\js\ " "%BuildTarget%\js\ "
ROBOCOPY /mir "%Source%\system\ " "%BuildTarget%\system\ "
ROBOCOPY /mir "%Source%\dist\ " "%BuildTarget%\dist\ "

.nuget\nuget.exe pack %PackageName%.Install.nuspec

REM performs the string substitution. Specifically, it replaces forward slashes (/) with backslashes (\).
SET "NormalizedPath=!OqtaneInstallPackage:/=\!"
MOVE "*.nupkg" "%NormalizedPath%"
