@echo off

set CURR_DIR=%CD%

set SCRIPT_DIR=%~dp0

cd %SCRIPT_DIR%
cd ..\src

msbuild /v:m /nologo /p:Configuration=Debug /t:clean /t:build

cd %CURR_DIR%

