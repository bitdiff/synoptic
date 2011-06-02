@echo off

set CURR_DIR=%CD%

set SCRIPT_DIR=%~dp0

cd %SCRIPT_DIR%
cd ..\src

msbuild /verbosity:quiet /nologo /p:Configuration=Release /t:clean /t:build

cd %SCRIPT_DIR%
cd ..
rd /q /s tmp-nuget
mkdir tmp-nuget
mkdir tmp-nuget\lib

copy /y /v bin\Release\Synoptic.*  tmp-nuget\lib
copy /y /v build\synoptic.nuspec tmp-nuget

cd tmp-nuget

nuget pack synoptic.nuspec

copy *.nupkg ..\build

cd ..

rd /s /q tmp-nuget

cd %CURR_DIR%

echo ..........................................................
echo nuget push synoptic.x.x.x.x.nupkg
echo ..........................................................

