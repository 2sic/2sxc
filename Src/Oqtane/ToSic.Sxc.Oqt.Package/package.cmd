del "*.nupkg"
del "*.zip"
dotnet clean -c Release ..\ToSic.Sxc.Oqt.Server
dotnet clean -c Release ..\ToSic.Sxc.Oqt.Package 
:: dotnet build -c Release ..\ToSic.Sxc.Oqt.Server
dotnet build -c Release ..\ToSic.Sxc.Oqt.Package
release.cmd
pause 
