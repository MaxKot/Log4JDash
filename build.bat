@setlocal
@echo off
for %%f in (
    "%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
    "%ProgramFiles%\Microsoft Visual Studio\Installer\vswhere.exe"
) do (
    if exist "%%f" (
        set "VsWhere=%%f"
        goto :vswherefound
    )
)

goto :novswhere

:vswherefound
for /F "tokens=* USEBACKQ" %%f in (`%VsWhere% -property installationPath`) do (
    set "VsInstallationPath=%%f"
)

if exist "%VsInstallationPath%\Common7\Tools\VsMSBuildCmd.bat" (
    set VsMSBuildCmd="%VsInstallationPath%\Common7\Tools\VsMSBuildCmd.bat"
    goto :found
)

:novswhere
for %%f in (
    "%VS170COMNTOOLS%VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio\2022\Professional\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio\2022\Community\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2022\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2022\Professional\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2022\Community\Common7\Tools\VsMSBuildCmd.bat"
    "%VS160COMNTOOLS%VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio\2019\Professional\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles%\Microsoft Visual Studio\2019\Community\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\Common7\Tools\VsMSBuildCmd.bat"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\Common7\Tools\VsMSBuildCmd.bat"
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

msbuild %~dp0\src\build.proj /t:Build /p:Platform=%platform%
@endlocal
