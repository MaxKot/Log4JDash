@echo off
for %%f in (
    "%VS150COMNTOOLS%VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio\2017\Community\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\Tools\VsMSBuildCmd.bat"
    "%VS140COMNTOOLS%VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat"
    "%VS120COMNTOOLS%VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio 12.0\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\Tools\VsMSBuildCmd.bat"
    "%VS110COMNTOOLS%VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio 11.0\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\Tools\VsMSBuildCmd.bat"
) do (
    if exist %%f (
        set VsMSBuildCmd=%%f
        goto :found
    )
)

echo "MSBuild environament setup script was not found."
goto :eof

:found
call %VsMSBuildCmd%

set platform=%1
if "%platform%" equ "" (
    set platform=Win32
)

msbuild src\build.proj /t:Build /p:Platform=%platform%
