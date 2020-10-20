"..\..\oqtane.framework\oqtane.package\nuget.exe" pack ToSic.Sxc.nuspec
XCOPY "*.nupkg" "c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\wwwroot\Modules\" /Y
