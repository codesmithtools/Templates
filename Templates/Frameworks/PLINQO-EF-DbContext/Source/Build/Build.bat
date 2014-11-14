@echo off
del *.nupkg
"..\.nuget\NuGet.exe" pack PLINQO.EntityFramework.DbContext.nuspec
copy *.nupkg c:\projects\NuGet\